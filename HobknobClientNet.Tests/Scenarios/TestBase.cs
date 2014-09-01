using System;
using System.Collections.Generic;
using etcetera;
using NUnit.Framework;

namespace HobknobClientNet.Tests.Scenarios
{
    public class TestBase
    {
        private readonly EtcdClient _etcdClient;
        private HobknobClient _hobknobClient;
        private string _applicationName;

        private const string EtcdHost = "192.168.7.58";
        private const int EtcdPort = 4001;

        private readonly HashSet<string> _applicationKeysToClearOnTearDown = new HashSet<string>();

        protected TestBase()
        {
            _etcdClient = new EtcdClient(new Uri(string.Format("http://{0}:{1}/v2/keys/", EtcdHost, EtcdPort)));
        }

        [TearDown]
        public void TearDown()
        {
            if (_hobknobClient != null)
            {
                _hobknobClient.Dispose();
            }

            foreach (var application in _applicationKeysToClearOnTearDown)
            {
                _etcdClient.DeleteDir("v1/toggles/" + application, true);
            }
            _applicationKeysToClearOnTearDown.Clear();
        }

        protected void Set_application_name(string applicationName)
        {
            _applicationName = applicationName;
        }

        protected void Given_a_toggle(string applicationName, string toggleName, string value)
        {
            var key = string.Format("v1/toggles/{0}/{1}", applicationName, toggleName);
            _etcdClient.Set(key, value);
            _applicationKeysToClearOnTearDown.Add(applicationName);
        }

        protected void When_I_get(string toggleName, out bool? value)
        {
            _hobknobClient = new HobknobClientFactory().Create(EtcdHost, EtcdPort, _applicationName, TimeSpan.FromMinutes(1));
            value = _hobknobClient.Get(toggleName);
        }

        protected void When_I_get_with_default(string toggleName, bool defaultValue,  out bool? value)
        {
            _hobknobClient = new HobknobClientFactory().Create(EtcdHost, EtcdPort, _applicationName, TimeSpan.FromMinutes(1));
            value = _hobknobClient.GetOrDefault(toggleName, defaultValue);
        }
    }
}

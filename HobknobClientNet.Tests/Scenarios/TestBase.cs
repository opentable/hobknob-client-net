using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace HobknobClientNet.Tests.Scenarios
{
    internal class TestBase
    {
        protected readonly EtcdClientForTests EtcdClient;
        private HobknobClient _hobknobClient;
        private string _applicationName;

        private const string EtcdHost = "192.168.7.51";
        private const int EtcdPort = 4001;

        private readonly HashSet<string> _applicationKeysToClearOnTearDown = new HashSet<string>();
        private TimeSpan _cacheUpdateInterval = TimeSpan.FromMinutes(1);

        protected TestBase()
        {
            EtcdClient = new EtcdClientForTests(new Uri(string.Format("http://{0}:{1}/v2/keys/", EtcdHost, EtcdPort)));
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
                EtcdClient.DeleteDir(new Uri("v1/toggles/" + application + "/", UriKind.Relative));
            }
            _applicationKeysToClearOnTearDown.Clear();
        }

        protected void Set_application_name(string applicationName)
        {
            _applicationName = applicationName;
        }

        protected void Set_cache_update_interval(TimeSpan cacheUpdateInterval)
        {
            _cacheUpdateInterval = cacheUpdateInterval;
        }

        protected void Given_a_toggle(string applicationName, string toggleName, string value)
        {
            var key = string.Format("v1/toggles/{0}/{1}", applicationName, toggleName);
            EtcdClient.Set(new Uri(key, UriKind.Relative), value);
            _applicationKeysToClearOnTearDown.Add(applicationName);
        }

        protected void Given_a_toggle_is_removed(string applicationName, string toggleName)
        {
            var key = string.Format("v1/toggles/{0}/{1}", applicationName, toggleName);
            EtcdClient.Delete(new Uri(key, UriKind.Relative));
        }

        protected void When_I_get(string toggleName, out bool? value)
        {
            _hobknobClient = new HobknobClientFactory().Create(EtcdHost, EtcdPort, _applicationName, _cacheUpdateInterval);
            value = _hobknobClient.Get(toggleName);
        }

        protected void When_I_get_without_initialising_a_new_hobknob_instance(string toggleName, out bool? value)
        {
            value = _hobknobClient.Get(toggleName);
        }

        protected void When_I_get_with_default(string toggleName, bool defaultValue,  out bool? value)
        {
            _hobknobClient = new HobknobClientFactory().Create(EtcdHost, EtcdPort, _applicationName, TimeSpan.FromMinutes(1));
            value = _hobknobClient.GetOrDefault(toggleName, defaultValue);
        }

        protected HobknobClient Create_hobknob_client()
        {
            _hobknobClient = new HobknobClientFactory().Create(EtcdHost, EtcdPort, _applicationName, _cacheUpdateInterval);
            return _hobknobClient;
        }
    }
}

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace HobknobClientNet.Tests.Scenarios
{
    internal class TestBase
    {
        protected EtcdClientForTests EtcdClient;
        protected IHobknobClient HobknobClient;
        private string _applicationName;

        protected const string EtcdHost = "192.168.0.8";
        protected const int EtcdPort = 4001;

        private readonly HashSet<string> _applicationKeysToClearOnTearDown = new HashSet<string>();
        private TimeSpan _cacheUpdateInterval = TimeSpan.FromMinutes(1);

        protected TestBase()
        {
            EtcdClient = new EtcdClientForTests(new Uri(string.Format("http://{0}:{1}/v2/keys/", EtcdHost, EtcdPort)));
        }

        [TearDown]
        public void TearDown()
        {
            if (HobknobClient != null)
            {
                HobknobClient.Dispose();
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

        protected void Given_a_toggle(string applicationName, string featureName, string value)
        {
            Given_a_toggle(applicationName, featureName, null, value);
        }

        protected void Given_a_toggle(string applicationName, string featureName, string toggleName, string value)
        {
            var toggleSuffix = toggleName != null ? "/" + toggleName : string.Empty;
            var key = string.Format("v1/toggles/{0}/{1}{2}", applicationName, featureName, toggleSuffix);
            EtcdClient.Set(new Uri(key, UriKind.Relative), value);
            _applicationKeysToClearOnTearDown.Add(applicationName);
        }

        protected void Given_a_toggle_is_removed(string applicationName, string toggleName)
        {
            var key = string.Format("v1/toggles/{0}/{1}", applicationName, toggleName);
            EtcdClient.Delete(new Uri(key, UriKind.Relative));
        }

        protected void When_I_get_with_default(string featureName, bool defaultValue, out bool? value)
        {
            When_I_get_with_default(featureName, null, defaultValue, out value);
        }

        protected void When_I_get_with_default(string featureName, string toggleName, bool defaultValue,  out bool? value)
        {
            HobknobClient = new HobknobClientFactory().Create(EtcdHost, EtcdPort, _applicationName, TimeSpan.FromSeconds(1));
            value = HobknobClient.GetOrDefault(featureName, toggleName, defaultValue);
        }

        protected void When_I_get_with_default_without_initialising_a_new_hobknob_instance(string featureName, out bool? value)
        {
            value = HobknobClient.GetOrDefault(featureName, false);
        }

        protected IHobknobClient Create_hobknob_client(string etcdHost = EtcdHost)
        {
            HobknobClient = new HobknobClientFactory().Create(etcdHost, EtcdPort, _applicationName, _cacheUpdateInterval);
            return HobknobClient;
        }
    }
}

using System;
using etcetera;

namespace HobknobClientNet
{
    public class HobknobClientFactory
    {
        public HobknobClient Create(string etcdHost, int etcdPort, string applicationName, TimeSpan cacheUpdateInterval)
        {
            var applicationDirectoryKey = string.Format("http://{0}:{1}/v2/keys/", etcdHost, etcdPort);

            var etcdClient = new EtcdClient(new Uri(applicationDirectoryKey));
            var featureToggleProvider = new FeatureToggleProvider(etcdClient, applicationName);
            var featureToggleCache = new FeatureToggleCache(featureToggleProvider, cacheUpdateInterval);
            var hobknobClient = new HobknobClient(featureToggleCache, applicationName);

            featureToggleCache.Initialize();

            return hobknobClient;
        }
    }
}

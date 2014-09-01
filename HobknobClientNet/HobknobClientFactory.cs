using System;
using System.Threading;
using etcetera;

namespace HobknobClientNet
{
    public class HobknobClientFactory
    {
        public HobknobClient Create(string etcdHost, int etcdPort, string applicationName, TimeSpan cacheUpdateInterval)
        {
            var applicationDirectoryKey = string.Format("http://{0}:{1}/v2/keys/", etcdHost, etcdPort);
            var cacheInitializationWaitLimit = TimeSpan.FromMinutes(1);

            var etcdClient = new EtcdClient(new Uri(applicationDirectoryKey));
            var featureToggleProvider = new FeatureToggleProvider(etcdClient, applicationName);
            var firstTimeInitializedEvent = new ManualResetEvent(false);
            var featureToggleCache = new FeatureToggleCache(featureToggleProvider, cacheUpdateInterval);
            var hobknobClient = new HobknobClient(featureToggleCache);

            featureToggleCache.CacheUpdated += (sender, args) => firstTimeInitializedEvent.Set();

            Exception cacheUpdateFailedException = null;
            featureToggleCache.CacheUpdateFailed += (sender, args) =>
                {
                    cacheUpdateFailedException = args.Exception;
                    firstTimeInitializedEvent.Set();
                };

            featureToggleCache.Initialize();

            if (!firstTimeInitializedEvent.WaitOne(cacheInitializationWaitLimit))
            {
                throw new Exception(string.Format("Cache did not initialise in time ({0})", cacheInitializationWaitLimit));
            }

            if (cacheUpdateFailedException != null)
            {
                throw cacheUpdateFailedException;
            }

            return hobknobClient;
        }
    }
}

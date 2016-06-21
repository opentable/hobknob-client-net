using System;

namespace HobknobClientNet
{
    public interface IHobknobClient : IDisposable
    {
        event EventHandler<CacheUpdatedArgs> CacheUpdated;
        event EventHandler<CacheUpdateFailedArgs> CacheUpdateFailed;
        bool GetOrDefault(string featureName, bool defaultValue);
        bool GetOrDefault(string featureName, string toggleName, bool defaultValue);
    }

    public class HobknobClient : IHobknobClient
    {
        public event EventHandler<CacheUpdatedArgs> CacheUpdated;
        public event EventHandler<CacheUpdateFailedArgs> CacheUpdateFailed;

        private readonly FeatureToggleCache _featureToggleCache;
        private readonly string _applicationName;

        internal HobknobClient(FeatureToggleCache featureToggleCache, string applicationName)
        {
            _featureToggleCache = featureToggleCache;
            _applicationName = applicationName;

            _featureToggleCache.CacheUpdated += RaiseCacheUpdatedEvent;
            _featureToggleCache.CacheUpdateFailed += RaiseCacheUpdateFailedEvent;
        }

        public bool GetOrDefault(string featureName, bool defaultValue)
        {
            return GetOrDefault(featureName, null, defaultValue);
        }

        public bool GetOrDefault(string featureName, string toggleName, bool defaultValue)
        {
            return _featureToggleCache.Get(_applicationName, featureName, toggleName)
                .GetValueOrDefault(defaultValue);
        }

        private void RaiseCacheUpdatedEvent(object sender, CacheUpdatedArgs eventArgs)
        {
            if (CacheUpdated != null)
            {
                CacheUpdated(sender, eventArgs);
            }
        }

        private void RaiseCacheUpdateFailedEvent(object sender, CacheUpdateFailedArgs eventArgs)
        {
            if (CacheUpdateFailed != null)
            {
                CacheUpdateFailed(sender, eventArgs);
            }
        }

        public void Dispose()
        {
            _featureToggleCache.CacheUpdated -= RaiseCacheUpdatedEvent;
            _featureToggleCache.CacheUpdateFailed -= RaiseCacheUpdateFailedEvent;

            if (_featureToggleCache != null)
            {
                _featureToggleCache.Dispose();
            }
        }
    }
}

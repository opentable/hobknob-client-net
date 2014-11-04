using System;

namespace HobknobClientNet
{
    public class HobknobClient : IDisposable
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

        [Obsolete("Get is dangerous and will be removed in a future version. Use GetOrDefault instead")]
        public bool Get(string toggleName)
        {
            var value = _featureToggleCache.Get(toggleName);
            if (!value.HasValue)
            {
                throw new Exception(string.Format("Key not found for toggle {0}/{1}", _applicationName, toggleName));
            }
            return value.Value;
        }

        public bool GetOrDefault(string featureToggleName, bool defaultValue)
        {
            return _featureToggleCache.Get(featureToggleName).GetValueOrDefault(defaultValue);
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

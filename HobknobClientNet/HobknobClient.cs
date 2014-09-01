using System;

namespace HobknobClientNet
{
    public class HobknobClient : IDisposable
    {
        public event EventHandler CacheUpdated;
        public event EventHandler<CacheUpdateFailedArgs> CacheUpdateFailed;

        private readonly FeatureToggleCache _featureToggleCache;

        internal HobknobClient(FeatureToggleCache featureToggleCache)
        {
            _featureToggleCache = featureToggleCache;
            _featureToggleCache.CacheUpdated += RaiseCacheUpdatedEvent;
            _featureToggleCache.CacheUpdateFailed += RaiseCacheUpdateFailedEvent;
        }

        public bool Get(string featureToggleName)
        {
            var value = _featureToggleCache.Get(featureToggleName);
            if (!value.HasValue)
            {
                throw new Exception("Key not found"); // todo: better exception
            }
            return value.Value;
        }

        public bool GetOrDefault(string featureToggleName, bool defaultValue)
        {
            return _featureToggleCache.Get(featureToggleName).GetValueOrDefault(defaultValue);
        }

        private void RaiseCacheUpdatedEvent(object sender, EventArgs eventArgs)
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
            if (_featureToggleCache != null)
            {
                _featureToggleCache.Dispose();
            }
        }
    }
}

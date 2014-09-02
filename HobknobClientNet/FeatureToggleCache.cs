using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HobknobClientNet
{
    public class FeatureToggleCache : IDisposable
    {
        public event EventHandler CacheUpdated;
        public event EventHandler<CacheUpdateFailedArgs> CacheUpdateFailed;

        private readonly FeatureToggleProvider _featureToggleProvider;
        private readonly TimeSpan _updateInterval;
        private Timer _timer;
        private Dictionary<string, bool> _cache;

        public FeatureToggleCache(FeatureToggleProvider featureToggleProvider, TimeSpan cacheUpdateInterval)
        {
            _featureToggleProvider = featureToggleProvider;
            _updateInterval = cacheUpdateInterval;

            if (_updateInterval < TimeSpan.FromSeconds(1))
            {
                throw new Exception("Cache update interval must be at least 1 second long");
            }
        }

        public void Initialize()
        {
            Exception exception;
            if (!UpdateCache(out exception))
            {
                throw exception;
            }
            _timer = new Timer(UpdateCacheTick, null, _updateInterval, _updateInterval);
        }

        public bool? Get(string featureToggleName)
        {
            bool value;
            return _cache.TryGetValue(featureToggleName, out value) ? value : (bool?)null;
        }

        private bool UpdateCache(out Exception exception)
        {
            try
            {
                var featureToggles = _featureToggleProvider.Get();
                _cache = featureToggles.ToDictionary(x => x.Key, x => x.Value);
            }
            catch (Exception ex)
            {
                if (CacheUpdateFailed != null)
                {
                    CacheUpdateFailed(this, new CacheUpdateFailedArgs(ex));
                }

                exception = ex;
                return false;
            }

            if (CacheUpdated != null)
            {
                CacheUpdated(this, EventArgs.Empty);
            }

            exception = null;
            return true;
        }

        private void UpdateCacheTick(object _)
        {
            Exception ignore;
            UpdateCache(out ignore);
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }
    }
}

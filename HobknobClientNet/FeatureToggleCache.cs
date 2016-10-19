using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HobknobClientNet
{
    internal class FeatureToggleCache : IDisposable
    {
        public event EventHandler<CacheUpdatedArgs> CacheUpdated;
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
            UpdateCache();
            _timer = new Timer(UpdateCacheTick, null, _updateInterval, _updateInterval);
        }

        public bool? Get(string applicationName, string featureName, string toggleName = null)
        {
            var toggleSuffix = toggleName != null ? "/" + toggleName : string.Empty;
            var featureToggleKey = string.Format("/v1/toggles/{0}/{1}{2}", applicationName, featureName, toggleSuffix);
            bool value;
            return _cache.TryGetValue(featureToggleKey, out value) ? value : (bool?)null;
        }

        private bool UpdateCache()
        {
            Dictionary<string, bool> featureToggles;
            try
            {
                featureToggles = _featureToggleProvider.Get().ToDictionary(x => x.Key, x => x.Value);
            }
            catch (Exception ex)
            {
                CacheUpdateFailed?.Invoke(this, new CacheUpdateFailedArgs(ex));
                return false;
            }

            var updates = GetUpdates(_cache, featureToggles);
            _cache = featureToggles;
            CacheUpdated?.Invoke(this, new CacheUpdatedArgs(updates));
            return true;
        }

        private void UpdateCacheTick(object _)
        {
            UpdateCache();
        }

        private static IEnumerable<CacheUpdate> GetUpdates(Dictionary<string, bool> existingToggles, Dictionary<string, bool> newToggles)
        {
            var existingNotNull = existingToggles ?? new Dictionary<string, bool>();

            var updates = new List<CacheUpdate>();
            foreach (var newToggle in newToggles)
            {
                bool existingValue;
                if (existingNotNull.TryGetValue(newToggle.Key, out existingValue))
                {
                    if (existingValue != newToggle.Value)
                    {
                        updates.Add(new CacheUpdate(newToggle.Key, existingValue, newToggle.Value));
                    }
                }
                else
                {
                    updates.Add(new CacheUpdate(newToggle.Key, null, newToggle.Value));
                }
            }

            // removed
            var removedKeys = existingNotNull.Keys.Except(newToggles.Keys);
            foreach (var removedKey in removedKeys)
            {
                updates.Add(new CacheUpdate(removedKey, existingNotNull[removedKey], null));
            }

            return updates;
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

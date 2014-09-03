using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HobknobClientNet
{
    public class FeatureToggleCache : IDisposable
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
            Dictionary<string, bool> featureToggles;
            try
            {
                featureToggles = _featureToggleProvider.Get().ToDictionary(x => x.Key, x => x.Value);
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

            var updates = GetUpdates(_cache, featureToggles);

            _cache = featureToggles;

            if (CacheUpdated != null)
            {
                CacheUpdated(this, new CacheUpdatedArgs(updates));
            }

            exception = null;
            return true;
        }

        private void UpdateCacheTick(object _)
        {
            Exception ignore;
            UpdateCache(out ignore);
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

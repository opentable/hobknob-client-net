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

        private Dictionary<string, bool?> _cache;

        public FeatureToggleCache(FeatureToggleProvider featureToggleProvider, TimeSpan updateInterval)
        {
            _featureToggleProvider = featureToggleProvider;
            // todo: gaurd against crazy short update interval
            _updateInterval = updateInterval;
        }

        public void Initialize()
        {
            _timer = new Timer(UpdateCache, null, TimeSpan.Zero, _updateInterval);
        }

        public bool? Get(string featureToggleName)
        {
            bool? value;
            return _cache.TryGetValue(featureToggleName, out value) ? value : null;
        }

        private void UpdateCache(object _)
        {
            try
            {
                var applicationFeatureToggles = _featureToggleProvider.Get();
                _cache = applicationFeatureToggles.ToDictionary(x => x.Key, x => x.Value);
            }
            catch (Exception ex)
            {
                CacheUpdateFailed(this, new CacheUpdateFailedArgs(ex));
                return;
            }

            if (CacheUpdated != null)
            {
                CacheUpdated(this, EventArgs.Empty);
            }
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

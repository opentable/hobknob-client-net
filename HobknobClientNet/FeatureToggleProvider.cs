using System;
using System.Collections.Generic;
using System.Linq;

namespace HobknobClientNet
{
    internal class FeatureToggleProvider
    {
        private readonly EtcdClient _etcdClient;
        private readonly Uri _applicationDirectoryKey;

        public FeatureToggleProvider(EtcdClient etcdClient, string applicationName)
        {
            _etcdClient = etcdClient;
            _applicationDirectoryKey = new Uri(string.Format("v1/toggles/{0}", applicationName), UriKind.Relative);
        }

        public IEnumerable<KeyValuePair<string, bool>> Get()
        {
            var etcdResponse = _etcdClient.Get(_applicationDirectoryKey);

            if (etcdResponse == null || etcdResponse.Node.Nodes == null)
            {
                return Enumerable.Empty<KeyValuePair<string, bool>>();
            }

            return etcdResponse.Node.Nodes
                .Select(node =>
                {
                    if (node.Dir)
                    {
                        return node.Nodes
                            .Where(x => !x.Key.EndsWith("/@meta"))
                            .Select(x => new KeyValuePair<string, bool?>(ExtractFeatureToggleName(x.Key),
                                ParseFeatureToggleValue(x.Key, x.Value)))
                            .ToArray();
                    }
                    return new[]
                    {
                        new KeyValuePair<string, bool?>(ExtractFeatureToggleName(node.Key),
                            ParseFeatureToggleValue(node.Key, node.Value))
                    };
                })
                .SelectMany(x => x)
                .Where(pair => pair.Value.HasValue)
                .Select(pair => new KeyValuePair<string, bool>(pair.Key, pair.Value.Value));
        }

        private static bool? ParseFeatureToggleValue(string key, string value)
        {
            if (value == null) return null;

            switch (value.ToLower())
            {
                case "true":
                    return true;
                case "false":
                    return false;
                default:
                    return null;
            }
        }

        private static string ExtractFeatureToggleName(string name)
        {
            return name.Split('/').Last();
        }
    }
}

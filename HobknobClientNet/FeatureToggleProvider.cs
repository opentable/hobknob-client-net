using System.Collections.Generic;
using System.Linq;
using etcetera;

namespace HobknobClientNet
{
    public class FeatureToggleProvider
    {
        private readonly EtcdClient _etcdClient;
        private readonly string _applicationDirectoryKey;

        public FeatureToggleProvider(EtcdClient etcdClient, string applicationName)
        {
            _etcdClient = etcdClient;
            _applicationDirectoryKey = string.Format("v1/toggles/{0}", applicationName);
        }

        public IEnumerable<KeyValuePair<string, bool?>> Get()
        {
            var etcdResponse = _etcdClient.Get(_applicationDirectoryKey); // todo: does this support recursive node gets?

            // todo: check for error? node == null?
            return etcdResponse.Node.Nodes.Select(GetKeyValuePair);
        }

        private static KeyValuePair<string, bool?> GetKeyValuePair(Node node)
        {
            return new KeyValuePair<string, bool?>(node.Key, ParseFeatureToggleValue(node.Value));
        }

        private static bool? ParseFeatureToggleValue(string value)
        {
            if (value == null) return null;

            switch (value.ToLower())
            {
                case "true":
                    return true;
                case "false":
                    return false;
                default:
                    // todo: exception? log?
                    return null;
            }
        }
    }
}

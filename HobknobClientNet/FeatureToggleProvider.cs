using System;
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

            if (etcdResponse.ErrorCode.HasValue)
            {
                const int keyNotFoundEtcdErrorCode = 100;
                if (etcdResponse.ErrorCode == keyNotFoundEtcdErrorCode)
                {
                    return Enumerable.Empty<KeyValuePair<string, bool?>>();
                }
                throw new Exception("Error getting toggles from etcd: " + etcdResponse.Message);
            }

            return etcdResponse.Node.Nodes.Select(GetKeyValuePair);
        }

        private static KeyValuePair<string, bool?> GetKeyValuePair(Node node)
        {
            var key = GetKey(node.Key);
            var featureToggleValue = ParseFeatureToggleValue(node.Value);
            return new KeyValuePair<string, bool?>(key, featureToggleValue);
        }

        private static string GetKey(string path)
        {
            return path.Split('/').Last();
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

using System;
using System.Collections.Generic;
using System.Linq;
using etcetera;

namespace HobknobClientNet
{
    internal class FeatureToggleProvider
    {
        private readonly EtcdClient _etcdClient;
        private readonly string _applicationDirectoryKey;

        public FeatureToggleProvider(EtcdClient etcdClient, string applicationName)
        {
            _etcdClient = etcdClient;
            _applicationDirectoryKey = string.Format("v1/toggles/{0}", applicationName);
        }

        public IEnumerable<KeyValuePair<string, bool>> Get()
        {
            var etcdResponse = _etcdClient.Get(_applicationDirectoryKey);

            if (etcdResponse.ErrorCode.HasValue)
            {
                const int keyNotFoundEtcdErrorCode = 100;
                if (etcdResponse.ErrorCode == keyNotFoundEtcdErrorCode)
                {
                    return Enumerable.Empty<KeyValuePair<string, bool>>();
                }
                throw new Exception("Error getting toggles from etcd: " + etcdResponse.Message);
            }

            return etcdResponse.Node.Nodes
                .Select(node => new { Key = GetKey(node.Key), Value = ParseFeatureToggleValue(node.Key, node.Value) })
                .Where(pair => pair.Value.HasValue)
                .Select(pair => new KeyValuePair<string, bool>(pair.Key, pair.Value.Value));
        }

        private static string GetKey(string path)
        {
            return path.Split('/').Last();
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
    }
}

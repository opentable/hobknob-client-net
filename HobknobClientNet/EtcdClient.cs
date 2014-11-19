using System.Net;
using Newtonsoft.Json;

namespace HobknobClientNet
{
    public class EtcdClient
    {
        readonly string _etcdHost;
        readonly string _etcdPort;
        readonly WebClient _webClient;

        public EtcdClient(string etcdHost, string etcdPort)
        {
            _etcdPort = etcdPort;
            _etcdHost = etcdHost;
            _webClient = new WebClient();
            _webClient.Headers.Add("Accept", "application/json");
        }

        public EtcdResponse Get(string key)
        {
            var fullKeyUrl = string.Format("http://{0}:{1}/v2/keys/{2}", _etcdHost, _etcdPort, key);
            string etcdResponseJson = _webClient.DownloadString(fullKeyUrl);
            return JsonConvert.DeserialiseObject<EtcdResponse>(etcdResponseJson);
        }
    }

    public class EtcdResponse
    {
        public EtcdResponse()
        {
            Headers = new EtcdHeaders();
        }

        public string Action { get; set; }
        public Node Node { get; set; }

        public int? ErrorCode { get; set; }
        public string Cause { get; set; }
        public int? Index { get; set; }
        public string Message { get; set; }
    }

    public class EtcdHeaders
    {
        public int EtcdIndex { get; set; }
        public int? RaftIndex { get; set; }
        public int? RaftTerm { get; set; }
    }

    public class Node
    {
        public int CreatedIndex { get; set; }
        public string Key { get; set; }
        public int ModifiedIndex { get; set; }
        public string Value { get; set; }
        public int? Ttl { get; set; }
        public DateTime? Expiration { get; set; }
        public List<Node> Nodes { get; set; }
        public bool Dir { get; set; }
    }
}

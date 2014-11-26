using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace HobknobClientNet
{
    internal class EtcdClient
    {
        private readonly Uri _keysUri;

        public EtcdClient(Uri keysUri)
        {
            _keysUri = keysUri;
        }

        public EtcdResponse Get(Uri relativeKeyUri, bool recursive = true)
        {
            var fullKeyUriWithQuery = new UriBuilder(GetFullUri(relativeKeyUri)) { Query = "recursive=" + recursive };
            var webRequest = (HttpWebRequest)WebRequest.Create(fullKeyUriWithQuery.Uri);
            webRequest.Accept = "application/json";
            webRequest.UserAgent = "hobknob-client-net";

            try
            {
                using (var webResponse = webRequest.GetResponse())
                {
                    return GetEtcdResponse(webResponse);
                }
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

        private static EtcdResponse GetEtcdResponse(WebResponse webResponse)
        {
            string etcdResponseJson;
            using (var responseStream = webResponse.GetResponseStream())
            using (var reader = new StreamReader(responseStream, Encoding.UTF8))
            {
                etcdResponseJson = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<EtcdResponse>(etcdResponseJson);
        }

        protected Uri GetFullUri(Uri relativeUri)
        {
            return new Uri(_keysUri, relativeUri);
        }
    }
}

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace HobknobClientNet
{
    internal class EtcdClient : IDisposable
    {
        private static HttpClient _httpClient;

        private static HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();

                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("hobknob-client-net")));
                }

                return _httpClient;
            }
        }

        private readonly Uri _keysUri;

        public EtcdClient(Uri keysUri)
        {
            _keysUri = keysUri;
        }

        public EtcdResponse Get(Uri relativeKeyUri, bool recursive = true)
        {
            var fullKeyUriWithQuery = new UriBuilder(GetFullUri(relativeKeyUri)) {Query = "recursive=" + recursive};
            
            var response = HttpClient.GetAsync(fullKeyUriWithQuery.Uri).Result;
                
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
                
            return GetEtcdResponse(response);
        }

        private static EtcdResponse GetEtcdResponse(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<EtcdResponse>(response.Content.ReadAsStringAsync().Result);
        }

        protected Uri GetFullUri(Uri relativeUri)
        {
            return new Uri(_keysUri, relativeUri);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _httpClient = null;
        }
    }
}

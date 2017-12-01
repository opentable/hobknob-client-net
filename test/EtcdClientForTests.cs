using System;
using System.Net;
using System.Text;

namespace HobknobClientNet.Tests
{
    internal class EtcdClientForTests : EtcdClient
    {
        public EtcdClientForTests(Uri keysUri) : base(keysUri)
        {
        }

        public void Set(Uri relativeKeyUri, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("value=" + value);

            var webRequest = (HttpWebRequest)WebRequest.Create(GetFullUri(relativeKeyUri));
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "PUT";
            webRequest.ContentLength = bytes.Length;

            using (var dataStream = webRequest.GetRequestStream())
            {
                dataStream.Write(bytes, 0, bytes.Length);
                dataStream.Flush();
                using (webRequest.GetResponse())
                {
                }
            }
        }

        public void DeleteDir(Uri relativeKeyUri)
        {
            if (!relativeKeyUri.OriginalString.EndsWith("/"))
            {
                throw new ArgumentException("Use Delete to delete a key, or end the key with a forward slash.to delete a directory.");
            }
            var fullKeyUriWithQuery = new UriBuilder(GetFullUri(relativeKeyUri)) { Query = "recursive=true" };

            var webRequest = (HttpWebRequest)WebRequest.Create(fullKeyUriWithQuery.Uri);
            webRequest.Method = "DELETE";

            try
            {
                using (webRequest.GetResponse())
                {
                }
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    return;
                }
                throw;
            }
        }

        public void Delete(Uri relativeKeyUri)
        {
            if (relativeKeyUri.OriginalString.EndsWith("/"))
            {
                throw new ArgumentException("Use DeleteDir to delete a directory, or don't end the key with a forward slash.");
            }

            var webRequest = (HttpWebRequest)WebRequest.Create(GetFullUri(relativeKeyUri));
            webRequest.Method = "DELETE";
            try
            {
                using (webRequest.GetResponse())
                {
                }
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    return;
                }
                throw;
            }
        }
    }
}

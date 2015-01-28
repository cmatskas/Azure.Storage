using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Storage.Portable
{
    public class BlobStorage
    {
        /* <service name="Blob" url="http://127.0.0.1:10000/"/>
        <service name="Queue" url="http://127.0.0.1:10001/"/>
        <service name="Table" url="http://127.0.0.1:10002/"/> 
        private string blobUrl = "http://127.0.0.1:10000/devstoreaccount1/";
        private string queueUrl = "http://127.0.0.1:10000/devstoreaccount1/";
        private string tableUrl = "http://127.0.0.1:10000/devstoreaccount1/"; */

        /*private string blobUrl = "http://ipv4.fiddler:10000/devstoreaccount1/";
        private string queueUrl = "http://ipv4.fiddler:10000/devstoreaccount1/";
        private string tableUrl = "http://ipv4.fiddler:10000/devstoreaccount1/"; */
        private const string StorageServiceVersion = "2009-09-19";

        private readonly string containerName;
        private readonly string account;
        private readonly string key;
        private readonly string endpointUrl;

        public BlobStorage(string endpointUrl, string containerName, string account, string key, bool isPublic = true)
        {
            this.containerName = containerName;
            this.account = account;
            this.key = key;
            this.endpointUrl = endpointUrl;
        }

        private string CreateAuthorizationHeader(string canonicalizedString)
        {
            string signature;
            using (var hmacSha256 = new HMACSHA256(Convert.FromBase64String(key)))
            {
                var dataToHmac = Encoding.UTF8.GetBytes(canonicalizedString);
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", account, signature);
        }

        public string CreateContainer()
        {
            var urlPath = String.Format("{0}?{1}", containerName, "restype=container");
            
            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var canonicalizedHeaders = String.Format(
                "x-ms-date:{0}\nx-ms-version:{1}",
                dateInRfc1123Format,
                StorageServiceVersion);
            var canonicalizedResource = String.Format("/{0}/{0}/{1}\n{2}", "devstoreaccount1", containerName, "restype:container");

            var stringToSign = String.Format("PUT\n" +
                                             "\n" +
                                             "\n" +
                                             "0\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
                                             "{0}\n" +
                                             "{1}",
                canonicalizedHeaders,
                canonicalizedResource);

            var authorizationHeader = CreateAuthorizationHeader(stringToSign);

            var uri = new Uri(endpointUrl + urlPath);
            var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage(new HttpMethod("PUT"), uri);
            httpRequest.Headers.Add("x-ms-date", dateInRfc1123Format);
            httpRequest.Headers.Add("x-ms-version", StorageServiceVersion);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("SharedKey", authorizationHeader);

            var response = httpClient.SendAsync(httpRequest).Result;
            IEnumerable<string> values;
            
            return response.Headers.TryGetValues("Etag", out values) ? values.First() : string.Empty;
        }

        public string ChangeContainerAccess(bool makePublic = true)
        {
            var urlPath = String.Format("{0}?{1}&{2}", containerName, "comp=acl","restype=container");
            //var data = GetContainerAclRequestBody();
            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var canonicalizedHeaders = String.Format(
                "x-ms-blob-public-access:container\nx-ms-date:{0}\nx-ms-version:{1}",
                dateInRfc1123Format,
                StorageServiceVersion);
            var canonicalizedResource = String.Format("/{0}/{0}/{1}\n{2}\n{3}", "devstoreaccount1", containerName, "comp:acl", "restype:container");
            var stringToSign = String.Format("PUT\n" +
                                             "\n" +
                                             "\n" +
                                             "0\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
                                             "{0}\n" +
                                             "{1}",
                canonicalizedHeaders,
                canonicalizedResource);

            var authorizationHeader = CreateAuthorizationHeader(stringToSign);
            
            var uri = new Uri(endpointUrl + urlPath);
            var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage(new HttpMethod("PUT"), uri);
            httpRequest.Headers.Add("x-ms-date", dateInRfc1123Format);
            httpRequest.Headers.Add("x-ms-version", StorageServiceVersion);
            httpRequest.Headers.Add("x-ms-blob-public-access","container");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("SharedKey", authorizationHeader);

            var response = httpClient.SendAsync(httpRequest).Result;
            IEnumerable<string> values;

            return response.Headers.TryGetValues("Etag", out values) ? values.First() : string.Empty;
        }

        public string CreateBlockBlob(string blobName, Stream data)
        {
            var urlPath = String.Format("{0}/{1}", containerName, blobName);
            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var canonicalizedHeaders = String.Format(
                "x-ms-blob-type:{0}\nx-ms-date:{1}\nx-ms-version:{2}",
                "BlockBlob",
                dateInRfc1123Format,
                StorageServiceVersion);
            var canonicalizedResource = String.Format("/{0}/{0}/{1}", "devstoreaccount1", urlPath);

            var stringToSign = String.Format("{0}\n\n\n{1}\n\n\n\n\n\n\n\n\n{2}\n{3}",
                "PUT",
                data.Length,
                canonicalizedHeaders,
                canonicalizedResource);

            var authorizationHeader = CreateAuthorizationHeader(stringToSign);

            var uri = new Uri(endpointUrl + urlPath);
            var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage(new HttpMethod("PUT"), uri);
            httpRequest.Headers.Add("x-ms-date", dateInRfc1123Format);
            httpRequest.Headers.Add("x-ms-version", StorageServiceVersion);
            httpRequest.Headers.Add("x-ms-blob-type", "BlockBlob");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("SharedKey", authorizationHeader);
            httpRequest.Content = new StreamContent(data);

            var response = httpClient.SendAsync(httpRequest).Result;
            IEnumerable<string> values;

            return response.Headers.TryGetValues("Etag", out values) ? values.First() : string.Empty;
        }

        public string CreateBlockBlob(string blobName, byte[] data)
        {
            var stream = new MemoryStream(data);
            return CreateBlockBlob(blobName, stream);
        }

        public string CreateBlockBlob(string blobName, string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var stream = new MemoryStream(bytes);
            return CreateBlockBlob(blobName, stream);
        }

        public Task GetBlockBlobDataAsStreamAsync(string blobId)
        {
            throw new NotImplementedException();
        }

        public Task GetBlockBlobDataAsStringAsync(string blobId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetBlockBlobsInContainer(string containerName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBlobContainerAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteBlobAsync(string blobId)
        {
            throw new NotImplementedException();
        }
    }
}

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
using System.Xml.Serialization;
using Azure.Storage.Portable.Interfaces;

namespace Azure.Storage.Portable
{
    public class BlobStorage : IBlobStorage
    {
        private const string StorageServiceVersion = "2009-09-19";

        private readonly string containerName;
        private readonly string account;
        private readonly string key;
        private readonly string endpointUrl;
        private bool runsOnEmulator = false;

        public BlobStorage(string endpointUrl, string containerName, string account, string key, bool makePublic = true)
        {
            Validate.String(endpointUrl, "endpointUrl");
            Validate.BlobContainerName(containerName, "containerName");
            Validate.String(account, "account");
            Validate.String(key, "key");

            this.containerName = containerName;
            this.account = account;
            this.key = key;
            this.endpointUrl = endpointUrl;
            runsOnEmulator = endpointUrl.Contains("127.0.0.1") || endpointUrl.Contains("fiddler");

            var response = GetContainer(); //find it
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                response = CreateContainer(); // create it
            }

            ChangeContainerAccess(makePublic); // change ACL settings
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

        public HttpResponseMessage GetContainer()
        {
            var urlPath = String.Format("{0}?{1}", containerName, "restype=container");
            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var canonicalizedHeaders = String.Format(
                "x-ms-date:{0}\nx-ms-version:{1}",
                dateInRfc1123Format,
                StorageServiceVersion);

            var canonicalizedResource = runsOnEmulator
                ? String.Format("/{0}/{0}/{1}\n{2}", "devstoreaccount1", containerName, "restype:container")
                : String.Format("/{0}/{1}\n{2}", account, containerName, "restype:container");
            
            var stringToSign = String.Format("GET\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
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
            var httpRequest = new HttpRequestMessage(new HttpMethod("GET"), uri);
            httpRequest.Headers.Add("x-ms-date", dateInRfc1123Format);
            httpRequest.Headers.Add("x-ms-version", StorageServiceVersion);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("SharedKey", authorizationHeader);

            return httpClient.SendAsync(httpRequest).Result;
        }

        private HttpResponseMessage CreateContainer()
        {
            var urlPath = String.Format("{0}?{1}", containerName, "restype=container");
            
            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var canonicalizedHeaders = String.Format(
                "x-ms-date:{0}\nx-ms-version:{1}",
                dateInRfc1123Format,
                StorageServiceVersion);
            var canonicalizedResource = runsOnEmulator
                ? String.Format("/{0}/{0}/{1}\n{2}", "devstoreaccount1", containerName, "restype:container")
                : String.Format("/{0}/{1}\n{2}", account, containerName, "restype:container");

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

            return httpClient.SendAsync(httpRequest).Result;
        }

        public HttpResponseMessage ChangeContainerAccess(bool makePublic = true)
        {
            var urlPath = String.Format("{0}?{1}&{2}", containerName, "comp=acl","restype=container");
            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var canonicalizedHeaders = makePublic
                    ? String.Format("x-ms-blob-public-access:container\nx-ms-date:{0}\nx-ms-version:{1}",
                            dateInRfc1123Format, StorageServiceVersion)
                    : String.Format("x-ms-date:{0}\nx-ms-version:{1}", dateInRfc1123Format, StorageServiceVersion);

            var canonicalizedResource = runsOnEmulator
                ? String.Format("/{0}/{0}/{1}\n{2}\n{3}", "devstoreaccount1", containerName, "comp:acl",
                    "restype:container")
                : String.Format("/{0}/{1}\n{2}\n{3}", account, containerName, "comp:acl", "restype:container");
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
            if (makePublic)
            {
                httpRequest.Headers.Add("x-ms-blob-public-access", "container");
            }

            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("SharedKey", authorizationHeader);

            return httpClient.SendAsync(httpRequest).Result;
        }

        public HttpResponseMessage CreateBlockBlob(string blobName, Stream data)
        {
            Validate.BlobName(blobName, "blobName");
            Validate.Stream(data, "data");

            var urlPath = String.Format("{0}/{1}", containerName, blobName);
            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var canonicalizedHeaders = String.Format(
                "x-ms-blob-type:{0}\nx-ms-date:{1}\nx-ms-version:{2}",
                "BlockBlob",
                dateInRfc1123Format,
                StorageServiceVersion);

            var canonicalizedResource = runsOnEmulator
                ? String.Format("/{0}/{0}/{1}", "devstoreaccount1", urlPath)
                : String.Format("/{0}/{1}", account, urlPath);

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

            return httpClient.SendAsync(httpRequest).Result;
        }

        public HttpResponseMessage CreateBlockBlob(string blobName, byte[] data)
        {
            Validate.BlobName(blobName, "blobName");
            Validate.ByteArray(data, "data");

            var stream = new MemoryStream(data);
            return CreateBlockBlob(blobName, stream);
        }

        public HttpResponseMessage CreateBlockBlob(string blobName, string data)
        {
            Validate.BlobName(blobName, "blobName");
            Validate.String(data, "data");

            var bytes = Encoding.UTF8.GetBytes(data);
            var stream = new MemoryStream(bytes);
            return CreateBlockBlob(blobName, stream);
        }

        public Stream GetBlockBlobDataAsStream(string blobName)
        {
            Validate.BlobName(blobName, "blobName");

            var urlPath = String.Format("{0}/{1}", containerName, blobName);
            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var canonicalizedHeaders = String.Format(
                "x-ms-date:{0}\nx-ms-version:{1}",
                dateInRfc1123Format,
                StorageServiceVersion);
            var canonicalizedResource = runsOnEmulator
                ? String.Format("/{0}/{0}/{1}/{2}", "devstoreaccount1", containerName, blobName)
                : String.Format("/{0}/{1}/{2}", account, containerName, blobName);
            var stringToSign = String.Format("GET\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
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
            var httpRequest = new HttpRequestMessage(new HttpMethod("GET"), uri);
            httpRequest.Headers.Add("x-ms-date", dateInRfc1123Format);
            httpRequest.Headers.Add("x-ms-version", StorageServiceVersion);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("SharedKey", authorizationHeader);

            var response = httpClient.SendAsync(httpRequest).Result;
            return response.Content.ReadAsStreamAsync().Result;
        }

        public string GetBlockBlobDataAsString(string blobName)
        {
            string content;
            var stream = GetBlockBlobDataAsStream(blobName);
            using (var reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }

            return content;
        }

        public IEnumerable<string> ListBlobsInContainer()
        {
            var urlPath = String.Format("{0}?{1}&{2}", containerName, "comp=list", "restype=container");
            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var canonicalizedHeaders = String.Format(
                "x-ms-date:{0}\nx-ms-version:{1}",
                dateInRfc1123Format,
                StorageServiceVersion);
            var canonicalizedResource = runsOnEmulator 
            ? String.Format("/{0}/{0}/{1}\n{2}\n{3}", "devstoreaccount1", containerName, "comp:list", "restype:container")
            : String.Format("/{0}/{1}\n{2}\n{3}", account, containerName, "comp:list", "restype:container");

            var stringToSign = String.Format("GET\n" +
                                             "\n" +
                                             "\n" +
                                             "\n" +
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
            var httpRequest = new HttpRequestMessage(new HttpMethod("GET"), uri);
            httpRequest.Headers.Add("x-ms-date", dateInRfc1123Format);
            httpRequest.Headers.Add("x-ms-version", StorageServiceVersion);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("SharedKey", authorizationHeader);
            var response = httpClient.SendAsync(httpRequest).Result;
            var contents = response.Content.ReadAsStringAsync().Result;
            return GetBlobListFromResponse(contents);
        }

        private static IEnumerable<string> GetBlobListFromResponse(string xml)
        {
            var serializer = new XmlSerializer(typeof(EnumerationResults));
            EnumerationResults blobs;
            using (TextReader reader = new StringReader(xml))
            {
                blobs = (EnumerationResults)serializer.Deserialize(reader);
            }

            return blobs.Blobs.Select(t => t.Url).ToList();
        }

        public HttpResponseMessage DeleteBlobContainer()
        {
            var getResponse = GetContainer();
            if (getResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var urlPath = String.Format("{0}?{1}", containerName, "restype=container");
            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var canonicalizedHeaders = String.Format(
                "x-ms-date:{0}\nx-ms-version:{1}",
                dateInRfc1123Format,
                StorageServiceVersion);
            var canonicalizedResource = runsOnEmulator
                ? String.Format("/{0}/{0}/{1}\n{2}", "devstoreaccount1", containerName, "restype:container")
                : String.Format("/{0}/{1}\n{2}", account, containerName, "restype:container");
            var stringToSign = String.Format("DELETE\n" +
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
            var httpRequest = new HttpRequestMessage(new HttpMethod("DELETE"), uri);
            httpRequest.Headers.Add("x-ms-date", dateInRfc1123Format);
            httpRequest.Headers.Add("x-ms-version", StorageServiceVersion);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("SharedKey", authorizationHeader);

            return httpClient.SendAsync(httpRequest).Result;
        }

        public HttpResponseMessage DeleteBlob(string blobName)
        {
            var urlPath = String.Format("{0}/{1}", containerName, blobName);
            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var canonicalizedHeaders = String.Format(
                "x-ms-date:{0}\nx-ms-version:{1}",
                dateInRfc1123Format,
                StorageServiceVersion);
            var canonicalizedResource = runsOnEmulator
                ? String.Format("/{0}/{0}/{1}/{2}", "devstoreaccount1", containerName, blobName)
                : String.Format("/{0}/{1}/{2}", account, containerName, blobName);
            var stringToSign = String.Format("DELETE\n" +
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
            var httpRequest = new HttpRequestMessage(new HttpMethod("DELETE"), uri);
            httpRequest.Headers.Add("x-ms-date", dateInRfc1123Format);
            httpRequest.Headers.Add("x-ms-version", StorageServiceVersion);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("SharedKey", authorizationHeader);

            return httpClient.SendAsync(httpRequest).Result;
        }
    }
}

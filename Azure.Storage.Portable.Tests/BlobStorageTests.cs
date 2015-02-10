using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Xunit;

namespace Azure.Storage.Portable.Tests
{
    public class BlobStorageTests: IDisposable
    {
        private const string ContainerName = "restapicontainertest";
        private const string ContainerToDelete = "containertodelete";
        private const string ContainerToEnumerate = "containertoenumerate";
        private const string Account = "devstoreaccount1";
        private const string Key = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

        
        private const string EndpointUrl = "http://127.0.0.1:10000/devstoreaccount1/";
        //private const string EndpointUrl = "http://ipv4.fiddler:10000/devstoreaccount1/";

        private readonly BlobStorage blobStorage;

        public BlobStorageTests()
        {
            blobStorage = new BlobStorage(EndpointUrl, ContainerName, Account, Key);
        }

        public void Dispose()
        {
            //blobStorage.DeleteBlobContainer();
        }
        
        [Fact]
        public void CreateBlockBlobFromStreamShouldSucceed()
        {
            var bytes = Encoding.UTF8.GetBytes("hello world");
            var stream = new MemoryStream(bytes);
            var result = blobStorage.CreateBlockBlob("resttestblob", stream);
            Assert.True(result.StatusCode == HttpStatusCode.Created);
        }

        [Fact]
        public void CreateBlockBlobFromBytesShouldSucceed()
        {
            var bytes = Encoding.UTF8.GetBytes("hello world");
            var result = blobStorage.CreateBlockBlob("resttestbyteblob", bytes);
            Assert.True(result.StatusCode == HttpStatusCode.Created);
        }

        [Fact]
        public void CreateBlockBlobFromStringShouldSucceed()
        {
            var result = blobStorage.CreateBlockBlob("restteststringblob", "hello World");
            Assert.True(result.StatusCode == HttpStatusCode.Created);
        }

        [Fact]
        public void SetContainerAclToPublicShouldSucceed()
        {
            var result = blobStorage.ChangeContainerAccess();
            Assert.True(result.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public void SetContainerAclToPrivateShouldSucceed()
        {
            var result = blobStorage.ChangeContainerAccess(false);
            Assert.True(result.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public void DeleteContainerShouldSucceed()
        {
            var blobContainerToDelete = new BlobStorage(EndpointUrl, ContainerToDelete, Account, Key);
            var result = blobContainerToDelete.DeleteBlobContainer();
            Assert.True(result.StatusCode == HttpStatusCode.Accepted);
        }

        [Fact]
        public void ListBlobsInContainerShouldSucceed()
        {
            var blobContainerToEnumerage = new BlobStorage(EndpointUrl, ContainerToEnumerate, Account, Key);
            blobContainerToEnumerage.CreateBlockBlob("restteststringblob", "hello World");
            blobContainerToEnumerage.CreateBlockBlob("restteststringblobtwo", "hello World again");

            var results = blobContainerToEnumerage.ListBlobsInContainer().ToList<string>();
            Assert.True(results.Count == 2);

            blobContainerToEnumerage.DeleteBlobContainer();
        }

        [Fact]
        public void DeleteBlobShouldSucceed()
        {
            const string blobToDelete = "blobtodelete";
            var result = blobStorage.CreateBlockBlob(blobToDelete, "hello World");
            result = blobStorage.DeleteBlob(blobToDelete);

            Assert.True(result.StatusCode == HttpStatusCode.Accepted);
        }

        [Fact]
        public void GetBlobAsStreamShouldSucceed()
        {
            const string blobToDownload = "blobstreamtest";
            const string originalText = "testing is fun";            
            var result = blobStorage.CreateBlockBlob(blobToDownload, originalText);
            var stream = blobStorage.GetBlockBlobDataAsStream(blobToDownload);
            string downloadedText;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                downloadedText = reader.ReadToEnd();
            }

            Assert.True(originalText.Equals(downloadedText, StringComparison.CurrentCultureIgnoreCase));
        }

        [Fact]
        public void GetBlobAsStringShouldSucceed()
        {
            const string blobToDownload = "blobstringtest";
            const string originalText = "testing is fun";
            var result = blobStorage.CreateBlockBlob(blobToDownload, originalText);
            var downloadedText = blobStorage.GetBlockBlobDataAsString(blobToDownload);

            Assert.True(originalText.Equals(downloadedText, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}

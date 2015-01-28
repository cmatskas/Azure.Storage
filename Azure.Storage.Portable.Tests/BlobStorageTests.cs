using System;
using System.IO;
using System.Text;
using Xunit;

namespace Azure.Storage.Portable.Tests
{
    public class BlobStorageTests: IDisposable
    {
        private string containerName = "restapicontainertest";
        private const string Account = "devstoreaccount1";
        private const string Key = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
        //private const string EndpointUrl = "http://127.0.0.1:10000/devstoreaccount1/";
        private const string EndpointUrl = "http://ipv4.fiddler:10000/devstoreaccount1/";

        private readonly BlobStorage blobStorage;

        public BlobStorageTests()
        {
            blobStorage = new BlobStorage(EndpointUrl, containerName, Account, Key);
        }

        public void Dispose()
        {
            //clean up in necessary
        }

        [Fact]
        public void CreateContainerSucceeds()
        {
            var result = blobStorage.CreateContainer();
        }

        [Fact]
        public void CreateBlockBlobFromStreamShouldSucceed()
        {
            var bytes = Encoding.UTF8.GetBytes("hello world");
            var stream = new MemoryStream(bytes);
            var result = blobStorage.CreateBlockBlob("resttestblob", stream);
        }

        [Fact]
        public void CreateBlockBlobFromBytesShouldSucceed()
        {
            var bytes = Encoding.UTF8.GetBytes("hello world");
            var result = blobStorage.CreateBlockBlob("resttestbyteblob", bytes);
        }

        [Fact]
        public void SetContainerAclToPublicShouldSucceed()
        {
            var result = blobStorage.ChangeContainerAccess();
        }

        [Fact]
        public void CreateBlockBlobFromStringShouldSucceed()
        {
            var result = blobStorage.CreateBlockBlob("restteststringblob", "hello World");
        }
    }
}

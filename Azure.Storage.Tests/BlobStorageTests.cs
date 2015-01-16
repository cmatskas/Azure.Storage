using System;
using System.IO;
using System.Text;
using Azure.Storage.Interfaces;
using Xunit;

namespace Azure.Storage.Tests
{
    public class BlobStorageTests : IDisposable
    {
        private const string ContainerName = "test-container";
        private const string ConnectionString = "UseDevelopmentStorage=true";
        private readonly IBlobStorage blobStorage;

        public BlobStorageTests()
        {
            blobStorage = new BlobStorage(ContainerName, ConnectionString);
        }

        public void Dispose()
        {
            blobStorage.DeleteBlobContainer();
            // can be used to dispose stuff, if anything changes in the future
        }

        [Fact]
        public void CreateBlobFromSreamSucceeds()
        {
            const string streamBlob = "streamblob";
            var bytes = Encoding.UTF8.GetBytes("hello world");
            var stream = new MemoryStream(bytes);
            var result = blobStorage.CreateBlockBlob(streamBlob, "text/plain", stream);

            Assert.NotNull(result);
        }

        [Fact]
        public void CreateBlobFromBytesSucceeds()
        {
            const string streamBlob = "streamblob";
            var bytes = Encoding.UTF8.GetBytes("hello world");
            var result = blobStorage.CreateBlockBlob(streamBlob, "text/plain", bytes);

            Assert.NotNull(result);
        }
    }
}

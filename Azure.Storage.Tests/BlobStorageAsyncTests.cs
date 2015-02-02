using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Interfaces;
using Xunit;

namespace Azure.Storage.Tests
{
    public class BlobStorageAsyncTests : IDisposable
    {
        private const string ContainerName = "test-container";
        private const string ConnectionString = "UseDevelopmentStorage=true";
        private readonly IBlobStorageAsync blobStorage;

        public BlobStorageAsyncTests()
        {
            blobStorage = new BlobStorageAsync(ContainerName, ConnectionString);
        }

        public void Dispose()
        {
            blobStorage.DeleteBlobContainerAsync().Wait();
            // Dispose anything else that needs to be cleand out
        }

        [Fact]
        public async Task CreateBlobFromSreamAsyncSucceeds()
        {
            const string streamBlob = "streamblob";
            var bytes = Encoding.UTF8.GetBytes("hello world");
            var stream = new MemoryStream(bytes);
            var result = await blobStorage.CreateBlockBlobAsync(streamBlob, "text/plain", stream);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateBlobFromBytesAsyncSucceeds()
        {
            const string streamBlob = "streamblob";
            var bytes = Encoding.UTF8.GetBytes("hello world");
            var result = await blobStorage.CreateBlockBlobAsync(streamBlob, "text/plain", bytes);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateBlobFromStringAsyncSucceeds()
        {
            const string stringBlob = "stringblob";
            var result = await blobStorage.CreateBlockBlobAsync(stringBlob, "text/plain", "hello world");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateBlobFromFileAsyncSucceeds()
        {
            const string fileBlob = "fileblob";

            var result = await blobStorage.CreateBlockBlobAsync(fileBlob, "SampleData.txt");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetBlobReferenceAsyncSucceeds()
        {
            const string stringBlob = "stringblob";
            await blobStorage.CreateBlockBlobAsync(stringBlob, "text/plain", "hello world");

            var result = blobStorage.GetBlockBlobReference(stringBlob);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetBlobContentsAsStreamAsyncSucceeds()
        {
            const string stringBlob = "streamblob";
            const string originalValue = "Hello World";
            await blobStorage.CreateBlockBlobAsync(stringBlob, "text/plain", originalValue);
            var stream = await blobStorage.GetBlockBlobDataAsStreamAsync(stringBlob);
            string result;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.True(string.Equals(result, originalValue, StringComparison.CurrentCultureIgnoreCase));
        }

        [Fact]
        public async Task GetBlobContentsAsStringAsyncSucceeds()
        {
            const string stringBlob = "stringblob";
            const string originalValue = "Hello World";
            await blobStorage.CreateBlockBlobAsync(stringBlob, "text/plain", originalValue);
            var result = await blobStorage.GetBlockBlobDataAsStringAsync(stringBlob);

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.True(string.Equals(result, originalValue, StringComparison.CurrentCultureIgnoreCase));
        }

        [Fact]
        public async Task GetBlobsInContainerAsyncSucceeds()
        {
            const string stringBlob1 = "blob1";
            const string stringBlob2 = "blob2";
            const string originalValue = "Hello World";
            await blobStorage.CreateBlockBlobAsync(stringBlob1, "text/plain", originalValue);
            await blobStorage.CreateBlockBlobAsync(stringBlob2, "text/plain", originalValue);
            var result = blobStorage.ListBlobsInContainer(ContainerName).ToList();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Count == 2);
        }


        [Fact]
        public async Task DeleteBlobAsyncSucceeds()
        {
            const string blobToDelete = "blobtodelete";
            const string content = "Hello World";
            await blobStorage.CreateBlockBlobAsync(blobToDelete, "text/plain", content);
            var createResult = blobStorage.GetBlockBlobReference(blobToDelete);

            Assert.NotNull(createResult);

            await blobStorage.DeleteBlobAsync(blobToDelete);
            var deleteResult = blobStorage.ListBlobsInContainer(blobToDelete).ToList();

            Assert.True(deleteResult.Count == 0);
        }
    }
}

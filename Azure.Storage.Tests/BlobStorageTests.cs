using System;
using System.IO;
using System.Linq;
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
            // disable this when running outside the emulator
            blobStorage.DeleteBlobContainer();
            // Dispose anything else that needs to be cleand out
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

        [Fact]
        public void CreateBlobFromStringSucceeds()
        {
            const string stringBlob = "stringblob";
            var result = blobStorage.CreateBlockBlob(stringBlob, "text/plain", "hello world");

            Assert.NotNull(result);
        }

        [Fact]
        public void CreateBlobFromFileSucceeds()
        {
            const string fileBlob = "fileblob";

            var result = blobStorage.CreateBlockBlob(fileBlob, "SampleData.txt");

            Assert.NotNull(result);
        }

        [Fact]
        public void GetBlobReferenceSucceeds()
        {
            const string stringBlob = "stringblob";
            blobStorage.CreateBlockBlob(stringBlob, "text/plain", "hello world");

            var result = blobStorage.GetBlockBlobReference(stringBlob);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetBlobContentsAsStreamSucceeds()
        {
            const string stringBlob = "streamblob";
            const string originalValue = "Hello World";
            blobStorage.CreateBlockBlob(stringBlob, "text/plain", originalValue);
            var stream = blobStorage.GetBlockBlobDataAsStream(stringBlob);
            string result;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.True(string.Equals(result, originalValue, StringComparison.CurrentCultureIgnoreCase));
        }

        [Fact]
        public void GetBlobContentsAsStringSucceeds()
        {
            const string stringBlob = "stringblob";
            const string originalValue = "Hello World";
            blobStorage.CreateBlockBlob(stringBlob, "text/plain", originalValue);
            var result = blobStorage.GetBlockBlobDataAsString(stringBlob);

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.True(string.Equals(result, originalValue, StringComparison.CurrentCultureIgnoreCase));
        }

        [Fact]
        public void GetBlobsInContainerSucceeds()
        {
            const string stringBlob1 = "blob1";
            const string stringBlob2 = "blob2";
            const string originalValue = "Hello World";
            blobStorage.CreateBlockBlob(stringBlob1, "text/plain", originalValue);
            blobStorage.CreateBlockBlob(stringBlob2, "text/plain", originalValue);
            var result = blobStorage.ListBlobsInContainer().ToList();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Count > 0);
        }


        [Fact]
        public void DeleteBlobSucceeds()
        {
            const string blobToDelete = "blobtodelete";
            const string content = "Hello World";
            blobStorage.CreateBlockBlob(blobToDelete, "text/plain", content);
            var createResult = blobStorage.GetBlockBlobReference(blobToDelete);

            Assert.NotNull(createResult);

            blobStorage.DeleteBlob(blobToDelete);
            var deleteResult = blobStorage.ListBlobsInContainer().ToList();
            var deletedBlob = deleteResult.FirstOrDefault(u => u.Uri.AbsolutePath.Contains(blobToDelete));
            Assert.True(deletedBlob == null);
        }
    }
}

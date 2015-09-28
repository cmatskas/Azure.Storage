using System.Threading.Tasks;
using Azure.Storage.Interfaces;
using Microsoft.WindowsAzure.Storage.Blob;
using Xunit;

namespace Azure.Storage.Tests
{
    public class BlobDataManagerTests
    {
        private const string ContainerName = "test-container";
        private const string ConnectionString = "UseDevelopmentStorage=true";
        private IBlobDataManager blobDataManager;

        [Fact]
        public async Task UploadLocalFileShouldSucceed()
        {
            blobDataManager = new BlobDataManager();
            var options = new BlobOptions
            {
                BlobName = "testBlob",
                BlobType = BlobType.BlockBlob,
                ConnectionString = ConnectionString,
                ContainerName = ContainerName,
                OverwriteDestination = true,
                Public = true
            };

            await blobDataManager.UploadFileToBlobAsync("SampleData.txt", options);

            Assert.True(blobDataManager.ProgressRecorder.LatestNumberOfFilesFailed == 0);
            Assert.True(blobDataManager.ProgressRecorder.LatestNumberOfFilesTransferred == 1);
        }
    }
}

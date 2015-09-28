using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Interfaces;
using Microsoft.WindowsAzure.Storage.DataMovement;

namespace Azure.Storage
{
    public class BlobDataManager : IBlobDataManager
    {
        public ProgressRecorder ProgressRecorder { get; set; }  

        public async Task UploadFileToBlobAsync(string sourceFilePath, BlobOptions options)
        {
            Validate.BlobContainerName(options.ContainerName, "containerName");
            Validate.BlobName(options.BlobName, "blobName");

            var destinationBlob = DataManagerUtility.GetCloudBlob(options);
            TransferManager.Configurations.ParallelOperations = 64;

            var context = new TransferContext
            {
                OverwriteCallback = (path, destinationPath) => options.OverwriteDestination
            };

            ProgressRecorder = new ProgressRecorder();
            context.ProgressHandler = ProgressRecorder;

            await TransferManager.UploadAsync(sourceFilePath, destinationBlob, null, context, CancellationToken.None);
        }
    }
}

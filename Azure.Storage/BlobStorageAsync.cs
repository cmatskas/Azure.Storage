using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Azure.Storage
{
	/// <summary>
	/// Simple helper class for Windows Azure storage blobs
	/// </summary>
    public class BlobStorageAsync : IBlobStorageAsync
	{
		private readonly CloudBlobContainer cloudBlobContainer;

		/// <summary>
		/// Creates a new BlobStorage object
		/// </summary>
		/// <param name="blobContainerName">The name of the blob to be managed</param>
		/// <param name="storageConnectionString">The connection string pointing to the storage account (this can be local or hosted in Windows Azure</param>
		public BlobStorageAsync(string blobContainerName, string storageConnectionString)
		{
			Validate.BlobContainerName(blobContainerName, "blobContainerName");
			Validate.String(storageConnectionString, "storageConnectionString");

			var cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
			var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

			cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
			cloudBlobContainer.CreateIfNotExists();

			var permissions = cloudBlobContainer.GetPermissions();
			permissions.PublicAccess = BlobContainerPublicAccessType.Container;
			cloudBlobContainer.SetPermissions(permissions);
		}

		/// <summary>
		/// Creates a new block blob and populates it from a stream
		/// </summary>
		/// <param name="blobId">The blobId for the block blob</param>
		/// <param name="contentType">The content type for the block blob</param>
		/// <param name="data">The data to store in the block blob</param>
		/// <returns>The URI to the created block blob</returns>
		public async Task<string> CreateBlockBlobAsync(string blobId, string contentType, Stream data)
		{
			Validate.BlobName(blobId, "blobId");
			Validate.String(contentType, "contentType");
			Validate.Null(data, "data");

			var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobId);
			cloudBlockBlob.Properties.ContentType = contentType;
			await cloudBlockBlob.UploadFromStreamAsync(data);

			return cloudBlockBlob.Uri.ToString();
		}

		/// <summary>
		/// Creates a new block blob and populates it from a byte array
		/// </summary>
		/// <param name="blobId">The blobId for the block blob</param>
		/// <param name="contentType">The content type for the block blob</param>
		/// <param name="data">The data to store in the block blob</param>
		/// <returns>The URI to the created block blob</returns>
		public async Task<string> CreateBlockBlobAsync(string blobId, string contentType, byte[] data)
		{
			Validate.BlobName(blobId, "blobId");
			Validate.String(contentType, "contentType");
			Validate.Null(data, "data");

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobId);
			cloudBlockBlob.Properties.ContentType = contentType;
			await cloudBlockBlob.UploadFromByteArrayAsync(data, 0, data.Length);

			return cloudBlockBlob.Uri.ToString();
		}

        /// <summary>
        /// Creates a new block blob and populates it from a string
        /// </summary>
        /// <param name="blobId">The blobId for the block blob</param>
        /// <param name="contentType">The content type for the block blob</param>
        /// <param name="data">The data to store in the block blob</param>
        /// <returns>The URI to the created block blob</returns>
        public async Task<string> CreateBlockBlobAsync(string blobId, string contentType, string data)
        {
            Validate.BlobName(blobId, "blobId");
            Validate.String(contentType, "contentType");
            Validate.String(data, "data");

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobId);
            cloudBlockBlob.Properties.ContentType = contentType;
            await cloudBlockBlob.UploadTextAsync(data);

            return cloudBlockBlob.Uri.ToString();
        }

	    /// <summary>
	    /// Creates a new block blob and populates it from a file
	    /// </summary>
	    /// <param name="blobId">The blobId for the block blob</param>
	    /// <param name="filePath"></param>
	    /// <returns>The URI to the created block blob</returns>
	    public async Task<string> CreateBlockBlobAsync(string blobId, string filePath)
	    {
            Validate.BlobName(blobId, "blobId");
            Validate.String(filePath, "contentType");

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobId);
            const FileMode fileMode = FileMode.OpenOrCreate;
            await cloudBlockBlob.UploadFromFileAsync(filePath, fileMode);

            return cloudBlockBlob.Uri.ToString();
	    }

	    /// <summary>
	    /// Gets a reference to a block blob with the given unique blob name
	    /// </summary>
	    /// <param name="blobId">The unique block blob identifier</param>
	    /// <returns>A reference to the block blob</returns>
	    public CloudBlockBlob GetBlockBlobReference(string blobId)
		{
			Validate.BlobName(blobId, "blobId");

			return cloudBlobContainer.GetBlockBlobReference(blobId);
		}

        /// <summary>
        /// Returns as stream with the contents of a block blob
        /// with the given blob name
        /// </summary>
        /// <param name="blobId"></param>
        /// <returns>Stream</returns>
	    public async Task<Stream> GetBlockBlobDataAsStreamAsync(string blobId)
	    {
            Validate.BlobName(blobId, "blobId");

            var blob = cloudBlobContainer.GetBlockBlobReference(blobId);
            var stream = new MemoryStream();
            await blob.DownloadToStreamAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
	    }

        /// <summary>
        /// Returns as string with the contents of a block blob
        /// with the given blob name
        /// </summary>
        /// <param name="blobId"></param>
        /// <returns>string</returns>
	    public async Task<string> GetBlockBlobDataAsStringAsync(string blobId)
	    {
            Validate.BlobName(blobId, "blobId");

            var blob = cloudBlobContainer.GetBlockBlobReference(blobId);
            return await blob.DownloadTextAsync();
	    }

	    /// <summary>
	    /// Returns a list of all the blobs in a container
	    /// </summary>
	    /// <param name="containerName"></param>
	    /// <returns></returns>
        public IEnumerable<IListBlobItem> ListBlobsInContainer(string containerName)
        {
            Validate.BlobContainerName(containerName, "containerName");
            return cloudBlobContainer.ListBlobs(null, true).ToList();
        } 

		/// <summary>
		/// Deletes the blob container
		/// </summary>
        public async Task DeleteBlobContainerAsync()
        {
            await cloudBlobContainer.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Deletes the block blob with the given unique blob name
        /// </summary>
        /// <param name="blobId">The unique block blob identifier</param>
        public async Task DeleteBlobAsync(string blobId)
        {
            var blob = cloudBlobContainer.GetBlockBlobReference(blobId);
            await blob.DeleteIfExistsAsync();
        }
	}
}
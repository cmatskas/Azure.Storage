using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace AzureStorageHelpers
{
	/// <summary>
	/// Simple helper class for Windows Azure storage blobs
	/// </summary>
	public class BlobStorage
	{
		private string blobName;
		private CloudBlobClient cloudBlobClient;

		/// <summary>
		/// Creates a new BlobStorage object
		/// </summary>
		/// <param name="blobStorageName">The name of the blob to be managed</param>
		/// <param name="storageConnectionString">The connection string pointing to the storage account (this can be local or hosted in Windows Azure</param>
		public BlobStorage(string blobStorageName, string storageConnectionString)
		{
			blobName = blobStorageName;

			CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);

			cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

			CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(blobName);
			cloudBlobContainer.CreateIfNotExist();

			var permissions = cloudBlobContainer.GetPermissions();
			permissions.PublicAccess = BlobContainerPublicAccessType.Container;
			cloudBlobContainer.SetPermissions(permissions);
		}

		/// <summary>
		/// Creates a new block blob
		/// </summary>
		/// <param name="fileName">The filename for the block blob</param>
		/// <param name="fileContentType">The content type for the block blob</param>
		/// <param name="fileContent">The data to store in the block blob</param>
		/// <returns>The URI to the created block blob</returns>
		public string CreateBlockBlob(string fileName, string fileContentType, Stream fileContent)
		{
			CloudBlockBlob cloudBlockBlob = cloudBlobClient.GetBlockBlobReference(blobName + "/" + fileName);
			cloudBlockBlob.Properties.ContentType = fileContentType;
			cloudBlockBlob.UploadFromStream(fileContent);

			return cloudBlockBlob.Uri.ToString();
		}

		/// <summary>
		/// Creates a new block blob
		/// </summary>
		/// <param name="fileName">The filename for the block blob</param>
		/// <param name="fileContentType">The content type for the block blob</param>
		/// <param name="fileContent">The data to store in the block blob</param>
		/// <returns>The URI to the created block blob</returns>
		public string CreateBlockBlob(string fileName, string fileContentType, byte[] fileContent)
		{
			CloudBlockBlob cloudBlockBlob = cloudBlobClient.GetBlockBlobReference(blobName + "/" + fileName);
			cloudBlockBlob.Properties.ContentType = fileContentType;
			cloudBlockBlob.UploadByteArray(fileContent);

			return cloudBlockBlob.Uri.ToString();
		}

		/// <summary>
		/// Creates a new block blob
		/// </summary>
		/// <param name="fileName">The filename for the block blob</param>
		/// <param name="fileContentType">The content type for the block blob</param>
		/// <returns>A reference to the block blob so data can be written to externally</returns>
		public CloudBlockBlob CreateBlockBlob(string fileName, string fileContentType)
		{
			CloudBlockBlob cloudBlockBlob = cloudBlobClient.GetBlockBlobReference(blobName + "/" + fileName);
			cloudBlockBlob.Properties.ContentType = fileContentType;

			return cloudBlockBlob;
		}

		/// <summary>
		/// Gets a reference to a block blob with the given unique blob name
		/// </summary>
		/// <param name="uniqueBlobName">The unique block blob identifier</param>
		/// <returns>A reference to the block blob</returns>
		public CloudBlockBlob GetBlockBlob(string uniqueBlobName)
		{
			if (uniqueBlobName == null || uniqueBlobName.Length <= 0)
				return null;

			return cloudBlobClient.GetBlockBlobReference(uniqueBlobName);
		}

		/// <summary>
		/// Deletes the block blob with the given unique blob name
		/// </summary>
		/// <param name="uniqueBlobName">The unique block blob identifier</param>
		public void DeleteBlockBlob(string uniqueBlobName)
		{
			if (uniqueBlobName == null || uniqueBlobName.Length <= 0)
				return;

			CloudBlockBlob cloudBlockBlob = cloudBlobClient.GetBlockBlobReference(uniqueBlobName);
			if (cloudBlockBlob != null)
				cloudBlockBlob.DeleteIfExists();
		}
	}
}
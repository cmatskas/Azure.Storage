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
		private string blobContainerName;
		private CloudBlobClient cloudBlobClient;

		/// <summary>
		/// Creates a new BlobStorage object
		/// </summary>
		/// <param name="blobContainerName">The name of the blob to be managed</param>
		/// <param name="storageConnectionString">The connection string pointing to the storage account (this can be local or hosted in Windows Azure</param>
		public BlobStorage(string blobContainerName, string storageConnectionString)
		{
			Validate.BlobContainerName(blobContainerName, "blobContainerName");
			Validate.String(storageConnectionString, "storageConnectionString");

			//http://msdn.microsoft.com/en-us/library/windowsazure/dd135715.aspx

			this.blobContainerName = blobContainerName;

			CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);

			cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

			CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
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
			Validate.BlobName(fileName, "fileName");
			Validate.String(fileContentType, "fileContentType");
			Validate.Null(fileContent, "fileContent");

			CloudBlockBlob cloudBlockBlob = cloudBlobClient.GetBlockBlobReference(blobContainerName + "/" + fileName);
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
			Validate.BlobName(fileName, "fileName");
			Validate.String(fileContentType, "fileContentType");
			Validate.Null(fileContent, "fileContent");

			CloudBlockBlob cloudBlockBlob = cloudBlobClient.GetBlockBlobReference(blobContainerName + "/" + fileName);
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
			Validate.BlobName(fileName, "fileName");
			Validate.String(fileContentType, "fileContentType");

			CloudBlockBlob cloudBlockBlob = cloudBlobClient.GetBlockBlobReference(blobContainerName + "/" + fileName);
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
			Validate.BlobName(uniqueBlobName, "uniqueBlobName");

			return cloudBlobClient.GetBlockBlobReference(uniqueBlobName);
		}

		/// <summary>
		/// Deletes the block blob with the given unique blob name
		/// </summary>
		/// <param name="uniqueBlobName">The unique block blob identifier</param>
		public void DeleteBlockBlob(string uniqueBlobName)
		{
			Validate.BlobName(uniqueBlobName, "uniqueBlobName");

			CloudBlockBlob cloudBlockBlob = cloudBlobClient.GetBlockBlobReference(uniqueBlobName);
			if (cloudBlockBlob != null)
				cloudBlockBlob.DeleteIfExists();
		}
	}
}
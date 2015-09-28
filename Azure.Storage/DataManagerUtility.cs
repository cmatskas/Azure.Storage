using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;

namespace Azure.Storage
{
    /// <summary>
    /// A helper class to support common Azure Data Manager operations
    /// </summary>
    public class DataManagerUtility
    {
        private static CloudStorageAccount storageAccount;
        private static CloudBlobClient blobClient;
        private static CloudFileClient fileClient;

        /// <summary>
        /// Get a CloudBlob instance with the specified name and type in the given container.
        /// </summary>
        /// <param name="options">BlobOptions</param>
        /// <returns>A CloudBlob instance with the specified name and type in the given container.</returns>
        public static CloudBlob GetCloudBlob(BlobOptions options)
        {
            var client = GetCloudBlobClient(options.ConnectionString);
            var container = client.GetContainerReference(options.ContainerName);
            container.CreateIfNotExists();

            CloudBlob cloudBlob;
            switch (options.BlobType)
            {
                case BlobType.AppendBlob:
                    cloudBlob = container.GetAppendBlobReference(options.BlobName);
                    break;
                case BlobType.BlockBlob:
                    cloudBlob = container.GetBlockBlobReference(options.BlobName);
                    break;
                case BlobType.PageBlob:
                    cloudBlob = container.GetPageBlobReference(options.BlobName);
                    break;
                default:
                    throw new ArgumentException($"Invalid blob type {options.BlobName}");
            }

            if (options.Public)
            {
                var permission = container.GetPermissions();

                permission.PublicAccess = BlobContainerPublicAccessType.Container;
                container.SetPermissions(permission);
            }

            return cloudBlob;
        }

        /// <summary>
        /// Get a CloudFile instance with the specified name in the given share.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="shareName">Share name.</param>
        /// <param name="fileName">File name.</param>
        /// <returns>A CloudFile instance with the specified name in the given share.</returns>
        public static CloudFile GetCloudFile(string connectionString, string shareName, string fileName)
        {
            var client = GetCloudFileClient(connectionString);
            var share = client.GetShareReference(shareName);
            share.CreateIfNotExists();

            var rootDirectory = share.GetRootDirectoryReference();
            return rootDirectory.GetFileReference(fileName);
        }

        private static CloudBlobClient GetCloudBlobClient(string connectionString)
        {
            return blobClient ?? (blobClient = GetStorageAccount(connectionString).CreateCloudBlobClient());
        }

        private static CloudFileClient GetCloudFileClient(string connectionString)
        {
            return fileClient ?? (fileClient = GetStorageAccount(connectionString).CreateCloudFileClient());
        }

        private static CloudStorageAccount GetStorageAccount(string connectionString)
        {
            return storageAccount ?? (storageAccount = CloudStorageAccount.Parse(connectionString));
        }
    }
}

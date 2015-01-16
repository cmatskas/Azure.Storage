using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;

namespace Azure.Storage
{
    public class FileStorage
    {
        private readonly CloudFileShare cloudFileShare;

        public FileStorage(string fileShareName, string storageConnectionString )
        {
            Validate.String(fileShareName, "fileShareName");
            Validate.String(storageConnectionString, "storageConnectionString");

            var cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var fileClient = cloudStorageAccount.CreateCloudFileClient();

            cloudFileShare = fileClient.GetShareReference(fileShareName);
            cloudFileShare.CreateIfNotExists();
        }

        public void CreateDirectory(string directoryName)
        {
            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var newDirectory = rootDirectory.GetDirectoryReference(directoryName);
            newDirectory.CreateIfNotExists();
        }

        public void DeleteDirectory(string directoryName)
        {
            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var newDirectory = rootDirectory.GetDirectoryReference(directoryName);
            newDirectory.DeleteIfExists();
        }

        public void WriteTextToFile(string directoryName, string fileName, string content)
        {
            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var directoryToUpdate = rootDirectory.GetDirectoryReference(directoryName);
            var file = directoryToUpdate.GetFileReference(fileName);
            if (!file.Exists())
            {
                file.Create(long.MaxValue);
            }
                
            file.UploadText(content);
        }

        public void WriteStreamToFile(string directoryName, string fileName, Stream content)
        {
            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var directoryToUpdate = rootDirectory.GetDirectoryReference(directoryName);
            var file = directoryToUpdate.GetFileReference(fileName);
            if (!file.Exists())
            {
                file.Create(long.MaxValue);
            }

            file.UploadFromStream(content);
        }

        public void UploadFile(string directoryName, string fileName, string sourceFilePath, FileMode mode)
        {
            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var directoryToUpdate = rootDirectory.GetDirectoryReference(directoryName);
            var file = directoryToUpdate.GetFileReference(fileName);
            if (!file.Exists())
            {
                file.Create(long.MaxValue);
            }

            file.UploadFromFile(sourceFilePath, mode);
        }
    }
}

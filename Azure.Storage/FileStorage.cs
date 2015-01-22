using System.IO;
using Azure.Storage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;

namespace Azure.Storage
{
    public class FileStorage : IFileStorage
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

        public bool DirectoryExists(string directoryName)
        {
            Validate.String(directoryName, "directoryName");
            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var directoryToTest = rootDirectory.GetDirectoryReference(directoryName);
            
            return directoryToTest.Exists();
        }

        public bool FileExists(string directoryName, string fileName)
        {
            Validate.String(directoryName, "directoryName");
            Validate.String(fileName, "fileName");

            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var directoryToUpdate = rootDirectory.GetDirectoryReference(directoryName);
            var file = directoryToUpdate.GetFileReference(fileName);

            return file.Exists();
        }

        public void CreateDirectory(string directoryName)
        {
            Validate.String(directoryName, "directoryName");

            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var newDirectory = rootDirectory.GetDirectoryReference(directoryName);
            newDirectory.CreateIfNotExists();
        }

        public void DeleteDirectory(string directoryName)
        {
            Validate.String(directoryName, "directoryName");

            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var newDirectory = rootDirectory.GetDirectoryReference(directoryName);
            newDirectory.DeleteIfExists();
        }

        public void WriteTextToFile(string directoryName, string fileName, string content)
        {
            Validate.String(directoryName, "directoryName");
            Validate.String(fileName, "fileName");
            Validate.String(content, "content");

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
            Validate.String(directoryName, "directoryName");
            Validate.String(fileName, "fileName");
            Validate.Null(content, "content");

            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var directoryToUpdate = rootDirectory.GetDirectoryReference(directoryName);
            var file = directoryToUpdate.GetFileReference(fileName);
            if (!file.Exists())
            {
                file.Create(long.MaxValue);
            }

            file.UploadFromStream(content);
        }

        public void UploadFile(string directoryName, string fileName, string sourceFilePath)
        {
            Validate.String(directoryName, "directoryName");
            Validate.String(fileName, "fileName");
            Validate.String(sourceFilePath, "sourceFilePath");

            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var directoryToUpdate = rootDirectory.GetDirectoryReference(directoryName);
            var file = directoryToUpdate.GetFileReference(fileName);
            if (!file.Exists())
            {
                file.Create(long.MaxValue);
            }

            const FileMode fileMode = FileMode.OpenOrCreate;

            file.UploadFromFile(sourceFilePath, fileMode);
        }

        public void DeleteFile(string directoryName, string fileName)
        {
            Validate.String(directoryName, "directoryName");
            Validate.String(fileName, "fileName");

            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var directoryToUpdate = rootDirectory.GetDirectoryReference(directoryName);
            var file = directoryToUpdate.GetFileReference(fileName);

            file.DeleteIfExists();
        }
    }
}

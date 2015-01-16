using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;

namespace Azure.Storage
{
    public class FileStorageAsync : IFileStorageAsync
    {
        private readonly CloudFileShare cloudFileShare;

        public FileStorageAsync(string fileShareName, string storageConnectionString )
        {
            Validate.String(fileShareName, "fileShareName");
            Validate.String(storageConnectionString, "storageConnectionString");

            var cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var fileClient = cloudStorageAccount.CreateCloudFileClient();

            cloudFileShare = fileClient.GetShareReference(fileShareName);
            cloudFileShare.CreateIfNotExists();
        }

        public async Task CreateDirectoryAsync(string directoryName)
        {
            Validate.String(directoryName, "directoryName");

            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var newDirectory = rootDirectory.GetDirectoryReference(directoryName);
            await newDirectory.CreateIfNotExistsAsync();
        }

        public async Task DeleteDirectoryAsync(string directoryName)
        {
            Validate.String(directoryName, "directoryName");

            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var newDirectory = rootDirectory.GetDirectoryReference(directoryName);
            await newDirectory.DeleteIfExistsAsync();
        }

        public async Task WriteTextToFileAsync(string directoryName, string fileName, string content)
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
                
            await file.UploadTextAsync(content);
        }

        public async Task WriteStreamToFileAsync(string directoryName, string fileName, Stream content)
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

            await file.UploadFromStreamAsync(content);
        }

        public async Task UploadFileAsync(string directoryName, string fileName, string sourceFilePath)
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

            await file.UploadFromFileAsync(sourceFilePath, fileMode);
        }

        public async Task DeleteFileAsync(string directoryName, string fileName)
        {
            Validate.String(directoryName, "directoryName");
            Validate.String(fileName, "fileName");

            var rootDirectory = cloudFileShare.GetRootDirectoryReference();
            var directoryToUpdate = rootDirectory.GetDirectoryReference(directoryName);
            var file = directoryToUpdate.GetFileReference(fileName);

            await file.DeleteIfExistsAsync();
        }
    }
}

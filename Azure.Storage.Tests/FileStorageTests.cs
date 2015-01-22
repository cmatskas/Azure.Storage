using System.IO;
using System.Text;
using Azure.Storage.Interfaces;
using Xunit;

namespace Azure.Storage.Tests
{
    public class FileStorageAsyncTests
    {
        private const string FileShareName = "test-container";
        private const string ConnectionString = "UseDevelopmentStorage=true";
        private readonly IFileStorage fileStorage;

        public FileStorageAsyncTests()
        {
            fileStorage = new FileStorage(FileShareName, ConnectionString);
        }

        [Fact(Skip="Works only against live storage account only")]
        public void CreateDirectoryShouldSucceed()
        {
            const string directoryName = "testdirectory";
            fileStorage.CreateDirectory(directoryName);

            Assert.True(fileStorage.DirectoryExists(directoryName));
        }

        [Fact(Skip="Works only against live storage account only")]
        public void DeleteDirectoryShouldSucceed()
        {
            const string directoryName = "testdirectorydelete";
            fileStorage.CreateDirectory(directoryName);
            Assert.True(fileStorage.DirectoryExists(directoryName));

            fileStorage.DeleteDirectory(directoryName);
            Assert.False(fileStorage.DirectoryExists(directoryName));
        }

        [Fact(Skip="Works only against live storage account only")]
        public void WriteTextToFileShouldSucceed()
        {
            const string directoryName = "testdirectorytext";
            const string fileName = "testFileName";
            const string content = "hello world";

            fileStorage.WriteTextToFile(directoryName, fileName, content);
            Assert.True(fileStorage.FileExists(directoryName, fileName));
        }

        [Fact(Skip="Works only against live storage account only")]
        public void WriteStreamToFileShouldSucceed()
        {
            const string directoryName = "testdirectorystream";
            const string fileName = "testFileName";
            const string content = "hello world";
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            
            fileStorage.WriteStreamToFile(directoryName, fileName, stream);
            Assert.True(fileStorage.FileExists(directoryName, fileName));
        }

        [Fact(Skip="Works only against live storage account only")]
        public void UploadFileShouldSucceed()
        {
            const string directoryName = "testdirectoryfile";
            const string fileName = "testFileName";
            const string path = "SampleData.txt";

            fileStorage.UploadFile(directoryName, fileName, path);
            Assert.True(fileStorage.FileExists(directoryName, fileName));
        }

        [Fact(Skip = "Works only against live storage account only")]
        public void DeleteFileShouldSucceed()
        {
            const string directoryName = "testdirectorydeletefile";
            const string fileName = "testFileName";
            const string content = "hello world";

            fileStorage.WriteTextToFile(directoryName, fileName, content);
            Assert.True(fileStorage.FileExists(directoryName, fileName));

            fileStorage.DeleteFile(directoryName, fileName);
            Assert.False(fileStorage.FileExists(directoryName, fileName));

        } 
    }
}

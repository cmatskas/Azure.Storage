using System.IO;
using System.Threading.Tasks;

namespace Azure.Storage.Interfaces
{
    public interface IFileStorageAsync
    {
        Task CreateDirectoryAsync(string directoryName);
        Task DeleteDirectoryAsync(string directoryName);
        Task WriteTextToFileAsync(string directoryName, string fileName, string content);
        Task WriteStreamToFileAsync(string directoryName, string fileName, Stream content);
        Task UploadFileAsync(string directoryName, string fileName, string sourceFilePath);
        Task DeleteFileAsync(string directoryName, string fileName);
    }
}

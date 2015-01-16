using System.IO;

namespace Azure.Storage.Interfaces
{
    public interface IFileStorage
    {
        void CreateDirectory(string directoryName);
        void DeleteDirectory(string directoryName);
        void WriteTextToFile(string directoryName, string fileName, string content);
        void WriteStreamToFile(string directoryName, string fileName, Stream content);
        void UploadFile(string directoryName, string fileName, string sourceFilePath);
        void DeleteFile(string directoryName, string fileName);
    }
}

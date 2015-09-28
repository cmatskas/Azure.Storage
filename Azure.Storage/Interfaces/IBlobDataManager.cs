using System.Threading.Tasks;

namespace Azure.Storage.Interfaces
{
    public interface IBlobDataManager
    {
        ProgressRecorder ProgressRecorder { get; set; }
        Task UploadFileToBlobAsync(string sourceFilePath, BlobOptions options);

    }
}
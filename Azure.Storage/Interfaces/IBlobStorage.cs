using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Azure.Storage.Interfaces
{
    public interface IBlobStorage
    {
        string CreateBlockBlob(string blobId, string contentType, Stream data);
        string CreateBlockBlob(string blobId, string contentType, byte[] data);
        string CreateBlockBlob(string blobId, string contentType, string data);
        string CreateBlockBlob(string blobId, string filePath);
        string AddDataToAppendBlockBlob(string blobId, string data);
        CloudBlockBlob GetBlockBlobReference(string blobId);
        CloudAppendBlob GetAppendBlockBlobReference(string blobId);
        Stream GetBlockBlobDataAsStream(string blobId);
        string GetBlockBlobDataAsString(string blobId);
        IEnumerable<IListBlobItem> ListBlobsInContainer();
        void DeleteBlobContainer();
        void DeleteBlob(string blobId);
    }
}

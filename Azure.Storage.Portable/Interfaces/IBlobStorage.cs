using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Azure.Storage.Portable.Interfaces
{
    public interface IBlobStorage
    {
        HttpResponseMessage GetContainer();
        HttpResponseMessage ChangeContainerAccess(bool makePublic = true);
        HttpResponseMessage CreateBlockBlob(string blobName, Stream data);
        HttpResponseMessage CreateBlockBlob(string blobName, byte[] data);
        HttpResponseMessage CreateBlockBlob(string blobId, string data);
        Stream GetBlockBlobDataAsStream(string blobName);
        string GetBlockBlobDataAsString(string blobName);
        IEnumerable<string> ListBlobsInContainer();
        HttpResponseMessage DeleteBlobContainer();
        HttpResponseMessage DeleteBlob(string blobId);
    }
}

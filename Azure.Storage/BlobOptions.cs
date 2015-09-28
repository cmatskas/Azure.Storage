using Microsoft.WindowsAzure.Storage.Blob;

namespace Azure.Storage
{
    public class BlobOptions
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string BlobName { get; set; }
        public BlobType BlobType { get; set; }
        public bool OverwriteDestination { get; set; }
        public bool Public { get; set; }
    }
}

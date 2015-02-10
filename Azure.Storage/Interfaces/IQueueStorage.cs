using Microsoft.WindowsAzure.Storage.Queue;

namespace Azure.Storage.Interfaces
{
    public interface IQueueStorage
    {
        void EnQueue(byte[] content);
        CloudQueueMessage Peek();
        void EnQueue(string content);
        CloudQueueMessage DeQueue();
        void UpdateQueueMessage(string content);
        void DeleteMessage(CloudQueueMessage cloudQueueMessage);
        void DeleteQueue();
        void ClearQueue();
        int MessageCount();
    }
}

using System.Net.Http;

namespace Azure.Storage.Portable.Interfaces
{
    public interface IQueueStorage
    {
        HttpResponseMessage EnQueue(string message, int visibilityTimeout = int.MinValue, int messagettl = int.MinValue, int timeout = 30);
        HttpResponseMessage Peek(int numberOfMessages, int timeout = 30);
        HttpResponseMessage DeQueue(int numberOfMessages, int visibilityTimeout = int.MinValue, int timeout = 30);
        HttpResponseMessage DeQueueAndDelete(int numberOfMessage, int visibilityTimeout = int.MinValue, int timeout = 30);
        HttpResponseMessage UpdateQueueMessage(string message, string popreceipt, int visibilityTimeout = int.MinValue, int timeout = 30);
        HttpResponseMessage DeleteMessage(string popreceipt, int timeout = 30 );
        HttpResponseMessage ClearQueue(int timeout = 30);
    }
}

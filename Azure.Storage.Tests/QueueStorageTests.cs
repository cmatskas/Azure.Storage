using System;
using System.Text;
using Azure.Storage.Interfaces;
using Xunit;

namespace Azure.Storage.Tests
{
    public class QueueStorageTests : IDisposable
    {
        private const string QueueName = "test-queue";
        private const string ConnectionString = "UseDevelopmentStorage=true";
        private readonly IQueueStorage queueStorage;

        public QueueStorageTests()
        {
            queueStorage = new QueueStorage(QueueName, ConnectionString);
        }

        public void Dispose()
        {
            // disable this when running outside the emulator
            queueStorage.DeleteQueue();
            // Dispose anything else that needs to be cleand out
        }

        [Fact]
        public void QueueStringMessageSucceeds()
        {
            const string stringMessage = "string message";
            queueStorage.EnQueue(stringMessage);

            var message = queueStorage.Peek();
            Assert.NotNull(message);
        }

        [Fact]
        public void QueueByteMessageSucceeds()
        {
            const string byteMessage = "byte message";
            var bytes = Encoding.UTF8.GetBytes(byteMessage);
            queueStorage.EnQueue(bytes);

            var message = queueStorage.Peek();
            Assert.NotNull(message);
        }

        [Fact]
        public void DeQueueSucceeds()
        {
            const string stringMessage = "Something queued";
            queueStorage.EnQueue(stringMessage);

            var message = queueStorage.DeQueue();

            Assert.NotNull(message);
        }

        [Fact]
        public void UpdateMessageInQueueSucceeds()
        {
            const string originalMessage = "first message";
            const string secondMessage = "second queued";

            queueStorage.EnQueue(originalMessage);
            queueStorage.UpdateQueueMessage(secondMessage);

            var message = queueStorage.Peek();

            Assert.NotNull(message);
            Assert.True(message.AsString == secondMessage);
        }

        [Fact]
        public void DeleteMessageFromQueueSucceeds()
        {
            const string stringMessage = "hello world";
            queueStorage.ClearQueue();
            queueStorage.EnQueue(stringMessage);
            var messageToDelete = queueStorage.DeQueue();
            queueStorage.DeleteMessage(messageToDelete);

            var deletedMessage = queueStorage.DeQueue();
            if (deletedMessage != null)
            {
                Assert.False(deletedMessage.AsString == stringMessage);
                return;
            }
            
            Assert.Null(deletedMessage);
        }

        [Fact]
        public void ClearAllMessagesFromQueueSucceeds()
        {
            queueStorage.ClearQueue();
            Assert.True(queueStorage.MessageCount() == 0);
        }
    }
}

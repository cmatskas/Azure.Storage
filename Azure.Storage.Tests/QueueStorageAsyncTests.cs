using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Interfaces;
using Xunit;

namespace Azure.Storage.Tests
{
    public class QueueStorageAsyncTests : IDisposable
    {
        private const string QueueName = "test-queue";
        private const string ConnectionString = "UseDevelopmentStorage=true";
        private readonly IQueueStorageAsync queueStorage;

        public QueueStorageAsyncTests()
        {
            queueStorage = new QueueStorageAsync(QueueName, ConnectionString);
        }

        public void Dispose()
        {
            // disable this when running outside the emulator
             queueStorage.DeleteQueueAsync().Wait();
            // Dispose anything else that needs to be cleand out
        }

        [Fact]
        public async Task QueueStringMessageAsyncSucceeds()
        {
            const string stringMessage = "string message";
            await queueStorage.EnQueueAsync(stringMessage);

            var message = await queueStorage.PeekAsync();
            Assert.NotNull(message);
        }

        [Fact]
        public async Task QueueByteMessageSucceeds()
        {
            const string byteMessage = "byte message";
            var bytes = Encoding.UTF8.GetBytes(byteMessage);
            await queueStorage.EnQueueAsync(bytes);

            var message = await queueStorage.PeekAsync();
            Assert.NotNull(message);
        }

        [Fact]
        public async Task DeQueueSucceeds()
        {
            const string stringMessage = "Something queued";
            await queueStorage.EnQueueAsync(stringMessage);

            var message = await queueStorage.DeQueueAsync();

            Assert.NotNull(message);
        }

        [Fact]
        public async Task UpdateMessageInQueueSucceeds()
        {
            const string originalMessage = "first message";
            const string secondMessage = "second queued";

            await queueStorage.ClearQueueAsync();
            await queueStorage.EnQueueAsync(originalMessage);
            await queueStorage.UpdateQueueMessageAsync(secondMessage);

            var message = await queueStorage.PeekAsync();

            Assert.NotNull(message);
            Assert.True(message.AsString == secondMessage);
        }

        [Fact]
        public async Task DeleteMessageFromQueueSucceeds()
        {
            const string stringMessage = "hello world";
            await queueStorage.ClearQueueAsync();
            await queueStorage.EnQueueAsync(stringMessage);
            var messageToDelete = await queueStorage.DeQueueAsync();
            await queueStorage.DeleteMessageAsync(messageToDelete);

            var deletedMessage = await queueStorage.DeQueueAsync();
            if (deletedMessage != null)
            {
                Assert.False(deletedMessage.AsString == stringMessage);
                return;
            }
            
            Assert.Null(deletedMessage);
        }

        [Fact]
        public async Task ClearAllMessagesFromQueueSucceeds()
        {
            await queueStorage.ClearQueueAsync();
            Assert.True(queueStorage.MessageCount() == 0);
        }
    }
}

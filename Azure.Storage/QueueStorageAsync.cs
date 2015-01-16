using System;
using System.Threading.Tasks;
using Azure.Storage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Azure.Storage
{
	/// <summary>
	/// Simple helper class for Windows Azure storage queues
	/// </summary>
	public class QueueStorageAsync : IQueueStorageAsync
	{
		private readonly CloudQueue cloudQueue;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="queueName">The name of the queue to be managed</param>
		/// <param name="storageConnectionString">The connection string pointing to the storage account (this can be local or hosted in Windows Azure</param>
		public QueueStorageAsync(string queueName, string storageConnectionString)
		{
			Validate.QueueName(queueName, "queueName");
			Validate.String(storageConnectionString, "storageConnectionString");

			var cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);

			var cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
			cloudQueue = cloudQueueClient.GetQueueReference(queueName);
			cloudQueue.CreateIfNotExists();
		}

		/// <summary>
		/// Creates a new queue message with the given content and adds it to the queue
		/// </summary>
		/// <param name="content">The content to add to the queue message</param>
		public async Task EnQueueAsync(byte[] content)
		{
			Validate.Null(content, "content");

			var cloudQueueMessage = new CloudQueueMessage(content);

			await cloudQueue.AddMessageAsync(cloudQueueMessage);
		}

        /// <summary>
        /// Peek at the message in front of the queue without removing it
        /// </summary>
        /// <returns>CloudQueueMessage</returns>
	    public async Task<CloudQueueMessage> PeekAsync()
	    {
	        return await cloudQueue.PeekMessageAsync();
	    }

		/// <summary>
		/// Creates a new queue message with the given content and adds it to the queue
		/// </summary>
		/// <param name="content">The content to add to the queue message</param>
		public async Task EnQueueAsync(string content)
		{
			Validate.String(content, "content");

			var cloudQueueMessage = new CloudQueueMessage(content);

			await cloudQueue.AddMessageAsync(cloudQueueMessage);
		}

		/// <summary>
		/// Returns the next item on the queue. Note that this will not delete the message from the queue.
		/// </summary>
		/// <returns>The queue message</returns>
		public async Task<CloudQueueMessage> DeQueueAsync()
		{
			return await cloudQueue.GetMessageAsync();
		}

        /// <summary>
        /// change the contents of a message in-place in the queue
        /// </summary>
        /// <param name="content"></param>
	    public async Task UpdateQueueMessageAsync(string content)
	    {
            var message = cloudQueue.GetMessage();
            message.SetMessageContent(content);
            await cloudQueue.UpdateMessageAsync(message,
                TimeSpan.FromSeconds(0.0),
                MessageUpdateFields.Content | MessageUpdateFields.Visibility);
	    }

		/// <summary>
		/// Deletes the given queue message from the queue
		/// </summary>
		/// <param name="cloudQueueMessage">The queue message to delete</param>
		public async Task DeleteMessageAsync(CloudQueueMessage cloudQueueMessage)
		{
			Validate.Null(cloudQueueMessage, "cloudQueueMessage");

			await cloudQueue.DeleteMessageAsync(cloudQueueMessage);
		}
	}
}

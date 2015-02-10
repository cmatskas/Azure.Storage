using System;
using Azure.Storage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Azure.Storage
{
	/// <summary>
	/// Simple helper class for Windows Azure storage queues
	/// </summary>
	public class QueueStorage : IQueueStorage
	{
		private readonly CloudQueue cloudQueue;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="queueName">The name of the queue to be managed</param>
		/// <param name="storageConnectionString">The connection string pointing to the storage account (this can be local or hosted in Windows Azure</param>
		public QueueStorage(string queueName, string storageConnectionString)
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
		public void EnQueue(byte[] content)
		{
			Validate.Null(content, "content");

			var cloudQueueMessage = new CloudQueueMessage(content);

			cloudQueue.AddMessage(cloudQueueMessage);
		}

        /// <summary>
        /// Peek at the message in front of the queue without removing it
        /// </summary>
        /// <returns>CloudQueueMessage</returns>
	    public CloudQueueMessage Peek()
	    {
	        return cloudQueue.PeekMessage();
	    }

		/// <summary>
		/// Creates a new queue message with the given content and adds it to the queue
		/// </summary>
		/// <param name="content">The content to add to the queue message</param>
		public void EnQueue(string content)
		{
			Validate.String(content, "content");

			var cloudQueueMessage = new CloudQueueMessage(content);

			cloudQueue.AddMessage(cloudQueueMessage);
		}

		/// <summary>
		/// Returns the next item on the queue. Note that this will not delete the message from the queue.
		/// </summary>
		/// <returns>The queue message</returns>
		public CloudQueueMessage DeQueue()
		{
			return cloudQueue.GetMessage();
		}

        /// <summary>
        /// change the contents of a message in-place in the queue
        /// </summary>
        /// <param name="content"></param>
	    public void UpdateQueueMessage(string content)
	    {
            var message = cloudQueue.GetMessage();
            message.SetMessageContent(content);
            cloudQueue.UpdateMessage(message,
                TimeSpan.FromSeconds(0.0),
                MessageUpdateFields.Content | MessageUpdateFields.Visibility);
	    }

		/// <summary>
		/// Deletes the given queue message from the queue
		/// </summary>
		/// <param name="cloudQueueMessage">The queue message to delete</param>
		public void DeleteMessage(CloudQueueMessage cloudQueueMessage)
		{
			Validate.Null(cloudQueueMessage, "cloudQueueMessage");

			cloudQueue.DeleteMessage(cloudQueueMessage);
		}

        /// <summary>
        /// Deletes the current Queue
        /// </summary>
	    public void DeleteQueue()
	    {
	        cloudQueue.DeleteIfExists();
	    }

        /// <summary>
        /// Clears all messages from the Queue
        /// </summary>
	    public void ClearQueue()
	    {
	        cloudQueue.Clear();
	    }

        /// <summary>
        /// Gets an approximate message count
        /// </summary>
        /// <returns></returns>
	    public int MessageCount()
	    {
            cloudQueue.FetchAttributes();
	        return cloudQueue.ApproximateMessageCount ?? 0;
	    }
	}
}

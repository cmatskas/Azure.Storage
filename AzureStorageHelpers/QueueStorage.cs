using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace AzureStorageHelpers
{
	/// <summary>
	/// Simple helper class for Windows Azure storage queues
	/// </summary>
	public class QueueStorage
	{
		private CloudQueue cloudQueue;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queueName">The name of the queue to be managed</param>
		/// <param name="storageConnectionString">The connection string pointing to the storage account (this can be local or hosted in Windows Azure</param>
		/// <param name="useNagleAlgorithm">PUT HTTP requests that are smaller than 1460 bytes are inefficient with the Nagle algorithm turned on.</param>
		public QueueStorage(string queueName, string storageConnectionString, bool useNagleAlgorithm = false)
		{
			Validate.QueueName(queueName, "queueName");
			Validate.String(storageConnectionString, "storageConnectionString");

			//http://msdn.microsoft.com/en-us/library/windowsazure/dd179349.aspx

			CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);

			CloudQueueClient cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
			cloudQueue = cloudQueueClient.GetQueueReference(queueName);
			cloudQueue.CreateIfNotExist();

			if (!useNagleAlgorithm)
				DisableNagleOnEndpoint(cloudStorageAccount);
		}

		/// <summary>
		/// Creates a new queue message with the given content and adds it to the queue
		/// </summary>
		/// <param name="content">The content to add to the queue message</param>
		public void EnQueue(byte[] content)
		{
			Validate.Null(content, "content");

			CloudQueueMessage cloudQueueMessage = new CloudQueueMessage(content);

			cloudQueue.AddMessage(cloudQueueMessage);
		}

		/// <summary>
		/// Creates a new queue message with the given content and adds it to the queue
		/// </summary>
		/// <param name="content">The content to add to the queue message</param>
		public void EnQueue(string content)
		{
			Validate.String(content, "content");

			CloudQueueMessage cloudQueueMessage = new CloudQueueMessage(content);

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
		/// Deletes the given queue message from the queue
		/// </summary>
		/// <param name="cloudQueueMessage">The queue message to delete</param>
		public void DeleteMessage(CloudQueueMessage cloudQueueMessage)
		{
			Validate.Null(cloudQueueMessage, "cloudQueueMessage");

			cloudQueue.DeleteMessage(cloudQueueMessage);
		}

		/// <summary>
		/// Disables the nagle algorithm on the given storage account
		/// </summary>
		/// <param name="account">The cloud storage account to disable nagle</param>
		private void DisableNagleOnEndpoint(CloudStorageAccount account)
		{
			Validate.Null(account, "account");

			var queueServicePoint = ServicePointManager.FindServicePoint(account.QueueEndpoint);
			queueServicePoint.UseNagleAlgorithm = false;
		}
	}
}

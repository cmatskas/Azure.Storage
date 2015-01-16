using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage
{
	/// <summary>
	/// Simple helper class for Windows Azure storage tables
	/// </summary>
    public class TableStorageAsync<T> : ITableStorageAsync<T> where T : TableEntity, new()
	{
	    private readonly CloudTable cloudTable;

		/// <summary>
		/// Creates a new TableStorage object
		/// </summary>
		/// <param name="tableName">The name of the table to be managed</param>
		/// <param name="storageConnectionString">The connection string pointing to the storage account (this can be local or hosted in Windows Azure</param>
		public TableStorageAsync(string tableName, string storageConnectionString)
		{
			Validate.TableName(tableName, "tableName");
			Validate.String(storageConnectionString, "storageConnectionString");

			var cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);

		    var requestOptions = new TableRequestOptions
		    {
		        RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(1), 3)
		    };
		    
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
		    cloudTableClient.DefaultRequestOptions = requestOptions;

		    cloudTable = cloudTableClient.GetTableReference(tableName);
		    cloudTable.CreateIfNotExists();
		}

		/// <summary>
		/// Creates a new entity in the table
		/// </summary>
		/// <param name="entity">The entity to store in the table</param>
		public async Task CreateEntityAsync(T entity)
		{
			Validate.Null(entity, "entity");
		    var insertOperation = TableOperation.Insert(entity);
		    await cloudTable.ExecuteAsync(insertOperation);
		}

        /// <summary>
        /// Creates new entities in the table using batching
        /// </summary>
        /// <param name="entities">The entities to store in the table</param>
	    public async Task CreateEntitiesAsync(IEnumerable<T> entities)
        {
            Validate.Null(entities, "entities");
            var batchOperation = new TableBatchOperation();

            foreach (var entity in entities)
            {
               batchOperation.Insert(entity); 
            }

            await cloudTable.ExecuteBatchAsync(batchOperation);
        }

        /// <summary>
        /// Create an entity if it doesn't exist or merges the new values
        /// to the existing one
        /// </summary>
        /// <param name="entity"></param>
	    public async Task InsertOrUpdateAsync(T entity)
	    {
            Validate.Null(entity, "entity");
	        var insertOrUpdateOperation = TableOperation.InsertOrMerge(entity);
	        await cloudTable.ExecuteAsync(insertOrUpdateOperation);
	    }

		/// <summary>
		/// Deletes an entities from the table with the specified partitionKey
		/// </summary>
		/// <param name="partitionKey">
		/// The partition key of the entity to be deleted. 
		/// Note that a partition key can return more than one entity. 
		/// If more than one are returned, the first one is deleted.
		/// </param>
		public async Task DeleteEntitiesByPartitionKeyAsync(string partitionKey)
		{
			Validate.TablePropertyValue(partitionKey, "partitionKey");

		    var query =
		        new TableQuery<T>()
                    .Where(TableQuery.GenerateFilterCondition(
                        "PartitionKey", 
                        QueryComparisons.Equal,
		                partitionKey));

		    var results = await cloudTable.ExecuteQueryAsync(query);
		    var batchOperation = new TableBatchOperation();
		    var counter = 0;
		    foreach (var entity in results)
		    {
		        batchOperation.Delete(entity);
		        counter++;

                //Batch operations are limited to 100 items
                //when we reach 100, we commit and clear the operation
                if (counter == 100)
                {
                    await cloudTable.ExecuteBatchAsync(batchOperation);
                    batchOperation = new TableBatchOperation();
                    counter = 0;
                }
		    }
		}

        /// <summary>
        /// Deletes an entities from the table with the specified partitionKey
        /// </summary>
        /// <param name="rowKey">
        /// The row key of the entities to be deleted. 
        /// Note that a row key can return more than one entity. 
        /// If more than one are returned, the first one is deleted.
        /// </param>
        public async Task DeleteEntitiesByRowKeyAsync(string rowKey)
        {
            Validate.TablePropertyValue(rowKey, "rowKey");

            var query =
                new TableQuery<T>()
                    .Where(TableQuery.GenerateFilterCondition(
                        "RowKey",
                        QueryComparisons.Equal,
                        rowKey));

            var results = await cloudTable.ExecuteQueryAsync(query);
            var batchOperation = new TableBatchOperation();
            var counter = 0;
            foreach (var entity in results)
            {
                batchOperation.Delete(entity);
                counter++;

                //Batch operations are limited to 100 items
                //when we reach 100, we commit and clear the operation
                if (counter == 100)
                {
                    await cloudTable.ExecuteBatchAsync(batchOperation);
                    batchOperation = new TableBatchOperation();
                    counter = 0;
                }
            }
        }

	    /// <summary>
	    /// Deletes an entity from the table
	    /// </summary>
	    /// <param name="partitionKey">The partitionKey of the entity</param>
	    /// <param name="rowKey">The row key of the entity to be deleted</param>
	    public async Task DeleteEntityAsync(string partitionKey, string rowKey)
		{
            Validate.TablePropertyValue(rowKey, "rowKey");
            Validate.TablePropertyValue(partitionKey, "partitionKey");

            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var retrievedResult = await cloudTable.ExecuteAsync(retrieveOperation);

            var entityToDelete = retrievedResult.Result as T;
	        if (entityToDelete != null)
	        {
                var deleteOperation = TableOperation.Delete(entityToDelete);
                await cloudTable.ExecuteAsync(deleteOperation);
	        }
		}

		/// <summary>
		/// Gets an entity from the table
		/// </summary>
		/// <param name="partitionKey">
		/// The partition key of the entity to be returned.
		/// </param>
		public async Task<IEnumerable<T>> GetEntitiesByPartitionKeyAsync(string partitionKey)
		{
            Validate.TablePropertyValue(partitionKey, "partitionKey");

            var query =
               new TableQuery<T>()
                   .Where(TableQuery.GenerateFilterCondition(
                       "PartitionKey",
                       QueryComparisons.Equal,
                       partitionKey));

            return await cloudTable.ExecuteQueryAsync(query);
		}

		/// <summary>
		/// Gets all entities from the table with the specified rowKey
		/// </summary>
		/// <param name="rowKey">
		/// The row key of the entities to be returned.
		/// </param>
        public async Task<IEnumerable<T>> GetEntitiesByRowKeyAsync(string rowKey)
		{
			Validate.TablePropertyValue(rowKey, "rowKey");

            var query =
               new TableQuery<T>()
                   .Where(TableQuery.GenerateFilterCondition(
                       "RowKey",
                       QueryComparisons.Equal,
                       rowKey));

            return await cloudTable.ExecuteQueryAsync(query);
		}

		/// <summary>
		/// Gets an entity from the table
		/// </summary>
		/// <param name="partitionKey">
		/// The partition key of the entity to be returned. 
		/// The partition key and row key will always return a single entity.
		/// </param>
		/// <param name="rowKey">
		/// The row key of the entity to be returned.
		/// The partition key and row key will always return a single entity.
		/// </param>
		public async Task<T> GetEntityByPartitionKeyAndRowKeyAsync(string partitionKey, string rowKey)
		{
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            var retrievedResult = await cloudTable.ExecuteAsync(retrieveOperation);

            return retrievedResult.Result as T;
		}
	}
}

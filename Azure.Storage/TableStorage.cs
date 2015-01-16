using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage
{
	/// <summary>
	/// Simple helper class for Windows Azure storage tables
	/// </summary>
	public class TableStorage<T> where T : TableEntity, new()
	{
	    private readonly CloudTable cloudTable;

		/// <summary>
		/// Creates a new TableStorage object
		/// </summary>
		/// <param name="tableName">The name of the table to be managed</param>
		/// <param name="storageConnectionString">The connection string pointing to the storage account (this can be local or hosted in Windows Azure</param>
		public TableStorage(string tableName, string storageConnectionString)
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
		public void CreateEntity(T entity)
		{
			Validate.Null(entity, "entity");
		    var insertOperation = TableOperation.Insert(entity);
		    cloudTable.Execute(insertOperation);
		}

        /// <summary>
        /// Creates new entities in the table using batching
        /// </summary>
        /// <param name="entities">The entities to store in the table</param>
	    public void CreateEntities(List<T> entities)
        {
            Validate.Null(entities, "entities");
            var batchOperation = new TableBatchOperation();

            foreach (var entity in entities)
            {
               batchOperation.Insert(entity); 
            }

            cloudTable.ExecuteBatch(batchOperation);
        }

        /// <summary>
        /// Create an entity if it doesn't exist or merges the new values
        /// to the existing one
        /// </summary>
        /// <param name="entity"></param>
	    public void InsertOrUpdate(T entity)
	    {
            Validate.Null(entity, "entity");
	        var insertOrUpdateOperation = TableOperation.InsertOrMerge(entity);
	        cloudTable.Execute(insertOrUpdateOperation);
	    }

		/// <summary>
		/// Deletes an entities from the table with the specified partitionKey
		/// </summary>
		/// <param name="partitionKey">
		/// The partition key of the entity to be deleted. 
		/// Note that a partition key can return more than one entity. 
		/// If more than one are returned, the first one is deleted.
		/// </param>
		public void DeleteEntitiesByPartitionKey(string partitionKey)
		{
			Validate.TablePropertyValue(partitionKey, "partitionKey");

		    var query =
		        new TableQuery<T>()
                    .Where(TableQuery.GenerateFilterCondition(
                        "PartitionKey", 
                        QueryComparisons.Equal,
		                partitionKey));

		    var results = cloudTable.ExecuteQuery(query);
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
                    cloudTable.ExecuteBatch(batchOperation);
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
        public void DeleteEntitiesByRowKey(string rowKey)
        {
            Validate.TablePropertyValue(rowKey, "rowKey");

            var query =
                new TableQuery<T>()
                    .Where(TableQuery.GenerateFilterCondition(
                        "RowKey",
                        QueryComparisons.Equal,
                        rowKey));

            var results = cloudTable.ExecuteQuery(query);
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
                    cloudTable.ExecuteBatch(batchOperation);
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
	    public void DeleteEntity(string partitionKey, string rowKey)
		{
            Validate.TablePropertyValue(rowKey, "rowKey");
            Validate.TablePropertyValue(partitionKey, "partitionKey");

            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var retrievedResult = cloudTable.Execute(retrieveOperation);

            var entityToDelete = retrievedResult.Result as T;
	        if (entityToDelete != null)
	        {
                var deleteOperation = TableOperation.Delete(entityToDelete);
                cloudTable.Execute(deleteOperation);
	        }
		}

		/// <summary>
		/// Gets an entity from the table
		/// </summary>
		/// <param name="partitionKey">
		/// The partition key of the entity to be returned.
		/// </param>
		public IEnumerable<T> GetEntitiesByPartitionKey(string partitionKey)
		{
            Validate.TablePropertyValue(partitionKey, "partitionKey");

            var query =
               new TableQuery<T>()
                   .Where(TableQuery.GenerateFilterCondition(
                       "PartitionKey",
                       QueryComparisons.Equal,
                       partitionKey));

            return cloudTable.ExecuteQuery(query).AsEnumerable();
		}

		/// <summary>
		/// Gets all entities from the table with the specified rowKey
		/// </summary>
		/// <param name="rowKey">
		/// The row key of the entities to be returned.
		/// </param>
        public IEnumerable<T> GetEntitiesByRowKey(string rowKey)
		{
			Validate.TablePropertyValue(rowKey, "rowKey");

            var query =
               new TableQuery<T>()
                   .Where(TableQuery.GenerateFilterCondition(
                       "RowKey",
                       QueryComparisons.Equal,
                       rowKey));

            return cloudTable.ExecuteQuery(query).AsEnumerable();
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
		public T GetEntityByPartitionKeyAndRowKey(string partitionKey, string rowKey)
		{
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            var retrievedResult = cloudTable.Execute(retrieveOperation);

            return retrievedResult.Result as T;
		}
	}
}

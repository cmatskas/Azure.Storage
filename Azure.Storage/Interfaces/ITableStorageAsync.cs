using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Interfaces
{
    public interface ITableStorageAsync<T> where T : TableEntity, new()
    {
        Task CreateEntityAsync(T entity);
        Task CreateEntitiesAsync(IEnumerable<T> entities);
        Task InsertOrUpdateAsync(T entity);
        Task DeleteEntitiesByPartitionKeyAsync(string partitionKey);
        Task DeleteEntitiesByRowKeyAsync(string rowKey);
        Task DeleteEntityAsync(string partitionKey, string rowKey);
        Task<IEnumerable<T>> GetEntitiesByPartitionKeyAsync(string partitionKey);
        Task<IEnumerable<T>> GetEntitiesByRowKeyAsync(string rowKey);
        Task<T> GetEntityByPartitionKeyAndRowKeyAsync(string partitionKey, string rowKey);
    }
}

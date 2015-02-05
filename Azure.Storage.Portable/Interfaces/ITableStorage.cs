using System.Collections.Generic;

namespace Azure.Storage.Portable.Interfaces
{
    public interface ITableStorage
    {
        void CreateEntity(string entity);
        void CreateEntities(IEnumerable<string> entities);
        void InsertOrUpdate(string entity);
        void DeleteEntitiesByPartitionKey(string partitionKey);
        void DeleteEntitiesByRowKey(string rowKey);
        void DeleteEntity(string partitionKey, string rowKey);
        IEnumerable<string> GetEntitiesByPartitionKey(string partitionKey);
        IEnumerable<string> GetEntitiesByRowKey(string rowKey);
        string GetEntityByPartitionKeyAndRowKey(string partitionKey, string rowKey);
    }
}

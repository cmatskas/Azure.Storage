using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Interfaces;
using Xunit;

namespace Azure.Storage.Tests
{
    public class TableStorageTests: IDisposable
    {
        private const string TableName = "testtable";
        private const string ConnectionString = "UseDevelopmentStorage=true";
        private readonly ITableStorage<TestTableEntity> tableStorage;

        public TableStorageTests()
        {
            tableStorage = new TableStorage<TestTableEntity>(TableName, ConnectionString);
        }
        
        public void Dispose()
        {
            tableStorage.DeleteTable();
        }

        [Fact]
        public void CreateEntityShouldSucceed()
        {
            var testEntity = new TestTableEntity("john1", "smith") {Age = 21, Email = "test@test.com"};
            tableStorage.CreateEntity(testEntity);
            var result = tableStorage.GetEntitiesByRowKey("john1").ToList();
            Assert.True(result.Count > 0);
        }

        [Fact]
        public void CreateEntitiesShouldSucceed()
        {
            var testEntity1 = new TestTableEntity("john", "smith2") { Age = 21, Email = "test@test.com" };
            var testEntity2 = new TestTableEntity("john1", "smith2") { Age = 21, Email = "test@test.com" };
            var entityList = new List<TestTableEntity> {testEntity1, testEntity2};
            tableStorage.CreateEntities(entityList);

            var result = tableStorage.GetEntitiesByPartitionKey("smith2").ToList();
            Assert.True(result.Count > 0);
        }
    }
}

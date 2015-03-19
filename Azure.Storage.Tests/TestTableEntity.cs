using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Tests
{
    internal class TestTableEntity : TableEntity
    {
        public TestTableEntity(string firstName, string lastName)
        {
            PartitionKey = lastName;
            RowKey = firstName;
        }

        public TestTableEntity()
        {
        }

        public int Age { get; set; }

        public string Email { get; set; }
    }
}

using System;
using System.Net;
using System.Net.Http;

namespace Azure.Storage.Portable
{
    public class TableStorage
    {
        private readonly string account;
        private readonly string key;
        private readonly string endpointUrl;
        private readonly string tableName;

        public TableStorage(string tableName, string endpointUrl, string account, string key)
        {
            Validate.TableName(tableName, "tableName");
            Validate.String(endpointUrl, "endpointUrl");
            Validate.String(key, "key");
            Validate.String(account, "account");

            this.account = account;
            this.endpointUrl = endpointUrl;
            this.key = key;
            this.tableName = tableName;

            var getTableResponse = GetTable();
            if (getTableResponse.StatusCode == HttpStatusCode.NotFound)
            {
                CreateTable();
            }
        }

        private HttpResponseMessage CreateTable()
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage GetTable()
        {
            throw new NotImplementedException();
        }
    }
}

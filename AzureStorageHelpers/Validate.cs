using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureStorageHelpers
{
	class Validate
	{
		public static void Null(object paramValue, string paramName)
		{
			if (paramValue == null)
				throw new ArgumentException("Parameter must not be null.", (paramName != null) ? paramName : "");
		}

		public static void String(string paramValue, string paramName)
		{
			Null(paramValue, paramName);
			if (paramValue.Length == 0)
				throw new ArgumentException("Parameter must have length greater than zero.", (paramName != null) ? paramName : "");
		}

		public static void TableName(string paramValue, string paramName)
		{
			String(paramValue, paramName);

			//TODO
		}

		public static void TableParitionKey(string paramValue, string paramName)
		{
			String(paramValue, paramName);

			//TODO
		}

		public static void TableRowKey(string paramValue, string paramName)
		{
			String(paramValue, paramName);

			//TODO
		}

		public static void BlobContainerName(string paramValue, string paramName)
		{
			String(paramValue, paramName);

			//TODO
		}

		public static void BlobName(string paramValue, string paramName)
		{
			String(paramValue, paramName);

			//TODO
		}

		public static void QueueName(string paramValue, string paramName)
		{
			String(paramValue, paramName);

			//TODO
		}
	}
}

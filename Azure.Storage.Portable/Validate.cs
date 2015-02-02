using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Azure.Storage.Portable
{
	public class Validate
	{
		public static void Null(object paramValue, string paramName)
		{
		    if (paramValue == null)
		    {
		        throw new ArgumentException("Parameter must not be null.", paramName ?? "");
		    }
		}

	    public static void Stream(Stream paramValue, string paramName)
	    {
	        if (paramValue == null)
	        {
                throw new ArgumentNullException(paramName ?? "", "Parameter must not be null");
	        }
	    }

	    public static void ByteArray(byte[] paramValue, string paramName)
	    {
	        if (paramValue == null)
	        {
                throw new ArgumentNullException(paramName ?? "", "Parameter must not be null");
	        }
	    }

		public static void String(string paramValue, string paramName)
		{
			Null(paramValue, paramName);
		    if (paramValue.Length == 0)
		    {
		        throw new ArgumentException("Parameter must have length greater than zero.", paramName ?? "");
		    }
		}

		public static void TableName(string paramValue, string paramName)
		{
			Null(paramValue, paramName);

			var regex = new Regex("^[A-Za-z][A-Za-z0-9]{2,62}$");
			if (!regex.IsMatch(paramValue))
			{
				throw new ArgumentException("Table names must conform to these rules: " +
					"May contain only alphanumeric characters. " + 
					"Cannot begin with a numeric character. " + 
					"Are case-insensitive. " + 
					"Must be from 3 to 63 characters long.", paramName ?? "");
			}
		}

		public static void TablePropertyValue(string paramValue, string paramName)
		{
			Null(paramValue, paramName);

			var regex = new Regex(@"^[^/\\#?]{0,1024}$");
			if (!regex.IsMatch(paramValue))
			{
				throw new ArgumentException("Table property values must conform to these rules: " +
					"Must not contain the forward slash (/), backslash (\\), number sign (#), or question mark (?) characters. " +
					"Must be from 1 to 1024 characters long.", paramName ?? "");
			}
		}

		public static void BlobContainerName(string paramValue, string paramName)
		{
			Null(paramValue, paramName);

			var regex = new Regex("^(?-i)(?:[a-z0-9]|(?<=[0-9a-z])-(?=[0-9a-z])){3,63}$");
			if (!regex.IsMatch(paramValue))
			{
				throw new ArgumentException("Blob container names must conform to these rules: " +
					"Must start with a letter or number, and can contain only letters, numbers, and the dash (-) character. " +
					"Every dash (-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted in container names. " +
					"All letters in a container name must be lowercase. " +
					"Must be from 3 to 63 characters long.", paramName ?? "");
			}
		}

		public static void BlobName(string paramValue, string paramName)
		{
			String(paramValue, paramName);

			if (paramValue.Length > 1024)
			{
				throw new ArgumentException("Blob names must conform to these rules: " +
					"Must be from 1 to 1024 characters long.", paramName ?? "");
			}
		}

		public static void QueueName(string paramValue, string paramName)
		{
			Null(paramValue, paramName);

			var regex = new Regex("^(?-i)(?:[a-z0-9]|(?<=[0-9a-z])-(?=[0-9a-z])){3,63}$");
			if (!regex.IsMatch(paramValue))
			{
				throw new ArgumentException("Queue names must conform to these rules: " +
					"Must start with a letter or number, and can contain only letters, numbers, and the dash (-) character. " +
					"The first and last letters in the queue name must be alphanumeric. The dash (-) character cannot be the first or last character. Consecutive dash characters are not permitted in the queue name. " +
					"All letters in a queue name must be lowercase. " +
					"Must be from 3 to 63 characters long.", paramName ?? "");
			}
		}
	}
}

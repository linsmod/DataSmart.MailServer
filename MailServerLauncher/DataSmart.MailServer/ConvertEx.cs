using System;

namespace DataSmart.MailServer
{
	public class ConvertEx
	{
		public static string ToString(object value)
		{
			if (value == null)
			{
				return "";
			}
			return value.ToString();
		}

		public static bool ToBoolean(object value)
		{
			if (value == null)
			{
				return false;
			}
			bool result;
			try
			{
				result = Convert.ToBoolean(value);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool ToBoolean(object value, bool defaultValue)
		{
			bool result;
			try
			{
				result = Convert.ToBoolean(value);
			}
			catch
			{
				result = defaultValue;
			}
			return result;
		}

		public static int ToInt32(object value)
		{
			if (value == null)
			{
				return 0;
			}
			int result;
			try
			{
				result = Convert.ToInt32(value);
			}
			catch
			{
				result = 0;
			}
			return result;
		}
	}
}

using System;

namespace DataSmart.MailServer.Filters
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

		public static int ToInt32(object value)
		{
			return ConvertEx.ToInt32(value, 0);
		}

		public static int ToInt32(object value, int defaultValue)
		{
			if (value == null)
			{
				return defaultValue;
			}
			int result;
			try
			{
				result = Convert.ToInt32(value);
			}
			catch
			{
				result = defaultValue;
			}
			return result;
		}
	}
}

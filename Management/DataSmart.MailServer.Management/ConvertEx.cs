using System;
using System.Net;

namespace DataSmart.MailServer.Management
{
	internal class ConvertEx
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
				if (value == null)
				{
					result = false;
				}
				else
				{
					try
					{
						result = Convert.ToBoolean(value);
					}
					catch
					{
						result = false;
					}
				}
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

		public static IPEndPoint ToIPEndPoint(string value, IPEndPoint defultValue)
		{
			if (value == null)
			{
				return defultValue;
			}
			IPEndPoint result;
			try
			{
				if (value.IndexOf(':') == -1)
				{
					value += ":";
				}
				result = new IPEndPoint(IPAddress.Parse(value.Split(new char[]
				{
					':'
				})[0]), ConvertEx.ToInt32(value.Split(new char[]
				{
					':'
				})[1]));
			}
			catch
			{
				result = defultValue;
			}
			return result;
		}
	}
}

using System;
using System.IO;

namespace DataSmart.MailServer
{
	internal class SCore
	{
		public static bool IsMatch(string pattern, string text)
		{
			if (pattern.IndexOf("*") > -1)
			{
				if (pattern == "*")
				{
					return true;
				}
				if (pattern.StartsWith("*") && pattern.EndsWith("*") && text.IndexOf(pattern.Substring(1, pattern.Length - 2)) > -1)
				{
					return true;
				}
				if (pattern.IndexOf("*") == -1 && text.ToLower() == pattern.ToLower())
				{
					return true;
				}
				if (pattern.StartsWith("*") && text.ToLower().EndsWith(pattern.Substring(1).ToLower()))
				{
					return true;
				}
				if (pattern.EndsWith("*") && text.ToLower().StartsWith(pattern.Substring(0, pattern.Length - 1).ToLower()))
				{
					return true;
				}
			}
			else if (pattern.ToLower() == text.ToLower())
			{
				return true;
			}
			return false;
		}

		public static bool IsAstericMatch(string pattern, string text)
		{
			pattern = pattern.ToLower();
			text = text.ToLower();
			if (pattern == "")
			{
				pattern = "*";
			}
			while (pattern.Length > 0)
			{
				if (pattern.StartsWith("*"))
				{
					if (pattern.IndexOf("*", 1) <= -1)
					{
						return text.EndsWith(pattern.Substring(1));
					}
					string text2 = pattern.Substring(1, pattern.IndexOf("*", 1) - 1);
					if (text.IndexOf(text2) == -1)
					{
						return false;
					}
					text = text.Substring(text.IndexOf(text2) + text2.Length);
					pattern = pattern.Substring(pattern.IndexOf("*", 1));
				}
				else
				{
					if (pattern.IndexOfAny(new char[]
					{
						'*'
					}) <= -1)
					{
						return text == pattern;
					}
					string text3 = pattern.Substring(0, pattern.IndexOfAny(new char[]
					{
						'*'
					}));
					if (!text.StartsWith(text3))
					{
						return false;
					}
					text = text.Substring(text.IndexOf(text3) + text3.Length);
					pattern = pattern.Substring(pattern.IndexOfAny(new char[]
					{
						'*'
					}));
				}
			}
			return true;
		}

		public static string PathFix(string path)
		{
			return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
		}

		public static void StreamCopy(Stream source, Stream destination)
		{
			byte[] array = new byte[8000];
			for (int i = source.Read(array, 0, array.Length); i > 0; i = source.Read(array, 0, array.Length))
			{
				destination.Write(array, 0, i);
			}
		}
	}
}

using System.NetworkToolkit.Log;
using System;
using System.Diagnostics;
using System.IO;

namespace DataSmart.MailServer
{
	internal class Logger
	{
		public static void WriteLog(string fileName, string text)
		{
			try
			{
				fileName = SCore.PathFix(fileName);
				if (!Directory.Exists(Path.GetDirectoryName(fileName)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(fileName));
				}
				using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
				{
					StreamWriter streamWriter = new StreamWriter(fileStream);
					streamWriter.BaseStream.Seek(0L, SeekOrigin.End);
					streamWriter.Write(text + "\r\n");
					streamWriter.Flush();
				}
			}
			catch
			{
			}
		}

		public static void WriteLog(string file, LogEntry e)
		{
			try
			{
				using (TextDb textDb = new TextDb('\t'))
				{
					textDb.OpenOrCreate(file);
					string text = "";
					if (e.Text != null)
					{
						text = e.Text.Replace("\r", "");
						if (text.EndsWith("\n"))
						{
							text = text.Substring(0, text.Length - 1);
						}
					}
					string text2 = "";
					if (e.EntryType == LogEntryType.INFO)
					{
						text2 = "INFO";
					}
					else if (e.EntryType == LogEntryType.RECV)
					{
						text2 = "RECV";
					}
					else if (e.EntryType == LogEntryType.SEND)
					{
						text2 = "SEND";
					}
					string[] array = text.Split(new char[]
					{
						'\n'
					});
					for (int i = 0; i < array.Length; i++)
					{
						string text3 = array[i];
						textDb.Append(new string[]
						{
							e.ID,
							DateTime.Now.ToString("yyyyMMddHHmmss"),
							(e.RemoteEndPoint != null) ? e.RemoteEndPoint.ToString() : "",
							(e.UserIdentity != null) ? e.UserIdentity.Name : "",
							text2,
							text3
						});
					}
				}
			}
			catch (Exception x)
			{
				Error.DumpError(x);
			}
		}
	}
}

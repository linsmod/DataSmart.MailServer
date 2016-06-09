using System.NetworkToolkit;
using System.NetworkToolkit.IMAP;
using System.NetworkToolkit.Mail;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace DataSmart.MailServer
{
	internal class RecycleBinManager
	{
		public const string DateTimeFormat = "yyyyMMddHHmmss";

		private static string m_RecycleBinPath = "";

		public static string RecycleBinPath
		{
			get
			{
				return RecycleBinManager.m_RecycleBinPath;
			}
			set
			{
				RecycleBinManager.m_RecycleBinPath = value;
			}
		}

		public static List<RecycleBinMessageInfo> GetMessagesInfo(string user, DateTime startDate, DateTime endDate)
		{
			List<RecycleBinMessageInfo> list = new List<RecycleBinMessageInfo>();
			using (FileStream file = RecycleBinManager.GetFile())
			{
				TextReader textReader = new StreamReader(file);
				for (string text = textReader.ReadLine(); text != null; text = textReader.ReadLine())
				{
					if (!text.StartsWith("#") && !text.StartsWith("\0"))
					{
						string[] array = TextUtils.SplitQuotedString(text, ' ', true);
						bool flag = false;
						if (user != null && user != "" && array[2].ToLower() != user.ToLower())
						{
							flag = true;
						}
						if (startDate != DateTime.MinValue && startDate > DateTime.ParseExact(array[1], "yyyyMMddHHmmss", CultureInfo.InvariantCulture).Date)
						{
							flag = true;
						}
						if (endDate != DateTime.MinValue && endDate < DateTime.ParseExact(array[1], "yyyyMMddHHmmss", CultureInfo.InvariantCulture).Date)
						{
							break;
						}
						if (!flag)
						{
							if (array.Length == 5)
							{
								list.Add(new RecycleBinMessageInfo(array[0], DateTime.ParseExact(array[1], "yyyyMMddHHmmss", DateTimeFormatInfo.CurrentInfo), array[2], array[3], 0, ""));
							}
							else if (array.Length == 6)
							{
								list.Add(new RecycleBinMessageInfo(array[0], DateTime.ParseExact(array[1], "yyyyMMddHHmmss", DateTimeFormatInfo.CurrentInfo), array[2], array[3], Convert.ToInt32(array[4]), array[5]));
							}
						}
					}
				}
			}
			return list;
		}

		public static void StoreToRecycleBin(string folderOwner, string folder, string messageFile)
		{
			string text = Guid.NewGuid().ToString().Replace("-", "");
			string text2 = "";
			int num = 0;
			try
			{
				Mail_Message entity = Mail_Message.ParseFromFile(messageFile);
				text2 = IMAP_Envelope.ConstructEnvelope(entity);
				num = (int)new FileInfo(messageFile).Length;
			}
			catch
			{
			}
			if (!Directory.Exists(RecycleBinManager.m_RecycleBinPath))
			{
				try
				{
					Directory.CreateDirectory(RecycleBinManager.m_RecycleBinPath);
				}
				catch
				{
				}
			}
			File.Copy(messageFile, RecycleBinManager.m_RecycleBinPath + text + ".eml");
			using (FileStream file = RecycleBinManager.GetFile())
			{
				file.Position = file.Length;
				byte[] bytes = Encoding.UTF8.GetBytes(string.Concat(new object[]
				{
					text,
					" ",
					DateTime.Now.ToString("yyyyMMddHHmmss"),
					" ",
					folderOwner,
					" ",
					TextUtils.QuoteString(folder),
					" ",
					num,
					" ",
					TextUtils.QuoteString(text2),
					"\r\n"
				}));
				file.Write(bytes, 0, bytes.Length);
			}
		}

		public static Stream GetRecycleBinMessage(string messageID)
		{
			using (FileStream file = RecycleBinManager.GetFile())
			{
				int num = 0;
				StreamLineReader streamLineReader = new StreamLineReader(file);
				long arg_15_0 = file.Position;
				for (string text = streamLineReader.ReadLineString(); text != null; text = streamLineReader.ReadLineString())
				{
					if (!text.StartsWith("#"))
					{
						if (text.StartsWith("\0"))
						{
							num++;
						}
						else
						{
							string[] array = TextUtils.SplitQuotedString(text, ' ');
							if (array[0] == messageID)
							{
								string arg_59_0 = array[2];
								TextUtils.UnQuoteString(array[3]);
								return File.OpenRead(RecycleBinManager.m_RecycleBinPath + messageID + ".eml");
							}
						}
					}
					long arg_87_0 = file.Position;
				}
			}
			throw new Exception("Specified message doesn't exist !");
		}

		public static void DeleteRecycleBinMessage(string messageID)
		{
			using (FileStream file = RecycleBinManager.GetFile())
			{
				int num = 0;
				StreamLineReader streamLineReader = new StreamLineReader(file);
				long position = file.Position;
				for (string text = streamLineReader.ReadLineString(); text != null; text = streamLineReader.ReadLineString())
				{
					if (!text.StartsWith("#"))
					{
						if (text.StartsWith("\0"))
						{
							num++;
						}
						else
						{
							string[] array = TextUtils.SplitQuotedString(text, ' ');
							if (array[0] == messageID)
							{
								string arg_63_0 = array[2];
								TextUtils.UnQuoteString(array[3]);
								File.Delete(RecycleBinManager.m_RecycleBinPath + messageID + ".eml");
								byte[] array2 = new byte[file.Position - position - 2L];
								file.Position = position;
								file.Write(array2, 0, array2.Length);
								file.Position += 2L;
								num++;
								break;
							}
						}
					}
					position = file.Position;
				}
				if (num > 500)
				{
					RecycleBinManager.Vacuum(file);
				}
			}
		}

		public static void RestoreFromRecycleBin(string messageID, IMailServerManagementApi api)
		{
			using (FileStream file = RecycleBinManager.GetFile())
			{
				int num = 0;
				StreamLineReader streamLineReader = new StreamLineReader(file);
				long position = file.Position;
				for (string text = streamLineReader.ReadLineString(); text != null; text = streamLineReader.ReadLineString())
				{
					if (!text.StartsWith("#"))
					{
						if (text.StartsWith("\0"))
						{
							num++;
						}
						else
						{
							string[] array = TextUtils.SplitQuotedString(text, ' ');
							if (array[0] == messageID)
							{
								string text2 = array[2];
								string text3 = TextUtils.UnQuoteString(array[3]);
								using (FileStream fileStream = File.OpenRead(RecycleBinManager.m_RecycleBinPath + messageID + ".eml"))
								{
									if (!api.FolderExists(text2 + "/" + text3))
									{
										api.CreateFolder("system", text2, text3);
									}
									api.StoreMessage("system", text2, text3, fileStream, DateTime.Now, new string[]
									{
										"Recent"
									});
								}
								byte[] array2 = new byte[file.Position - position - 2L];
								file.Position = position;
								file.Write(array2, 0, array2.Length);
								file.Position += 2L;
								num++;
								File.Delete(RecycleBinManager.m_RecycleBinPath + messageID + ".eml");
								break;
							}
						}
					}
					position = file.Position;
				}
				if (num > 500)
				{
					RecycleBinManager.Vacuum(file);
				}
			}
		}

		internal static FileStream GetFile()
		{
			DateTime now = DateTime.Now;
			string str = "";
			while (now.AddSeconds(20.0) > DateTime.Now)
			{
				try
				{
					FileStream fileStream = File.Open(RecycleBinManager.m_RecycleBinPath + "_index.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
					if (fileStream.Length == 0L)
					{
						byte[] bytes = Encoding.ASCII.GetBytes("#\r\n# This file holds messages info, don't delete this file !\r\n#\r\n");
						fileStream.Write(bytes, 0, bytes.Length);
					}
					fileStream.Position = 0L;
					return fileStream;
				}
				catch (Exception ex)
				{
					str = ex.Message;
					Thread.Sleep(5);
				}
			}
			throw new Exception("Opening messages info file timed-out, failed with error: " + str);
		}

		private static void Vacuum(FileStream fs)
		{
			MemoryStream memoryStream = new MemoryStream();
			fs.Position = 0L;
			StreamLineReader streamLineReader = new StreamLineReader(fs);
			for (string text = streamLineReader.ReadLineString(); text != null; text = streamLineReader.ReadLineString())
			{
				if (!text.StartsWith("\0"))
				{
					byte[] bytes = Encoding.ASCII.GetBytes(text + "\r\n");
					memoryStream.Write(bytes, 0, bytes.Length);
				}
			}
			fs.SetLength(memoryStream.Length);
			fs.Position = 0L;
			memoryStream.WriteTo(fs);
		}
	}
}

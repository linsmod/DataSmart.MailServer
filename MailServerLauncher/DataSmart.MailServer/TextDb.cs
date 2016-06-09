using System.NetworkToolkit;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace DataSmart.MailServer
{
	public class TextDb : IDisposable
	{
		private char m_FieldDelimiter = '\t';

		private bool m_Open;

		private Stream m_pDatabaseStream;

		private StreamLineReader m_pReader;

		private string m_CurrentRow;

		public bool IsOpen
		{
			get
			{
				return this.m_Open;
			}
		}

		public string CurrentRowString
		{
			get
			{
				if (!this.m_Open)
				{
					throw new Exception("Database not open, please open or create database first !");
				}
				return this.m_CurrentRow;
			}
		}

		public string[] CurrentRow
		{
			get
			{
				if (!this.m_Open)
				{
					throw new Exception("Database not open, please open or create database first !");
				}
				if (this.m_CurrentRow != null)
				{
					return TextUtils.SplitQuotedString(this.m_CurrentRow, this.m_FieldDelimiter, true);
				}
				return null;
			}
		}

		public TextDb(char fieldDelimiter)
		{
			this.m_FieldDelimiter = fieldDelimiter;
		}

		public void Dispose()
		{
			this.Close();
		}

		public void Open(string file)
		{
			if (!File.Exists(file))
			{
				throw new Exception("Specified database file doesn't exist !");
			}
			this.m_pDatabaseStream = TextDb.OpenOrCreateDb(file);
			this.m_pReader = new StreamLineReader(this.m_pDatabaseStream);
			this.m_Open = true;
		}

		public void OpenOrCreate(string file)
		{
			this.m_pDatabaseStream = TextDb.OpenOrCreateDb(file);
			this.m_pReader = new StreamLineReader(this.m_pDatabaseStream);
			this.m_Open = true;
		}

		public void OpenRead(string file)
		{
			if (!File.Exists(file))
			{
				throw new Exception("Specified database file doesn't exist !");
			}
			this.m_pDatabaseStream = TextDb.OpenOrCreateDb(file);
			this.m_pReader = new StreamLineReader(new BufferedStream(this.m_pDatabaseStream));
			this.m_Open = true;
		}

		public void Close()
		{
			if (!this.m_Open)
			{
				return;
			}
			this.m_pDatabaseStream.Dispose();
			this.m_pDatabaseStream = null;
			this.m_Open = false;
		}

		public bool MoveNext()
		{
			if (!this.m_Open)
			{
				throw new Exception("Database not open, please open or create database first !");
			}
			this.m_CurrentRow = this.m_pReader.ReadLineString();
			return this.m_CurrentRow != null;
		}

		public void Append(string[] values)
		{
			if (!this.m_Open)
			{
				throw new Exception("Database not open, please open or create database first !");
			}
			if (values == null)
			{
				throw new ArgumentException("Parameter value may not be null !");
			}
			this.m_pDatabaseStream.Position = this.m_pDatabaseStream.Length;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < values.Length; i++)
			{
				if (i != values.Length - 1)
				{
					stringBuilder.Append(TextUtils.QuoteString(values[i]) + this.m_FieldDelimiter);
				}
				else
				{
					stringBuilder.Append(TextUtils.QuoteString(values[i]) + "\r\n");
				}
			}
			byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
			this.m_pDatabaseStream.Write(bytes, 0, bytes.Length);
		}

		public void AppendComment(string text)
		{
			if (!this.m_Open)
			{
				throw new Exception("Database not open, please open or create database first !");
			}
			if (text == null)
			{
				throw new ArgumentException("Parameter text may not be null !");
			}
			this.m_pDatabaseStream.Position = this.m_pDatabaseStream.Length;
			byte[] bytes = Encoding.UTF8.GetBytes("# " + text + "\r\n");
			this.m_pDatabaseStream.Write(bytes, 0, bytes.Length);
		}

		internal static FileStream OpenOrCreateDb(string file)
		{
			if (!Directory.Exists(Path.GetDirectoryName(file)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(file));
			}
			DateTime now = DateTime.Now;
			string str = "";
			while (now.AddSeconds(20.0) > DateTime.Now)
			{
				try
				{
					FileStream fileStream = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
					fileStream.Position = 0L;
					return fileStream;
				}
				catch (Exception ex)
				{
					str = ex.Message;
					Thread.Sleep(5);
				}
			}
			throw new Exception("Opening db file timed-out, failed with error: " + str);
		}
	}
}

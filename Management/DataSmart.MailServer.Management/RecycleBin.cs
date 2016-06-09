using System.NetworkToolkit;
using System;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class RecycleBin
	{
		private VirtualServer m_pVirtualServer;

		private bool m_deleteToRecycleBin;

		private int m_deleteMessagesAfter = 1;

		private bool m_ValuesChanged;

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public bool DeleteToRecycleBin
		{
			get
			{
				return this.m_deleteToRecycleBin;
			}
			set
			{
				if (this.m_deleteToRecycleBin != value)
				{
					this.m_deleteToRecycleBin = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public int DeleteMessagesAfter
		{
			get
			{
				return this.m_deleteMessagesAfter;
			}
			set
			{
				if (value < 1 || value > 365)
				{
					throw new ArgumentException("DeleteMessagesAfter value must be between 1 and 365 !");
				}
				if (this.m_deleteMessagesAfter != value)
				{
					this.m_deleteMessagesAfter = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal RecycleBin(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.Bind();
		}

		public DataTable GetMessagesInfo(string user, DateTime startDate, DateTime endDate)
		{
			if (user == null)
			{
				user = "";
			}
			DataTable result;
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
				{
					"GetRecycleBinMessagesInfo ",
					this.m_pVirtualServer.VirtualServerID,
					" ",
					TextUtils.QuoteString(user),
					" ",
					TextUtils.QuoteString(startDate.ToUniversalTime().ToString("yyyyMMddHHmmss")),
					" ",
					TextUtils.QuoteString(endDate.ToUniversalTime().ToString("yyyyMMddHHmmss"))
				}));
				string text = this.m_pVirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Count == 0)
				{
					dataSet.Tables.Add("MessagesInfo");
				}
				result = dataSet.Tables["MessagesInfo"];
			}
			return result;
		}

		public void GetMessage(string messageID, Stream message)
		{
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetRecycleBinMessage " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(messageID));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			int num = Convert.ToInt32(text.Split(new char[]
			{
				' '
			})[1]);
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(message, (long)num);
		}

		public void RestoreRecycleBinMessage(string messageID)
		{
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("RestoreRecycleBinMessage " + this.m_pVirtualServer.VirtualServerID + " " + messageID);
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
		}

		private void Bind()
		{
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetRecycleBinSettings " + this.m_pVirtualServer.VirtualServerID);
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			int num = Convert.ToInt32(text.Split(new char[]
			{
				' '
			}, 2)[1]);
			MemoryStream memoryStream = new MemoryStream();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
			memoryStream.Position = 0L;
			DataSet dataSet = new DataSet();
			dataSet.ReadXml(memoryStream);
			if (dataSet.Tables.Contains("RecycleBinSettings"))
			{
				this.m_deleteToRecycleBin = Convert.ToBoolean(dataSet.Tables["RecycleBinSettings"].Rows[0]["DeleteToRecycleBin"]);
				this.m_deleteMessagesAfter = Convert.ToInt32(dataSet.Tables["RecycleBinSettings"].Rows[0]["DeleteMessagesAfter"]);
			}
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"UpdateRecycleBinSettings ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				this.m_deleteToRecycleBin,
				" ",
				this.m_deleteMessagesAfter
			}));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_ValuesChanged = false;
		}
	}
}

using System;

namespace DataSmart.MailServer
{
	internal class RecycleBinMessageInfo
	{
		private string m_MessageID = "";

		private DateTime m_DeleteTime;

		private string m_User = "";

		private string m_Folder = "";

		private int m_Size;

		private string m_Envelope = "";

		public string MessageID
		{
			get
			{
				return this.m_MessageID;
			}
		}

		public DateTime DeleteTime
		{
			get
			{
				return this.m_DeleteTime;
			}
		}

		public string User
		{
			get
			{
				return this.m_User;
			}
		}

		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		public int Size
		{
			get
			{
				return this.m_Size;
			}
		}

		public string Envelope
		{
			get
			{
				return this.m_Envelope;
			}
		}

		public RecycleBinMessageInfo(string messageID, DateTime deleteTime, string user, string folder, int size, string envelope)
		{
			this.m_MessageID = messageID;
			this.m_DeleteTime = deleteTime;
			this.m_User = user;
			this.m_Folder = folder;
			this.m_Size = size;
			this.m_Envelope = envelope;
		}
	}
}

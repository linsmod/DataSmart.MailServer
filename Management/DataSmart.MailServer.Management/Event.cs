using System;

namespace DataSmart.MailServer.Management
{
	public class Event
	{
		private string m_ID = "";

		private EventType m_EventType;

		private string m_VirtualServer = "";

		private DateTime m_CreateDate;

		private string m_Message = "";

		private string m_Text = "";

		public string ID
		{
			get
			{
				return this.m_ID;
			}
		}

		public EventType Type
		{
			get
			{
				return this.m_EventType;
			}
		}

		public string VirtualServer
		{
			get
			{
				return this.m_VirtualServer;
			}
		}

		public DateTime CreateDate
		{
			get
			{
				return this.m_CreateDate;
			}
		}

		public string Text
		{
			get
			{
				return this.m_Text;
			}
		}

		internal Event(string id, EventType type, string virtualServer, DateTime createDate, string message, string text)
		{
			this.m_ID = id;
			this.m_EventType = type;
			this.m_VirtualServer = virtualServer;
			this.m_CreateDate = createDate;
			this.m_Message = message;
			this.m_Text = text;
		}
	}
}

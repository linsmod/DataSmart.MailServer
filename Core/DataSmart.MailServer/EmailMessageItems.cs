using System.NetworkToolkit.IMAP.Server;
using System;
using System.IO;

namespace DataSmart.MailServer
{
    public class EmailMessageItems
	{
		private string m_MessageID = "";

		private IMAP_MessageItems m_MessageItems = IMAP_MessageItems.Message;

		private Stream m_MessageStream;

		private long m_MessageStartOffset;

		private byte[] m_Header;

		private string m_Envelope;

		private string m_BodyStructure;

		private bool m_MessageExists = true;

		public string MessageID
		{
			get
			{
				return this.m_MessageID;
			}
		}

		public IMAP_MessageItems MessageItems
		{
			get
			{
				return this.m_MessageItems;
			}
		}

		public Stream MessageStream
		{
			get
			{
				if (this.m_MessageStream != null)
				{
					this.m_MessageStream.Position = this.m_MessageStartOffset;
				}
				return this.m_MessageStream;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Property MessageStream value can't be null !");
				}
				if (!value.CanSeek)
				{
					throw new Exception("Stream must support seeking !");
				}
				this.m_MessageStream = value;
				this.m_MessageStartOffset = this.m_MessageStream.Position;
			}
		}

		public byte[] Header
		{
			get
			{
				return this.m_Header;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Property Header value can't be null !");
				}
				this.m_Header = value;
			}
		}

		public string Envelope
		{
			get
			{
				return this.m_Envelope;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Property Envelope value can't be null !");
				}
				this.m_Envelope = value;
			}
		}

		public string BodyStructure
		{
			get
			{
				return this.m_BodyStructure;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Property BodyStructure value can't be null !");
				}
				this.m_BodyStructure = value;
			}
		}

		public bool MessageExists
		{
			get
			{
				return this.m_MessageExists;
			}
			set
			{
				this.m_MessageExists = value;
			}
		}

		public EmailMessageItems(string messageID, IMAP_MessageItems messageItems)
		{
			this.m_MessageID = messageID;
			this.m_MessageItems = messageItems;
		}

		public void CopyTo(IMAP_eArgs_MessageItems e)
		{
			if (this.BodyStructure != null)
			{
				e.BodyStructure = this.BodyStructure;
			}
			if (this.Envelope != null)
			{
				e.Envelope = this.Envelope;
			}
			if (this.Header != null)
			{
				e.Header = this.Header;
			}
			e.MessageExists = this.MessageExists;
			if (this.MessageStream != null)
			{
				e.MessageStream = this.MessageStream;
			}
		}
	}
}

using System;
using System.IO;

namespace System.NetworkToolkit.IMAP.Server
{
	public class IMAP_eArgs_MessageItems
	{
		private IMAP_Session m_pSession;

		private IMAP_Message m_pMessageInfo;

		private IMAP_MessageItems m_MessageItems = IMAP_MessageItems.Message;

		private bool m_CloseMessageStream = true;

		private Stream m_MessageStream;

		private long m_MessageStartOffset;

		private byte[] m_Header;

		private string m_Envelope;

		private string m_BodyStructure;

		private bool m_MessageExists = true;

		public IMAP_Session Session
		{
			get
			{
				return this.m_pSession;
			}
		}

		public IMAP_Message MessageInfo
		{
			get
			{
				return this.m_pMessageInfo;
			}
		}

		public IMAP_MessageItems MessageItems
		{
			get
			{
				return this.m_MessageItems;
			}
		}

		public bool CloseMessageStream
		{
			get
			{
				return this.m_CloseMessageStream;
			}
			set
			{
				this.m_CloseMessageStream = value;
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

		public long MessageSize
		{
			get
			{
				if (this.m_MessageStream == null)
				{
					throw new Exception("You must set MessageStream property first to use this property !");
				}
				return this.m_MessageStream.Length - this.m_MessageStream.Position;
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

		public IMAP_eArgs_MessageItems(IMAP_Session session, IMAP_Message messageInfo, IMAP_MessageItems messageItems)
		{
			this.m_pSession = session;
			this.m_pMessageInfo = messageInfo;
			this.m_MessageItems = messageItems;
		}

		~IMAP_eArgs_MessageItems()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			if (this.m_CloseMessageStream && this.m_MessageStream != null)
			{
				this.m_MessageStream.Dispose();
				this.m_MessageStream = null;
			}
		}

		internal void Validate()
		{
			if ((this.m_MessageItems & IMAP_MessageItems.BodyStructure) != IMAP_MessageItems.None && this.m_BodyStructure == null)
			{
				throw new Exception("IMAP BODYSTRUCTURE is required, but not provided to IMAP server component !");
			}
			if ((this.m_MessageItems & IMAP_MessageItems.Envelope) != IMAP_MessageItems.None && this.m_Envelope == null)
			{
				throw new Exception("IMAP ENVELOPE is required, but not provided to IMAP server component  !");
			}
			if ((this.m_MessageItems & IMAP_MessageItems.Header) != IMAP_MessageItems.None && this.m_Header == null)
			{
				throw new Exception("Message header is required, but not provided to IMAP server component  !");
			}
			if ((this.m_MessageItems & IMAP_MessageItems.Message) != IMAP_MessageItems.None && this.m_MessageStream == null)
			{
				throw new Exception("Full message is required, but not provided to IMAP server component  !");
			}
		}
	}
}

using System;

namespace System.NetworkToolkit.IMAP.Server
{
	public class IMAP_Message
	{
		private IMAP_MessageCollection m_pOwner;

		private string m_ID = "";

		private long m_UID;

		private DateTime m_InternalDate = DateTime.Now;

		private long m_Size;

		private IMAP_MessageFlags m_Flags;

		public int SequenceNo
		{
			get
			{
				return this.m_pOwner.IndexOf(this) + 1;
			}
		}

		public string ID
		{
			get
			{
				return this.m_ID;
			}
		}

		public long UID
		{
			get
			{
				return this.m_UID;
			}
		}

		public DateTime InternalDate
		{
			get
			{
				return this.m_InternalDate;
			}
		}

		public long Size
		{
			get
			{
				return this.m_Size;
			}
		}

		public IMAP_MessageFlags Flags
		{
			get
			{
				return this.m_Flags;
			}
		}

		public string FlagsString
		{
			get
			{
				return IMAP_Utils.MessageFlagsToString(this.m_Flags);
			}
		}

		public IMAP_Message(IMAP_MessageCollection onwer, string id, long uid, DateTime internalDate, long size, IMAP_MessageFlags flags)
		{
			this.m_pOwner = onwer;
			this.m_ID = id;
			this.m_UID = uid;
			this.m_InternalDate = internalDate;
			this.m_Size = size;
			this.m_Flags = flags;
		}

		internal void SetFlags(IMAP_MessageFlags flags)
		{
			this.m_Flags = flags;
		}
	}
}

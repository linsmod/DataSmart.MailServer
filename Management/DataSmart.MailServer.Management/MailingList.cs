using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class MailingList
	{
		private VirtualServer m_pVirtualServer;

		private MailingListCollection m_pOwner;

		private string m_ID;

		private string m_Name;

		private string m_Description;

		private bool m_Enabled;

		private MailingListMemberCollection m_pMembers;

		private MailingListAclCollection m_pAcl;

		private bool m_ValuesChanged;

		public VirtualServer VirtualServer
		{
			get
			{
				return this.m_pVirtualServer;
			}
		}

		public MailingListCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public string ID
		{
			get
			{
				return this.m_ID;
			}
		}

		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				if (this.m_Name != value)
				{
					this.m_Name = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public string Description
		{
			get
			{
				return this.m_Description;
			}
			set
			{
				if (this.m_Description != value)
				{
					this.m_Description = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public bool Enabled
		{
			get
			{
				return this.m_Enabled;
			}
			set
			{
				if (this.m_Enabled != value)
				{
					this.m_Enabled = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public MailingListMemberCollection Members
		{
			get
			{
				if (this.m_pMembers == null)
				{
					this.m_pMembers = new MailingListMemberCollection(this);
				}
				return this.m_pMembers;
			}
		}

		public MailingListAclCollection ACL
		{
			get
			{
				if (this.m_pAcl == null)
				{
					this.m_pAcl = new MailingListAclCollection(this);
				}
				return this.m_pAcl;
			}
		}

		internal MailingList(VirtualServer virtualServer, MailingListCollection owner, string id, string name, string description, bool enabled)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pOwner = owner;
			this.m_ID = id;
			this.m_Name = name;
			this.m_Description = description;
			this.m_Enabled = enabled;
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"UpdateMailingList ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_ID),
				" ",
				TextUtils.QuoteString(this.m_Name),
				" ",
				TextUtils.QuoteString(this.m_Description),
				" ",
				this.m_Enabled
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

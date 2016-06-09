using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class Group
	{
		private VirtualServer m_pVirtualServer;

		private GroupCollection m_pOwner;

		private string m_GroupID = "";

		private string m_GroupName = "";

		private string m_Description = "";

		private bool m_Enabled;

		private GroupMemberCollection m_pMembers;

		private bool m_ValuesChanged;

		public VirtualServer VirtualServer
		{
			get
			{
				return this.m_pVirtualServer;
			}
		}

		public GroupCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string GroupID
		{
			get
			{
				return this.m_GroupID;
			}
		}

		public string GroupName
		{
			get
			{
				return this.m_GroupName;
			}
			set
			{
				if (this.m_GroupName != value)
				{
					this.m_GroupName = value;
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

		public GroupMemberCollection Members
		{
			get
			{
				if (this.m_pMembers == null)
				{
					this.m_pMembers = new GroupMemberCollection(this);
				}
				return this.m_pMembers;
			}
		}

		internal Group(VirtualServer virtualServer, GroupCollection owner, string id, string name, string descritpion, bool enabled)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pOwner = owner;
			this.m_GroupID = id;
			this.m_GroupName = name;
			this.m_Description = descritpion;
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
				"UpdateGroup ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_GroupID),
				" ",
				TextUtils.QuoteString(this.m_GroupName),
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

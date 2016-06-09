using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class SharedRootFolder
	{
		private VirtualServer m_pVirtualServer;

		private SharedRootFolderCollection m_pOwner;

		private string m_ID = "";

		private bool m_Enabled;

		private string m_Name = "";

		private string m_Description = "";

		private SharedFolderRootType m_FolderType = SharedFolderRootType.BoundedRootFolder;

		private string m_BoundedUser = "";

		private string m_BoundedFolder = "";

		private bool m_ValuesChanged;

		public SharedRootFolderCollection Owner
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

		public string ID
		{
			get
			{
				return this.m_ID;
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

		public SharedFolderRootType Type
		{
			get
			{
				return this.m_FolderType;
			}
			set
			{
				if (this.m_FolderType != value)
				{
					this.m_FolderType = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public string BoundedUser
		{
			get
			{
				return this.m_BoundedUser;
			}
			set
			{
				if (this.m_BoundedUser != value)
				{
					this.m_BoundedUser = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public string BoundedFolder
		{
			get
			{
				return this.m_BoundedFolder;
			}
			set
			{
				if (this.m_BoundedFolder != value)
				{
					this.m_BoundedFolder = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal SharedRootFolder(VirtualServer virtualServer, SharedRootFolderCollection owner, string id, bool enabled, string name, string description, SharedFolderRootType type, string boundedUser, string boundedFolder)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pOwner = owner;
			this.m_ID = id;
			this.m_Enabled = enabled;
			this.m_Name = name;
			this.m_Description = description;
			this.m_FolderType = type;
			this.m_BoundedUser = boundedUser;
			this.m_BoundedFolder = boundedFolder;
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"UpdateSharedRootFolder ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_ID),
				" ",
				TextUtils.QuoteString(this.m_Name),
				" ",
				TextUtils.QuoteString(this.m_Description),
				" ",
				(int)this.m_FolderType,
				" ",
				TextUtils.QuoteString(this.m_BoundedUser),
				" ",
				TextUtils.QuoteString(this.m_BoundedFolder),
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

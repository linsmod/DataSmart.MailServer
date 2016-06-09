using System.NetworkToolkit;
using System.NetworkToolkit.IMAP;
using System;

namespace DataSmart.MailServer.Management
{
	public class UserFolderAcl
	{
		private UserFolderAclCollection m_pOwner;

		private UserFolder m_pFolder;

		private string m_UserOrGroup = "";

		private IMAP_ACL_Flags m_Permissions;

		private bool m_ValuesChanged;

		public UserFolderAclCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public UserFolder Folder
		{
			get
			{
				return this.m_pFolder;
			}
		}

		public string UserOrGroup
		{
			get
			{
				return this.m_UserOrGroup;
			}
		}

		public IMAP_ACL_Flags Permissions
		{
			get
			{
				return this.m_Permissions;
			}
			set
			{
				if (this.m_Permissions != value)
				{
					this.m_Permissions = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal UserFolderAcl(UserFolderAclCollection owner, UserFolder folder, string userOrGroup, IMAP_ACL_Flags permissions)
		{
			this.m_pOwner = owner;
			this.m_pFolder = folder;
			this.m_UserOrGroup = userOrGroup;
			this.m_Permissions = permissions;
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			Guid.NewGuid().ToString();
			this.m_pFolder.User.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"SetUserFolderAcl ",
				this.m_pFolder.User.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pFolder.User.UserName),
				" ",
				TextUtils.QuoteString(this.m_pFolder.FolderFullPath),
				" ",
				TextUtils.QuoteString(this.m_UserOrGroup),
				" ",
				(int)this.m_Permissions
			}));
			string text = this.m_pFolder.User.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_ValuesChanged = false;
		}
	}
}

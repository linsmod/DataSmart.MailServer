using System;

namespace DataSmart.MailServer.Management
{
	public class UsersDefaultFolder
	{
		private UsersDefaultFolderCollection m_pOwner;

		private string m_FolderName = "";

		private bool m_Permanent = true;

		public UsersDefaultFolderCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public string FolderName
		{
			get
			{
				return this.m_FolderName;
			}
		}

		public bool Permanent
		{
			get
			{
				return this.m_Permanent;
			}
		}

		internal UsersDefaultFolder(UsersDefaultFolderCollection owner, string folderName, bool permanent)
		{
			this.m_pOwner = owner;
			this.m_FolderName = folderName;
			this.m_Permanent = permanent;
		}
	}
}

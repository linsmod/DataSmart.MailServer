using System;

namespace DataSmart.MailServer
{
	public class SharedFolderRoot
	{
		private string m_RootID = "";

		private bool m_Enabled = true;

		private string m_FolderName = "";

		private string m_Description = "";

		private SharedFolderRootType m_RootType = SharedFolderRootType.BoundedRootFolder;

		private string m_BoundedUser = "";

		private string m_BoundedFolder = "";

		public string RootID
		{
			get
			{
				return this.m_RootID;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.m_Enabled;
			}
		}

		public string FolderName
		{
			get
			{
				return this.m_FolderName;
			}
		}

		public string Description
		{
			get
			{
				return this.m_Description;
			}
		}

		public SharedFolderRootType RootType
		{
			get
			{
				return this.m_RootType;
			}
		}

		public string BoundedUser
		{
			get
			{
				return this.m_BoundedUser;
			}
		}

		public string BoundedFolder
		{
			get
			{
				return this.m_BoundedFolder;
			}
		}

		public SharedFolderRoot(string rootID, bool enabled, string folderName, string description, SharedFolderRootType rootType, string boundedUser, string boundedFolder)
		{
			this.m_RootID = rootID;
			this.m_Enabled = enabled;
			this.m_FolderName = folderName;
			this.m_Description = description;
			this.m_RootType = rootType;
			this.m_BoundedUser = boundedUser;
			this.m_BoundedFolder = boundedFolder;
		}
	}
}

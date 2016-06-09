using System.NetworkToolkit;
using System.NetworkToolkit.IMAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class UserFolderAclCollection : IEnumerable
	{
		private UserFolder m_pFolder;

		private List<UserFolderAcl> m_pAclEntries;

		public UserFolder Folder
		{
			get
			{
				return this.m_pFolder;
			}
		}

		public int Count
		{
			get
			{
				return this.m_pAclEntries.Count;
			}
		}

		public UserFolderAcl this[int index]
		{
			get
			{
				return this.m_pAclEntries[index];
			}
		}

		public UserFolderAcl this[string userOrGroup]
		{
			get
			{
				foreach (UserFolderAcl current in this.m_pAclEntries)
				{
					if (current.UserOrGroup.ToLower() == userOrGroup.ToLower())
					{
						return current;
					}
				}
				throw new Exception("User or group with specified name '" + userOrGroup + "' doesn't exist !");
			}
		}

		internal UserFolderAclCollection(UserFolder folder)
		{
			this.m_pFolder = folder;
			this.m_pAclEntries = new List<UserFolderAcl>();
			this.Bind();
		}

		public UserFolderAcl Add(string userOrGroup, IMAP_ACL_Flags permissions)
		{
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
				TextUtils.QuoteString(userOrGroup),
				" ",
				(int)permissions
			}));
			string text = this.m_pFolder.User.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			UserFolderAcl userFolderAcl = new UserFolderAcl(this, this.m_pFolder, userOrGroup, permissions);
			this.m_pAclEntries.Add(userFolderAcl);
			return userFolderAcl;
		}

		public void Remove(UserFolderAcl aclEntry)
		{
			Guid.NewGuid().ToString();
			this.m_pFolder.User.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"DeleteUserFolderAcl ",
				this.m_pFolder.User.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pFolder.User.UserName),
				" ",
				TextUtils.QuoteString(this.m_pFolder.FolderFullPath),
				" ",
				TextUtils.QuoteString(aclEntry.UserOrGroup)
			}));
			string text = this.m_pFolder.User.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pAclEntries.Remove(aclEntry);
		}

		private void Bind()
		{
			lock (this.m_pFolder.User.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pFolder.User.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
				{
					"GetUserFolderAcl ",
					this.m_pFolder.User.VirtualServer.VirtualServerID,
					" ",
					this.m_pFolder.User.UserID,
					" \"",
					this.m_pFolder.FolderFullPath
				}));
				string text = this.m_pFolder.User.VirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pFolder.User.VirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("ACL"))
				{
					foreach (DataRow dataRow in dataSet.Tables["ACL"].Rows)
					{
						this.m_pAclEntries.Add(new UserFolderAcl(this, this.m_pFolder, dataRow["User"].ToString(), IMAP_Utils.ACL_From_String(dataRow["Permissions"].ToString())));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pAclEntries.GetEnumerator();
		}
	}
}

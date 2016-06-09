using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class UserFolderCollection : IEnumerable
	{
		private UserFolder m_pFolder;

		private User m_pUser;

		private List<UserFolder> m_pFolders;

		public UserFolder Parent
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
				return this.m_pFolders.Count;
			}
		}

		public UserFolder this[int index]
		{
			get
			{
				return this.m_pFolders[index];
			}
		}

		public UserFolder this[string folderName]
		{
			get
			{
				foreach (UserFolder current in this.m_pFolders)
				{
					if (current.FolderName.ToLower() == folderName.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Folder with specified name '" + folderName + "' doesn't exist !");
			}
		}

		internal List<UserFolder> List
		{
			get
			{
				return this.m_pFolders;
			}
		}

		internal UserFolderCollection(bool bind, UserFolder folder, User user)
		{
			this.m_pFolder = folder;
			this.m_pUser = user;
			this.m_pFolders = new List<UserFolder>();
			if (bind)
			{
				this.Bind();
			}
		}

		public UserFolder Add(string newFolder)
		{
			Guid.NewGuid().ToString();
			string text = "";
			string text2 = newFolder;
			if (this.m_pFolder != null)
			{
				text = this.m_pFolder.FolderFullPath;
				text2 = text + "/" + newFolder;
			}
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"AddUserFolder ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pUser.UserName),
				" ",
				TextUtils.QuoteString(text2)
			}));
			string text3 = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text3.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text3);
			}
			UserFolder userFolder = new UserFolder(this, this.m_pUser, this.m_pFolder, text, newFolder);
			this.m_pFolders.Add(userFolder);
			return userFolder;
		}

		public void Remove(UserFolder folder)
		{
			Guid.NewGuid().ToString();
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"DeleteUserFolder ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pUser.UserName),
				" ",
				TextUtils.QuoteString(folder.FolderFullPath)
			}));
			string text = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pFolders.Remove(folder);
		}

		public bool Contains(string folderName)
		{
			foreach (UserFolder current in this.m_pFolders)
			{
				if (current.FolderName.ToLower() == folderName.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		private void Bind()
		{
			lock (this.m_pUser.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetUserFolders " + this.m_pUser.VirtualServer.VirtualServerID + " " + this.m_pUser.UserID);
				string text = this.m_pUser.VirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("Folders"))
				{
					foreach (DataRow dataRow in dataSet.Tables["Folders"].Rows)
					{
						string[] array = dataRow["Folder"].ToString().Split(new char[]
						{
							'/'
						});
						UserFolderCollection userFolderCollection = this;
						string text2 = "";
						string[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							string text3 = array2[i];
							if (!userFolderCollection.Contains(text3))
							{
								UserFolder item = new UserFolder(userFolderCollection, this.m_pUser, userFolderCollection.Parent, text2, text3);
								userFolderCollection.List.Add(item);
							}
							userFolderCollection = userFolderCollection[text3].ChildFolders;
							if (text2 == "")
							{
								text2 = text3;
							}
							else
							{
								text2 = text2 + "/" + text3;
							}
						}
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pFolders.GetEnumerator();
		}
	}
}

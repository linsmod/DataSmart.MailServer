using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class UsersDefaultFolderCollection : IEnumerable
	{
		private VirtualServer m_pVirtualServer;

		private List<UsersDefaultFolder> m_pFolders;

		public VirtualServer VirtualServer
		{
			get
			{
				return this.m_pVirtualServer;
			}
		}

		public int Count
		{
			get
			{
				return this.m_pFolders.Count;
			}
		}

		public UsersDefaultFolder this[int index]
		{
			get
			{
				return this.m_pFolders[index];
			}
		}

		public UsersDefaultFolder this[string folderName]
		{
			get
			{
				foreach (UsersDefaultFolder current in this.m_pFolders)
				{
					if (current.FolderName.ToLower() == folderName.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Users default folder with specified name '" + folderName + "' doesn't exist !");
			}
		}

		public UsersDefaultFolderCollection(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pFolders = new List<UsersDefaultFolder>();
			this.Bind();
		}

		public UsersDefaultFolder Add(string folderName, bool permanent)
		{
			if (folderName.IndexOfAny(new char[]
			{
				'\\',
				'/'
			}) > -1)
			{
				throw new Exception("Folders with path not allowed !");
			}
			Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"AddUsersDefaultFolder ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(folderName),
				" ",
				permanent
			}));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			UsersDefaultFolder usersDefaultFolder = new UsersDefaultFolder(this, folderName, permanent);
			this.m_pFolders.Add(usersDefaultFolder);
			return usersDefaultFolder;
		}

		public void Remove(UsersDefaultFolder folder)
		{
			if (folder.FolderName.ToLower() == "inbox")
			{
				throw new Exception("Inbox is permanent system folder and can't be deleted ! '");
			}
			Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("DeleteUsersDefaultFolder " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(folder.FolderName));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pFolders.Remove(folder);
		}

		public bool Contains(string folderName)
		{
			foreach (UsersDefaultFolder current in this.m_pFolders)
			{
				if (current.FolderName.ToLower() == folderName.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		public UsersDefaultFolder GetFolderByName(string folderName)
		{
			foreach (UsersDefaultFolder current in this.m_pFolders)
			{
				if (current.FolderName.ToLower() == folderName.ToLower())
				{
					return current;
				}
			}
			throw new Exception("Folder with specified name '" + folderName + "' doesn't exist !");
		}

		private void Bind()
		{
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetUsersDefaultFolders " + this.m_pVirtualServer.VirtualServerID);
				string text = this.m_pVirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("UsersDefaultFolders"))
				{
					foreach (DataRow dataRow in dataSet.Tables["UsersDefaultFolders"].Rows)
					{
						this.m_pFolders.Add(new UsersDefaultFolder(this, dataRow["FolderName"].ToString(), ConvertEx.ToBoolean(dataRow["Permanent"])));
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

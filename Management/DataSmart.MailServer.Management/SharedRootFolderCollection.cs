using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class SharedRootFolderCollection : IEnumerable
	{
		private VirtualServer m_pVirtualServer;

		private List<SharedRootFolder> m_pRootFolders;

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
				return this.m_pRootFolders.Count;
			}
		}

		public SharedRootFolder this[int index]
		{
			get
			{
				return this.m_pRootFolders[index];
			}
		}

		public SharedRootFolder this[string rootFolderID]
		{
			get
			{
				foreach (SharedRootFolder current in this.m_pRootFolders)
				{
					if (current.ID.ToLower() == rootFolderID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("SharedRootFolder with specified ID '" + rootFolderID + "' doesn't exist !");
			}
		}

		internal SharedRootFolderCollection(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pRootFolders = new List<SharedRootFolder>();
			this.Bind();
		}

		public SharedRootFolder Add(bool enabled, string name, string description, SharedFolderRootType type, string boundedUser, string boundedFolder)
		{
			string text = Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"AddSharedRootFolder ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(text),
				" ",
				TextUtils.QuoteString(name),
				" ",
				TextUtils.QuoteString(description),
				" ",
				(int)type,
				" ",
				TextUtils.QuoteString(boundedUser),
				" ",
				TextUtils.QuoteString(boundedFolder),
				" ",
				enabled
			}));
			string text2 = this.m_pVirtualServer.Server.ReadLine();
			if (!text2.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text2);
			}
			SharedRootFolder sharedRootFolder = new SharedRootFolder(this.m_pVirtualServer, this, text, enabled, name, description, type, boundedUser, boundedFolder);
			this.m_pRootFolders.Add(sharedRootFolder);
			return sharedRootFolder;
		}

		public void Remove(SharedRootFolder sharedFolder)
		{
			Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("DeleteSharedRootFolder " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(sharedFolder.ID));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pRootFolders.Remove(sharedFolder);
		}

		public bool Contains(string rootFolderName)
		{
			foreach (SharedRootFolder current in this.m_pRootFolders)
			{
				if (current.Name.ToLower() == rootFolderName.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		public SharedRootFolder GetRootFolderByName(string rootFolderName)
		{
			foreach (SharedRootFolder current in this.m_pRootFolders)
			{
				if (current.Name.ToLower() == rootFolderName.ToLower())
				{
					return current;
				}
			}
			throw new Exception("SharedRootFolder with specified name '" + rootFolderName + "' doesn't exist !");
		}

		private void Bind()
		{
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetSharedRootFolders " + this.m_pVirtualServer.VirtualServerID);
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
				if (dataSet.Tables.Contains("SharedFoldersRoots"))
				{
					foreach (DataRow dataRow in dataSet.Tables["SharedFoldersRoots"].Rows)
					{
						this.m_pRootFolders.Add(new SharedRootFolder(this.m_pVirtualServer, this, dataRow["RootID"].ToString(), Convert.ToBoolean(dataRow["Enabled"].ToString()), dataRow["Folder"].ToString(), dataRow["Description"].ToString(), (SharedFolderRootType)Convert.ToInt32(dataRow["RootType"]), dataRow["BoundedUser"].ToString(), dataRow["BoundedFolder"].ToString()));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pRootFolders.GetEnumerator();
		}
	}
}

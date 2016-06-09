using System.NetworkToolkit;
using System.NetworkToolkit.Mail;
using System;
using System.Data;
using System.Globalization;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class UserFolder
	{
		private UserFolderCollection m_pOwner;

		private User m_pUser;

		private UserFolder m_pParent;

		private string m_FolderName = "";

		private string m_Path = "";

		private UserFolderAclCollection m_pAcl;

		private UserFolderCollection m_pChildFolders;

		public UserFolderCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public User User
		{
			get
			{
				return this.m_pUser;
			}
		}

		public UserFolder Parent
		{
			get
			{
				return this.m_pParent;
			}
		}

		public string FolderName
		{
			get
			{
				return this.m_FolderName;
			}
		}

		public string FolderPath
		{
			get
			{
				return this.m_Path;
			}
		}

		public string FolderFullPath
		{
			get
			{
				if (this.m_Path != "")
				{
					return this.m_Path + "/" + this.m_FolderName;
				}
				return this.m_FolderName;
			}
		}

		public UserFolderAclCollection ACL
		{
			get
			{
				if (this.m_pAcl == null)
				{
					this.m_pAcl = new UserFolderAclCollection(this);
				}
				return this.m_pAcl;
			}
		}

		public UserFolderCollection ChildFolders
		{
			get
			{
				return this.m_pChildFolders;
			}
		}

		public int MessagesCount
		{
			get
			{
				return Convert.ToInt32(this.GetFolderInfo()[1]);
			}
		}

		public long SizeUsed
		{
			get
			{
				return (long)Convert.ToInt32(this.GetFolderInfo()[2]);
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return DateTime.ParseExact(this.GetFolderInfo()[0], "yyyyMMdd HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
			}
		}

		internal UserFolder(UserFolderCollection owner, User user, UserFolder parent, string folderPath, string folderName)
		{
			this.m_pOwner = owner;
			this.m_pUser = user;
			this.m_pParent = parent;
			this.m_Path = folderPath;
			this.m_FolderName = folderName;
			this.m_pChildFolders = new UserFolderCollection(false, this, this.m_pUser);
		}

		public void Rename(string newFolderName)
		{
			UserFolderCollection userFolderCollection = this.m_pOwner;
			while (userFolderCollection.Parent != null)
			{
				userFolderCollection = userFolderCollection.Parent.Owner;
			}
			string[] array = newFolderName.Replace('\\', '/').Split(new char[]
			{
				'/'
			});
			if (array.Length != 1)
			{
				string arg_4A_0 = array[array.Length - 1];
			}
			UserFolderCollection userFolderCollection2 = userFolderCollection;
			for (int i = 0; i < array.Length - 1; i++)
			{
				userFolderCollection2 = userFolderCollection2[array[i]].ChildFolders;
			}
			Guid.NewGuid().ToString();
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"RenameUserFolder ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pUser.UserName),
				" ",
				TextUtils.QuoteString(this.FolderFullPath),
				" ",
				TextUtils.QuoteString(newFolderName)
			}));
			string text = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pOwner.List.Remove(this);
			userFolderCollection2.List.Add(this);
		}

		public DataSet GetMessagesInfo()
		{
			DataSet result;
			lock (this.m_pUser.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
				{
					"GetUserFolderMessagesInfo ",
					this.m_pUser.VirtualServer.VirtualServerID,
					" ",
					TextUtils.QuoteString(this.m_pUser.UserName),
					" ",
					TextUtils.QuoteString(this.FolderFullPath)
				}));
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
				result = dataSet;
			}
			return result;
		}

		public void DeleteMessage(string messageID, long uid)
		{
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"DeleteUserFolderMessage ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pUser.UserName),
				" ",
				TextUtils.QuoteString(this.FolderFullPath),
				" ",
				TextUtils.QuoteString(messageID),
				" ",
				TextUtils.QuoteString(uid.ToString())
			}));
			string text = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
		}

		public void GetMessage(string messageID, Stream message)
		{
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"GetUserFolderMessage ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pUser.UserName),
				" ",
				TextUtils.QuoteString(this.FolderFullPath),
				" ",
				TextUtils.QuoteString(messageID)
			}));
			string text = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			int num = Convert.ToInt32(text.Split(new char[]
			{
				' '
			})[1]);
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(message, (long)num);
		}

		public void StoreMessage(Stream message)
		{
			long position = message.Position;
			try
			{
				Mail_Message.ParseFromStream(message);
			}
			catch
			{
				throw new Exception("Specified message is not valid message or is corrupt !");
			}
			message.Position = position;
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"StoreUserFolderMessage ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pUser.UserName),
				" ",
				TextUtils.QuoteString(this.FolderFullPath),
				" ",
				(message.Length - message.Position).ToString()
			}));
			string text = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteStream(message);
			text = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
		}

		private string[] GetFolderInfo()
		{
			Guid.NewGuid().ToString();
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"GetUserFolderInfo ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pUser.UserName),
				" ",
				TextUtils.QuoteString(this.FolderFullPath)
			}));
			string text = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			return TextUtils.SplitQuotedString(text.Substring(4), ' ', true);
		}
	}
}

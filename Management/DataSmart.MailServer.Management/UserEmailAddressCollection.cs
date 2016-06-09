using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class UserEmailAddressCollection : IEnumerable
	{
		private User m_pUser;

		private List<string> m_pEmails;

		public User User
		{
			get
			{
				return this.m_pUser;
			}
		}

		public int Count
		{
			get
			{
				return this.m_pEmails.Count;
			}
		}

		public string this[int index]
		{
			get
			{
				return this.m_pEmails[index];
			}
		}

		public string this[string emailAddress]
		{
			get
			{
				foreach (string current in this.m_pEmails)
				{
					if (current.ToLower() == emailAddress.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Email address '" + emailAddress + "' doesn't exist !");
			}
		}

		internal UserEmailAddressCollection(User user)
		{
			this.m_pUser = user;
			this.m_pEmails = new List<string>();
			this.Bind();
		}

		public void Add(string emailAddress)
		{
			Guid.NewGuid().ToString();
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"AddUserEmailAddress ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pUser.UserID),
				" ",
				TextUtils.QuoteString(emailAddress)
			}));
			string text = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pEmails.Add(emailAddress);
		}

		public void Remove(string emailAddress)
		{
			Guid.NewGuid().ToString();
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"DeleteUserEmailAddress ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pUser.UserID),
				" ",
				TextUtils.QuoteString(emailAddress)
			}));
			string text = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pEmails.Remove(emailAddress);
		}

		public string[] ToArray()
		{
			return this.m_pEmails.ToArray();
		}

		private void Bind()
		{
			lock (this.m_pUser.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetUserEmailAddresses " + this.m_pUser.VirtualServer.VirtualServerID + " " + this.m_pUser.UserID);
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
				if (dataSet.Tables.Contains("UserAddresses"))
				{
					foreach (DataRow dataRow in dataSet.Tables["UserAddresses"].Rows)
					{
						this.m_pEmails.Add(dataRow["Address"].ToString());
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pEmails.GetEnumerator();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Timers;

namespace DataSmart.MailServer
{
	public class BadLoginManager : IDisposable
	{
		private class BadLoginEntry
		{
			private class UserEntry
			{
				private string m_UserName = "";

				private DateTime m_CreationTime;

				private int m_BadLoginCount = 1;

				public string UserName
				{
					get
					{
						return this.m_UserName;
					}
				}

				public DateTime CreationTime
				{
					get
					{
						return this.m_CreationTime;
					}
				}

				public int BadLoginCount
				{
					get
					{
						return this.m_BadLoginCount;
					}
				}

				public UserEntry(string userName)
				{
					this.m_UserName = userName;
					this.m_CreationTime = DateTime.Now;
				}

				public void IncreaseBadLoginCount()
				{
					this.m_BadLoginCount++;
				}
			}

			private string m_IP = "";

			private DateTime m_CreationTime;

			private Dictionary<string, BadLoginManager.BadLoginEntry.UserEntry> m_pUsers;

			public string IP
			{
				get
				{
					return this.m_IP;
				}
			}

			public DateTime CreationTime
			{
				get
				{
					return this.m_CreationTime;
				}
			}

			public BadLoginEntry(string ip)
			{
				this.m_IP = ip;
				this.m_CreationTime = DateTime.Now;
				this.m_pUsers = new Dictionary<string, BadLoginManager.BadLoginEntry.UserEntry>();
			}

			public void IncreaseBadLoginCount(string userName)
			{
				userName = userName.ToLower();
				lock (this.m_pUsers)
				{
					if (this.m_pUsers.ContainsKey(userName))
					{
						this.m_pUsers[userName].IncreaseBadLoginCount();
					}
					else
					{
						this.m_pUsers.Add(userName, new BadLoginManager.BadLoginEntry.UserEntry(userName));
					}
				}
			}

			public int GetUserBadLoginCount(string userName)
			{
				userName = userName.ToLower();
				int result;
				lock (this.m_pUsers)
				{
					if (this.m_pUsers.ContainsKey(userName))
					{
						result = this.m_pUsers[userName].BadLoginCount;
					}
					else
					{
						result = 0;
					}
				}
				return result;
			}

			public void RemoveOlderThan(int seconds)
			{
				List<string> list = new List<string>();
				foreach (string current in this.m_pUsers.Keys)
				{
					if (this.m_pUsers[current].CreationTime.AddSeconds((double)seconds) < DateTime.Now)
					{
						list.Add(current);
					}
				}
				foreach (string current2 in list)
				{
					this.m_pUsers.Remove(current2);
				}
			}

			public bool IsEmpty()
			{
				return this.m_pUsers.Count == 0;
			}
		}

		private Dictionary<string, BadLoginManager.BadLoginEntry> m_pEntries;

		private int m_MaxBadLogins = 3;

		private Timer m_pTimer;

		public int MaximumBadLogins
		{
			get
			{
				return this.m_MaxBadLogins;
			}
			set
			{
				if (this.m_MaxBadLogins != value)
				{
					this.m_MaxBadLogins = value;
				}
			}
		}

		public BadLoginManager()
		{
			this.m_pEntries = new Dictionary<string, BadLoginManager.BadLoginEntry>();
			this.m_pTimer = new Timer();
			this.m_pTimer.Interval = 30000.0;
			this.m_pTimer.Elapsed += new ElapsedEventHandler(this.m_pTimer_Elapsed);
			this.m_pTimer.Enabled = true;
		}

		public void Dispose()
		{
			if (this.m_pTimer != null)
			{
				this.m_pTimer.Enabled = false;
				this.m_pTimer = null;
			}
		}

		private void m_pTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				lock (this)
				{
					List<string> list = new List<string>();
					foreach (BadLoginManager.BadLoginEntry current in this.m_pEntries.Values)
					{
						current.RemoveOlderThan(30);
						if (current.IsEmpty())
						{
							list.Add(current.IP);
						}
					}
					foreach (string current2 in list)
					{
						this.m_pEntries.Remove(current2);
					}
				}
			}
			catch
			{
			}
		}

		public void Put(string ip, string userName)
		{
			lock (this.m_pEntries)
			{
				if (!this.m_pEntries.ContainsKey(ip))
				{
					this.m_pEntries.Add(ip, new BadLoginManager.BadLoginEntry(ip));
				}
				this.m_pEntries[ip].IncreaseBadLoginCount(userName);
			}
		}

		public bool IsExceeded(string ip, string userName)
		{
			return this.m_pEntries.ContainsKey(ip) && this.m_pEntries[ip].GetUserBadLoginCount(userName) > this.m_MaxBadLogins;
		}
	}
}

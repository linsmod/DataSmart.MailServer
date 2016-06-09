using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class SessionCollection : IEnumerable
	{
		private Server m_pOwner;

		private List<Session> m_pSessions;

		public Server Server
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public int Count
		{
			get
			{
				return this.m_pSessions.Count;
			}
		}

		public Session this[int index]
		{
			get
			{
				return this.m_pSessions[index];
			}
		}

		public Session this[string sessionID]
		{
			get
			{
				foreach (Session current in this.m_pSessions)
				{
					if (sessionID.ToLower() == sessionID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Session with specified ID '" + sessionID + "' doesn't exist !");
			}
		}

		internal List<Session> List
		{
			get
			{
				return this.m_pSessions;
			}
		}

		internal SessionCollection(Server owner)
		{
			this.m_pOwner = owner;
			this.m_pSessions = new List<Session>();
			this.Bind();
		}

		public void Refresh()
		{
			lock (this.m_pOwner.LockSynchronizer)
			{
				this.m_pSessions.Clear();
				this.Bind();
			}
		}

		public bool ConatainsID(string sessionID)
		{
			foreach (Session current in this.m_pSessions)
			{
				if (current.ID == sessionID)
				{
					return true;
				}
			}
			return false;
		}

		public Session GetSessionByID(string sessionID)
		{
			foreach (Session current in this.m_pSessions)
			{
				if (current.ID == sessionID)
				{
					return current;
				}
			}
			throw new Exception("Session with specified session ID '" + sessionID + "' doesn't exist !");
		}

		private void Bind()
		{
			lock (this.m_pOwner.LockSynchronizer)
			{
				this.m_pOwner.TCP_Client.TcpStream.WriteLine("GetSessions");
				string text = this.m_pOwner.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pOwner.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("Sessions"))
				{
					foreach (DataRow dataRow in dataSet.Tables["Sessions"].Rows)
					{
						this.m_pSessions.Add(new Session(this, dataRow["SessionID"].ToString(), dataRow["SessionType"].ToString(), Convert.ToDateTime(dataRow["SessionStartTime"]), Convert.ToInt32(dataRow["ExpectedTimeout"]), dataRow["UserName"].ToString(), dataRow["LocalEndPoint"].ToString(), dataRow["RemoteEndPoint"].ToString(), Convert.ToInt32(dataRow["ReadTransferRate"]), Convert.ToInt32(dataRow["WriteTransferRate"]), dataRow["SessionLog"].ToString()));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pSessions.GetEnumerator();
		}
	}
}

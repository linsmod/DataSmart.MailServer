using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;

namespace DataSmart.MailServer.Management
{
	public class IPSecurityCollection : IEnumerable
	{
		private VirtualServer m_pVirtualServer;

		private List<IPSecurity> m_pEntries;

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
				return this.m_pEntries.Count;
			}
		}

		public IPSecurity this[int index]
		{
			get
			{
				return this.m_pEntries[index];
			}
		}

		public IPSecurity this[string securityEntryID]
		{
			get
			{
				foreach (IPSecurity current in this.m_pEntries)
				{
					if (current.ID.ToLower() == securityEntryID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("IPSecurity with specified ID '" + securityEntryID + "' doesn't exist !");
			}
		}

		internal IPSecurityCollection(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pEntries = new List<IPSecurity>();
			this.Bind();
		}

		public IPSecurity Add(bool enabled, string description, ServiceKind service, IPSecurityAction action, IPAddress startIP, IPAddress endIP)
		{
			string text = Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"AddIPSecurityEntry ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(text),
				" ",
				enabled,
				" ",
				TextUtils.QuoteString(description),
				" ",
				(int)service,
				" ",
				(int)action,
				" ",
				TextUtils.QuoteString(startIP.ToString()),
				" ",
				TextUtils.QuoteString(endIP.ToString())
			}));
			string text2 = this.m_pVirtualServer.Server.ReadLine();
			if (!text2.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text2);
			}
			IPSecurity iPSecurity = new IPSecurity(this, text, enabled, description, service, action, startIP, endIP);
			this.m_pEntries.Add(iPSecurity);
			return iPSecurity;
		}

		public void Remove(IPSecurity entry)
		{
			Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("DeleteIPSecurityEntry " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(entry.ID));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pEntries.Remove(entry);
		}

		public IPSecurity[] ToArray()
		{
			return this.m_pEntries.ToArray();
		}

		public void Refresh()
		{
			this.m_pEntries.Clear();
			this.Bind();
		}

		private void Bind()
		{
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetIPSecurity " + this.m_pVirtualServer.VirtualServerID);
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
				if (dataSet.Tables.Contains("IPSecurity"))
				{
					foreach (DataRow dataRow in dataSet.Tables["IPSecurity"].Rows)
					{
						this.m_pEntries.Add(new IPSecurity(this, dataRow["ID"].ToString(), Convert.ToBoolean(dataRow["Enabled"]), dataRow["Description"].ToString(), (ServiceKind)Convert.ToInt32(dataRow["Service"]), (IPSecurityAction)Convert.ToInt32(dataRow["Action"]), IPAddress.Parse(dataRow["StartIP"].ToString()), IPAddress.Parse(dataRow["EndIP"].ToString())));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pEntries.GetEnumerator();
		}
	}
}

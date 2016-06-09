using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class DomainCollection : IEnumerable
	{
		private VirtualServer m_pVirtualServer;

		private List<Domain> m_pDomains;

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
				return this.m_pDomains.Count;
			}
		}

		public Domain this[int index]
		{
			get
			{
				return this.m_pDomains[index];
			}
		}

		public Domain this[string domainID]
		{
			get
			{
				foreach (Domain current in this.m_pDomains)
				{
					if (current.DomainID.ToLower() == domainID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Domain with specified ID '" + domainID + "' doesn't exist !");
			}
		}

		public DomainCollection(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pDomains = new List<Domain>();
			this.Bind();
		}

		public Domain Add(string name, string description)
		{
			string text = Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"AddDomain ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(text),
				" ",
				TextUtils.QuoteString(name),
				" ",
				TextUtils.QuoteString(description)
			}));
			string text2 = this.m_pVirtualServer.Server.ReadLine();
			if (!text2.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text2);
			}
			Domain domain = new Domain(this, text, name, description);
			this.m_pDomains.Add(domain);
			return domain;
		}

		public void Remove(Domain domain)
		{
			Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("DeleteDomain " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(domain.DomainID));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pDomains.Remove(domain);
		}

		public bool Contains(string domainName)
		{
			foreach (Domain current in this.m_pDomains)
			{
				if (current.DomainName.ToLower() == domainName.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		public void Refresh()
		{
			this.m_pDomains.Clear();
			this.Bind();
		}

		private void Bind()
		{
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetDomains " + this.m_pVirtualServer.VirtualServerID);
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
				if (dataSet.Tables.Contains("Domains"))
				{
					foreach (DataRow dataRow in dataSet.Tables["Domains"].Rows)
					{
						this.m_pDomains.Add(new Domain(this, dataRow["DomainID"].ToString(), dataRow["DomainName"].ToString(), dataRow["Description"].ToString()));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pDomains.GetEnumerator();
		}
	}
}

using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace DataSmart.MailServer.Management
{
	public class VirtualServerCollection : IEnumerable
	{
		private Server m_pParent;

		private List<VirtualServer> m_pVirtualServers;

		public Server Server
		{
			get
			{
				return this.m_pParent;
			}
		}

		public int Count
		{
			get
			{
				return this.m_pVirtualServers.Count;
			}
		}

		public VirtualServer this[int index]
		{
			get
			{
				return this.m_pVirtualServers[index];
			}
		}

		public VirtualServer this[string virtualServerID]
		{
			get
			{
				foreach (VirtualServer current in this.m_pVirtualServers)
				{
					if (current.VirtualServerID.ToLower() == virtualServerID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Virtual server with specified ID '" + virtualServerID + "' doesn't exist !");
			}
		}

		internal VirtualServerCollection(Server parent)
		{
			this.m_pParent = parent;
			this.m_pVirtualServers = new List<VirtualServer>();
			this.Bind();
		}

		public DataSet GetVirtualServerAPIs()
		{
			DataSet result;
			lock (this.m_pParent)
			{
				this.m_pParent.TCP_Client.TcpStream.WriteLine("GetVirtualServerAPIs");
				string text = this.m_pParent.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pParent.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				result = dataSet;
			}
			return result;
		}

		public VirtualServer Add(bool enabled, string name, string assembly, string type, string initString)
		{
			string text = Guid.NewGuid().ToString();
			this.m_pParent.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"AddVirtualServer ",
				text,
				" ",
				enabled,
				" ",
				TextUtils.QuoteString(name),
				" ",
				TextUtils.QuoteString(assembly),
				" ",
				TextUtils.QuoteString(type),
				" ",
				TextUtils.QuoteString(Convert.ToBase64String(Encoding.UTF8.GetBytes(initString)))
			}));
			string text2 = this.m_pParent.ReadLine();
			if (!text2.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text2);
			}
			VirtualServer virtualServer = new VirtualServer(this.m_pParent, this, text, enabled, name, assembly, type, initString);
			this.m_pVirtualServers.Add(virtualServer);
			return virtualServer;
		}

		public void Remove(VirtualServer server)
		{
			this.m_pParent.TCP_Client.TcpStream.WriteLine("DeleteVirtualServer " + server.VirtualServerID);
			string text = this.m_pParent.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pVirtualServers.Remove(server);
		}

		private void Bind()
		{
			lock (this.m_pParent)
			{
				this.m_pParent.TCP_Client.TcpStream.WriteLine("GetVirtualServers");
				string text = this.m_pParent.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pParent.TCP_Client.TcpStream.ReadFixedCount(memoryStream, num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("Servers"))
				{
					foreach (DataRow dataRow in dataSet.Tables["Servers"].Rows)
					{
						this.m_pVirtualServers.Add(new VirtualServer(this.m_pParent, this, dataRow["ID"].ToString(), ConvertEx.ToBoolean(dataRow["Enabled"], true), dataRow["Name"].ToString(), dataRow["API_assembly"].ToString(), dataRow["API_class"].ToString(), dataRow["API_initstring"].ToString()));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pVirtualServers.GetEnumerator();
		}
	}
}

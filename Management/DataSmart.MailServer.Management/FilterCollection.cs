using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class FilterCollection : IEnumerable
	{
		private VirtualServer m_pVirtualServer;

		private List<Filter> m_pFilters;

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
				return this.m_pFilters.Count;
			}
		}

		public Filter this[int index]
		{
			get
			{
				return this.m_pFilters[index];
			}
		}

		public Filter this[string filterID]
		{
			get
			{
				foreach (Filter current in this.m_pFilters)
				{
					if (current.ID.ToLower() == filterID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Filter with specified ID '" + filterID + "' doesn't exist !");
			}
		}

		internal FilterCollection(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pFilters = new List<Filter>();
			this.Bind();
		}

		public DataSet GetFilterTypes()
		{
			DataSet result;
			lock (this.m_pVirtualServer.Server)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetFilterTypes " + this.m_pVirtualServer.VirtualServerID);
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
				result = dataSet;
			}
			return result;
		}

		public Filter Add(bool enabled, string description, string assembly, string filterClass)
		{
			string text = Guid.NewGuid().ToString();
			long ticks = DateTime.Now.Ticks;
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"AddFilter ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(text),
				" ",
				ticks,
				" ",
				TextUtils.QuoteString(description),
				" ",
				TextUtils.QuoteString(assembly),
				" ",
				TextUtils.QuoteString(filterClass),
				" ",
				enabled
			}));
			string text2 = this.m_pVirtualServer.Server.ReadLine();
			if (!text2.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text2);
			}
			Filter filter = new Filter(this.m_pVirtualServer, this, text, ticks, enabled, description, assembly, filterClass);
			this.m_pFilters.Add(filter);
			return filter;
		}

		public void Remove(Filter filter)
		{
			Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("DeleteFilter " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(filter.ID));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pFilters.Remove(filter);
		}

		public Filter[] ToArray()
		{
			return this.m_pFilters.ToArray();
		}

		public void Refresh()
		{
			this.m_pFilters.Clear();
			this.Bind();
		}

		private void Bind()
		{
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetFilters " + this.m_pVirtualServer.VirtualServerID);
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
				if (dataSet.Tables.Contains("SmtpFilters"))
				{
					foreach (DataRow dataRow in dataSet.Tables["SmtpFilters"].Rows)
					{
						this.m_pFilters.Add(new Filter(this.m_pVirtualServer, this, dataRow["FilterID"].ToString(), Convert.ToInt64(dataRow["Cost"]), Convert.ToBoolean(dataRow["Enabled"].ToString()), dataRow["Description"].ToString(), dataRow["Assembly"].ToString(), dataRow["ClassName"].ToString()));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pFilters.GetEnumerator();
		}
	}
}

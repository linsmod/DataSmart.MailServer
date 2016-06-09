using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class SipCallCollection : IEnumerable
	{
		private VirtualServer m_pVirtualServer;

		private List<SipCall> m_pCalls;

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
				return this.m_pCalls.Count;
			}
		}

		internal SipCallCollection(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pCalls = new List<SipCall>();
			this.Bind();
		}

		public void Refresh()
		{
			this.m_pCalls.Clear();
			this.Bind();
		}

		private void Bind()
		{
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetSipCalls " + this.m_pVirtualServer.VirtualServerID);
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
				if (dataSet.Tables.Contains("SipCalls"))
				{
					foreach (DataRow dataRow in dataSet.Tables["SipCalls"].Rows)
					{
						this.m_pCalls.Add(new SipCall(this, dataRow["CallID"].ToString(), dataRow["Caller"].ToString(), dataRow["Callee"].ToString(), Convert.ToDateTime(dataRow["StartTime"])));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pCalls.GetEnumerator();
		}
	}
}

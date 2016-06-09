using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class QueueItemCollection : IEnumerable
	{
		private VirtualServer m_pVirtualServer;

		private List<QueueItem> m_pCollection;

		private bool m_smtp_relay;

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
				return this.m_pCollection.Count;
			}
		}

		public QueueItem this[int index]
		{
			get
			{
				return this.m_pCollection[index];
			}
		}

		internal QueueItemCollection(VirtualServer virtualServer, bool smtp_relay)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_smtp_relay = smtp_relay;
			this.m_pCollection = new List<QueueItem>();
			this.Bind();
		}

		public void Refresh()
		{
			this.m_pCollection.Clear();
			this.Bind();
		}

		private void Bind()
		{
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				if (this.m_smtp_relay)
				{
					this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetQueue " + this.m_pVirtualServer.VirtualServerID + " 1");
				}
				else
				{
					this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetQueue " + this.m_pVirtualServer.VirtualServerID + " 0");
				}
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
				if (dataSet.Tables.Contains("Queue"))
				{
					foreach (DataRow dataRow in dataSet.Tables["Queue"].Rows)
					{
						this.m_pCollection.Add(new QueueItem(Convert.ToDateTime(dataRow["CreateTime"]), dataRow["Header"].ToString()));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pCollection.GetEnumerator();
		}
	}
}

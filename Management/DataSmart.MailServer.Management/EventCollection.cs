using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class EventCollection : IEnumerable
	{
		private Server m_pOwner;

		private List<Event> m_pEvents;

		public int Count
		{
			get
			{
				return this.m_pEvents.Count;
			}
		}

		public Event this[int index]
		{
			get
			{
				return this.m_pEvents[index];
			}
		}

		public Event this[string eventID]
		{
			get
			{
				foreach (Event current in this.m_pEvents)
				{
					if (current.ID.ToLower() == eventID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Event with specified ID '" + eventID + "' doesn't exist !");
			}
		}

		internal EventCollection(Server owner)
		{
			this.m_pOwner = owner;
			this.m_pEvents = new List<Event>();
			this.Bind();
		}

		private void Bind()
		{
			lock (this.m_pOwner)
			{
				this.m_pOwner.TCP_Client.TcpStream.WriteLine("GetEvents");
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
				if (dataSet.Tables.Contains("Events"))
				{
					foreach (DataRow dataRow in dataSet.Tables["Events"].Rows)
					{
						this.m_pEvents.Add(new Event(dataRow["ID"].ToString(), (EventType)Convert.ToInt32(dataRow["Type"]), dataRow["VirtualServer"].ToString(), Convert.ToDateTime(dataRow["CreateDate"]), dataRow["Text"].ToString(), dataRow["Text"].ToString()));
					}
				}
			}
		}

		public void Refresh()
		{
			this.m_pEvents.Clear();
			this.Bind();
		}

		public void Clear()
		{
			this.m_pOwner.TCP_Client.TcpStream.WriteLine("ClearEvents");
			string text = this.m_pOwner.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pEvents.Clear();
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pEvents.GetEnumerator();
		}
	}
}

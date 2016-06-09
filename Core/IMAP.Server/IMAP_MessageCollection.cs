using System.NetworkToolkit.IMAP;
using System;
using System.Collections;
using System.Collections.Generic;

namespace System.NetworkToolkit.IMAP.Server
{
	public class IMAP_MessageCollection : IEnumerable
	{
		private SortedList<long, IMAP_Message> m_pMessages;

		public int Count
		{
			get
			{
				return this.m_pMessages.Count;
			}
		}

		public IMAP_Message this[int index]
		{
			get
			{
				return this.m_pMessages.Values[index];
			}
		}

		public IMAP_MessageCollection()
		{
			this.m_pMessages = new SortedList<long, IMAP_Message>();
		}

		public IMAP_Message Add(string id, long uid, DateTime internalDate, long size, IMAP_MessageFlags flags)
		{
			if (uid < 1L)
			{
				throw new ArgumentException("Message UID value must be > 0 !");
			}
			IMAP_Message iMAP_Message = new IMAP_Message(this, id, uid, internalDate, size, flags);
			this.m_pMessages.Add(uid, iMAP_Message);
			return iMAP_Message;
		}

		public void Remove(IMAP_Message message)
		{
			this.m_pMessages.Remove(message.UID);
		}

		public bool ContainsUID(long uid)
		{
			return this.m_pMessages.ContainsKey(uid);
		}

		public int IndexOf(IMAP_Message message)
		{
			return this.m_pMessages.IndexOfKey(message.UID);
		}

		public void Clear()
		{
			this.m_pMessages.Clear();
		}

		public IMAP_Message[] GetWithFlags(IMAP_MessageFlags flags)
		{
			List<IMAP_Message> list = new List<IMAP_Message>();
			foreach (IMAP_Message current in this.m_pMessages.Values)
			{
				if ((current.Flags & flags) != IMAP_MessageFlags.None)
				{
					list.Add(current);
				}
			}
			return list.ToArray();
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pMessages.Values.GetEnumerator();
		}
	}
}

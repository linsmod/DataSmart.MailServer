using System;
using System.Collections;
using System.Collections.Generic;

namespace DataSmart.MailServer.Management
{
	public class SipGatewayCollection : IEnumerable
	{
		private SystemSettings m_pOwner;

		private List<SipGateway> m_pCollection;

		public SystemSettings Owner
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
				return this.m_pCollection.Count;
			}
		}

		internal SipGatewayCollection(SystemSettings owner)
		{
			this.m_pOwner = owner;
			this.m_pCollection = new List<SipGateway>();
		}

		public SipGateway Add(string uriScheme, string transport, string host, int port, string realm, string userName, string password)
		{
			SipGateway result = this.AddInternal(uriScheme, transport, host, port, realm, userName, password);
			this.m_pOwner.SetValuesChanged();
			return result;
		}

		internal SipGateway AddInternal(string uriScheme, string transport, string host, int port, string realm, string userName, string password)
		{
			SipGateway sipGateway = new SipGateway(this, uriScheme, transport, host, port, realm, userName, password);
			this.m_pCollection.Add(sipGateway);
			return sipGateway;
		}

		public void Remove(SipGateway value)
		{
			this.m_pCollection.Remove(value);
			this.m_pOwner.SetValuesChanged();
		}

		public void Clear()
		{
			this.m_pCollection.Clear();
			this.m_pOwner.SetValuesChanged();
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pCollection.GetEnumerator();
		}
	}
}

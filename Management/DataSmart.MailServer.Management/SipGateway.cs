using System;

namespace DataSmart.MailServer.Management
{
	public class SipGateway
	{
		private SipGatewayCollection m_pCollection;

		private string m_UriScheme = "";

		private string m_Transport = "UDP";

		private string m_Host = "";

		private int m_Port = 5060;

		private string m_Realm = "";

		private string m_UserName = "";

		private string m_Password = "";

		public string UriScheme
		{
			get
			{
				return this.m_UriScheme;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Value cant be null or empty !");
				}
				if (this.m_UriScheme != value)
				{
					this.m_UriScheme = value;
					this.m_pCollection.Owner.SetValuesChanged();
				}
			}
		}

		public string Transport
		{
			get
			{
				return this.m_Transport;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Value cant be null or empty !");
				}
				if (this.m_Transport != value)
				{
					this.m_Transport = value;
					this.m_pCollection.Owner.SetValuesChanged();
				}
			}
		}

		public string Host
		{
			get
			{
				return this.m_Host;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Value cant be null or empty !");
				}
				if (this.m_Host != value)
				{
					this.m_Host = value;
					this.m_pCollection.Owner.SetValuesChanged();
				}
			}
		}

		public int Port
		{
			get
			{
				return this.m_Port;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException("Value must be >= 1 !");
				}
				if (this.m_Port != value)
				{
					this.m_Port = value;
					this.m_pCollection.Owner.SetValuesChanged();
				}
			}
		}

		public string Realm
		{
			get
			{
				return this.m_Realm;
			}
			set
			{
				if (value == null)
				{
					this.m_Realm = "";
				}
				if (this.m_Realm != value)
				{
					this.m_Realm = value;
					this.m_pCollection.Owner.SetValuesChanged();
				}
			}
		}

		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
			set
			{
				if (value == null)
				{
					this.m_UserName = "";
				}
				if (this.m_UserName != value)
				{
					this.m_UserName = value;
					this.m_pCollection.Owner.SetValuesChanged();
				}
			}
		}

		public string Password
		{
			get
			{
				return this.m_Password;
			}
			set
			{
				if (value == null)
				{
					this.m_Password = "";
				}
				if (this.m_Password != value)
				{
					this.m_Password = value;
					this.m_pCollection.Owner.SetValuesChanged();
				}
			}
		}

		internal SipGateway(SipGatewayCollection owner, string uriScheme, string transport, string host, int port, string realm, string userName, string password)
		{
			this.m_pCollection = owner;
			this.m_UriScheme = uriScheme;
			this.m_Transport = transport;
			this.m_Host = host;
			this.m_Port = port;
			this.m_Realm = realm;
			this.m_UserName = userName;
			this.m_Password = password;
		}

		public void Remove()
		{
			this.m_pCollection.Remove(this);
		}
	}
}

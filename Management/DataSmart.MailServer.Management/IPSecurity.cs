using System.NetworkToolkit;
using System;
using System.Net;

namespace DataSmart.MailServer.Management
{
	public class IPSecurity
	{
		private IPSecurityCollection m_pOwner;

		private string m_ID = "";

		private bool m_Enabled;

		private string m_Description = "";

		private ServiceKind m_Service;

		private IPSecurityAction m_Action;

		private IPAddress m_pStartIP;

		private IPAddress m_pEndIP;

		private bool m_ValuesChanged;

		public IPSecurityCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string ID
		{
			get
			{
				return this.m_ID;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.m_Enabled;
			}
			set
			{
				if (this.m_Enabled != value)
				{
					this.m_Enabled = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public string Description
		{
			get
			{
				return this.m_Description;
			}
			set
			{
				if (this.m_Description != value)
				{
					this.m_Description = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public ServiceKind Service
		{
			get
			{
				return this.m_Service;
			}
			set
			{
				if (this.m_Service != value)
				{
					this.m_Service = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public IPSecurityAction Action
		{
			get
			{
				return this.m_Action;
			}
			set
			{
				if (this.m_Action != value)
				{
					this.m_Action = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public IPAddress StartIP
		{
			get
			{
				return this.m_pStartIP;
			}
			set
			{
				if (this.m_pStartIP != value)
				{
					this.m_pStartIP = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public IPAddress EndIP
		{
			get
			{
				return this.m_pEndIP;
			}
			set
			{
				if (this.m_pEndIP != value)
				{
					this.m_pEndIP = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal IPSecurity(IPSecurityCollection owner, string id, bool enabled, string description, ServiceKind service, IPSecurityAction action, IPAddress startIP, IPAddress endIP)
		{
			this.m_pOwner = owner;
			this.m_ID = id;
			this.m_Enabled = enabled;
			this.m_Description = description;
			this.m_Service = service;
			this.m_Action = action;
			this.m_pStartIP = startIP;
			this.m_pEndIP = endIP;
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			Guid.NewGuid().ToString();
			this.m_pOwner.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"UpdateIPSecurityEntry ",
				this.m_pOwner.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_ID),
				" ",
				this.m_Enabled,
				" ",
				TextUtils.QuoteString(this.m_Description),
				" ",
				(int)this.m_Service,
				" ",
				(int)this.m_Action,
				" ",
				TextUtils.QuoteString(this.m_pStartIP.ToString()),
				" ",
				TextUtils.QuoteString(this.m_pEndIP.ToString())
			}));
			string text = this.m_pOwner.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
		}
	}
}

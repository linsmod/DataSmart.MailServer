using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class Domain
	{
		private DomainCollection m_pOwner;

		private string m_DomainID = "";

		private string m_DomainName = "";

		private string m_Description = "";

		private bool m_ValuesChanged;

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public DomainCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public string DomainID
		{
			get
			{
				return this.m_DomainID;
			}
		}

		public string DomainName
		{
			get
			{
				return this.m_DomainName;
			}
			set
			{
				if (this.m_DomainName != value)
				{
					this.m_DomainName = value;
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

		internal Domain(DomainCollection owner, string id, string name, string description)
		{
			this.m_pOwner = owner;
			this.m_DomainID = id;
			this.m_DomainName = name;
			this.m_Description = description;
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			this.m_pOwner.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"UpdateDomain ",
				this.m_pOwner.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_DomainID),
				" ",
				TextUtils.QuoteString(this.m_DomainName),
				" ",
				TextUtils.QuoteString(this.m_Description)
			}));
			string text = this.m_pOwner.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_ValuesChanged = false;
			this.m_pOwner.VirtualServer.DomainChanged();
		}
	}
}

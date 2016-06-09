using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class Filter
	{
		private VirtualServer m_pVirtualServer;

		private FilterCollection m_pOwner;

		private string m_ID = "";

		private long m_Cost;

		private bool m_Enabled;

		private string m_Description = "";

		private string m_Assembly = "";

		private string m_Class = "";

		private bool m_ValuesChanged;

		public FilterCollection Owner
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

		public string AssemblyName
		{
			get
			{
				return this.m_Assembly;
			}
			set
			{
				if (this.m_Assembly != value)
				{
					this.m_Assembly = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public string Class
		{
			get
			{
				return this.m_Class;
			}
			set
			{
				if (this.m_Class != value)
				{
					this.m_Class = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal Filter(VirtualServer virtualServer, FilterCollection owner, string id, long cost, bool enabled, string description, string assembly, string filterClass)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pOwner = owner;
			this.m_ID = id;
			this.m_Cost = cost;
			this.m_Enabled = enabled;
			this.m_Description = description;
			this.m_Assembly = assembly;
			this.m_Class = filterClass;
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"UpdateFilter ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_ID),
				" ",
				this.m_Cost,
				" ",
				TextUtils.QuoteString(this.m_Description),
				" ",
				TextUtils.QuoteString(this.m_Assembly),
				" ",
				TextUtils.QuoteString(this.m_Class),
				" ",
				this.m_Enabled
			}));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_ValuesChanged = false;
		}
	}
}

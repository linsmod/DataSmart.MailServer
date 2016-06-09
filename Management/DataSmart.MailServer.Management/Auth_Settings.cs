using System;

namespace DataSmart.MailServer.Management
{
	public class Auth_Settings
	{
		private SystemSettings m_pSysSettings;

		private ServerAuthenticationType_enum m_AuthType;

		private string m_WinDomain = "";

		private string m_LdapServer = "";

		private string m_LdapDn = "";

		public ServerAuthenticationType_enum AuthenticationType
		{
			get
			{
				return this.m_AuthType;
			}
			set
			{
				if (this.m_AuthType != value)
				{
					this.m_AuthType = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public string WinDomain
		{
			get
			{
				return this.m_WinDomain;
			}
			set
			{
				if (this.m_WinDomain != value)
				{
					this.m_WinDomain = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public string LdapServer
		{
			get
			{
				return this.m_LdapServer;
			}
			set
			{
				if (this.m_LdapServer != value)
				{
					this.m_LdapServer = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public string LdapDn
		{
			get
			{
				return this.m_LdapDn;
			}
			set
			{
				if (this.m_LdapDn != value)
				{
					this.m_LdapDn = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		internal Auth_Settings(SystemSettings sysSettings, ServerAuthenticationType_enum authType, string winDomain, string ldapServer, string ldapDN)
		{
			this.m_pSysSettings = sysSettings;
			this.m_AuthType = authType;
			this.m_WinDomain = winDomain;
			this.m_LdapServer = ldapServer;
			this.m_LdapDn = ldapDN;
		}
	}
}

using DataSmart.MailServer.Management;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SystemForms
{
	public class AuthenticationForm : Form
	{
		private TabControl m_pTab;

		private Button m_pApply;

		private Label mt_AuthenticationType;

		private ComboBox m_pAuthenticationType;

		private Label mt_DomainName;

		private TextBox m_pDomainName;

		private Label mt_LdapServer;

		private TextBox m_pLdapServer;

		private Label mt_LdapDN;

		private TextBox m_pLdapDN;

		private VirtualServer m_pVirtualServer;

		public AuthenticationForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.LoadData();
		}

		private void InitializeComponent()
		{
			this.m_pTab = new TabControl();
			this.m_pTab.Size = new Size(515, 490);
			this.m_pTab.Location = new Point(5, 0);
			this.m_pTab.TabPages.Add(new TabPage("General"));
			this.m_pApply = new Button();
			this.m_pApply.Size = new Size(70, 20);
			this.m_pApply.Location = new Point(450, 500);
			this.m_pApply.Text = "Apply";
			this.m_pApply.Click += new EventHandler(this.m_pApply_Click);
			this.mt_AuthenticationType = new Label();
			this.mt_AuthenticationType.Size = new Size(150, 20);
			this.mt_AuthenticationType.Location = new Point(10, 30);
			this.mt_AuthenticationType.Text = "Authentication Type:";
			this.m_pAuthenticationType = new ComboBox();
			this.m_pAuthenticationType.Size = new Size(150, 20);
			this.m_pAuthenticationType.Location = new Point(10, 50);
			this.m_pAuthenticationType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pAuthenticationType.SelectedIndexChanged += new EventHandler(this.m_pAuthenticationType_SelectedIndexChanged);
			this.m_pAuthenticationType.Items.Add("Integrated");
			this.m_pAuthenticationType.Items.Add("Windows");
			this.m_pAuthenticationType.Items.Add("LDAP");
			this.mt_DomainName = new Label();
			this.mt_DomainName.Size = new Size(150, 20);
			this.mt_DomainName.Location = new Point(10, 80);
			this.mt_DomainName.Text = "Domain:";
			this.mt_DomainName.Visible = false;
			this.m_pDomainName = new TextBox();
			this.m_pDomainName.Size = new Size(150, 20);
			this.m_pDomainName.Location = new Point(10, 100);
			this.m_pDomainName.Visible = false;
			this.mt_LdapServer = new Label();
			this.mt_LdapServer.Size = new Size(100, 20);
			this.mt_LdapServer.Location = new Point(0, 80);
			this.mt_LdapServer.TextAlign = ContentAlignment.MiddleRight;
			this.mt_LdapServer.Text = "LDAP Server:";
			this.mt_LdapServer.Visible = false;
			this.m_pLdapServer = new TextBox();
			this.m_pLdapServer.Size = new Size(250, 20);
			this.m_pLdapServer.Location = new Point(105, 80);
			this.m_pLdapServer.Visible = false;
			this.mt_LdapDN = new Label();
			this.mt_LdapDN.Size = new Size(100, 20);
			this.mt_LdapDN.Location = new Point(0, 105);
			this.mt_LdapDN.TextAlign = ContentAlignment.MiddleRight;
			this.mt_LdapDN.Text = "LDAP DN:";
			this.mt_LdapDN.Visible = false;
			this.m_pLdapDN = new TextBox();
			this.m_pLdapDN.Size = new Size(250, 20);
			this.m_pLdapDN.Location = new Point(105, 105);
			this.m_pLdapDN.Visible = false;
			this.m_pLdapDN.Text = "CN=%user,DC=domain,DC=com";
			this.m_pTab.TabPages[0].Controls.Add(this.mt_AuthenticationType);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pAuthenticationType);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_DomainName);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pDomainName);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_LdapServer);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pLdapServer);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_LdapDN);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pLdapDN);
			base.Controls.Add(this.m_pTab);
			base.Controls.Add(this.m_pApply);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (!base.Visible)
			{
				this.SaveData(true);
			}
		}

		private void m_pAuthenticationType_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.mt_DomainName.Visible = false;
			this.m_pDomainName.Visible = false;
			this.mt_LdapServer.Visible = false;
			this.m_pLdapServer.Visible = false;
			this.mt_LdapDN.Visible = false;
			this.m_pLdapDN.Visible = false;
			if (this.m_pAuthenticationType.Text == "Windows")
			{
				this.mt_DomainName.Visible = true;
				this.m_pDomainName.Visible = true;
				return;
			}
			if (this.m_pAuthenticationType.Text == "LDAP")
			{
				this.mt_LdapServer.Visible = true;
				this.m_pLdapServer.Visible = true;
				this.mt_LdapDN.Visible = true;
				this.m_pLdapDN.Visible = true;
			}
		}

		private void m_pApply_Click(object sender, EventArgs e)
		{
			this.SaveData(false);
		}

		private void LoadData()
		{
			try
			{
				Auth_Settings authentication = this.m_pVirtualServer.SystemSettings.Authentication;
				this.m_pAuthenticationType.SelectedIndex = Convert.ToInt32(authentication.AuthenticationType) - 1;
				this.m_pDomainName.Text = authentication.WinDomain;
				this.m_pLdapServer.Text = authentication.LdapServer;
				this.m_pLdapDN.Text = authentication.LdapDn;
				if (this.m_pLdapDN.Text == "")
				{
					this.m_pLdapDN.Text = "CN=%user,DC=domain,DC=com";
				}
			}
			catch (Exception x)
			{
				ErrorForm errorForm = new ErrorForm(x, new StackTrace());
				errorForm.ShowDialog(this);
			}
		}

		private void SaveData(bool confirmSave)
		{
			try
			{
				Auth_Settings authentication = this.m_pVirtualServer.SystemSettings.Authentication;
				authentication.AuthenticationType = this.m_pAuthenticationType.SelectedIndex + ServerAuthenticationType_enum.Windows;
				authentication.WinDomain = this.m_pDomainName.Text;
				authentication.LdapServer = this.m_pLdapServer.Text;
				authentication.LdapDn = this.m_pLdapDN.Text;
				if (this.m_pVirtualServer.SystemSettings.HasChanges && (!confirmSave || MessageBox.Show(this, "You have changes settings, do you want to save them ?", "Confirm:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
				{
					this.m_pVirtualServer.SystemSettings.Commit();
				}
			}
			catch (Exception x)
			{
				ErrorForm errorForm = new ErrorForm(x, new StackTrace());
				errorForm.ShowDialog(this);
			}
		}
	}
}

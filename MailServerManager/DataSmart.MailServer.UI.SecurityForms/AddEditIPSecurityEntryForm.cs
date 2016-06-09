using DataSmart.MailServer.Management;
using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SecurityForms
{
	public class AddEditIPSecurityEntryForm : Form
	{
		private CheckBox m_pEnabled;

		private Label mt_Description;

		private TextBox m_pDescription;

		private Label mt_Service;

		private ComboBox m_pService;

		private Label mt_Action;

		private ComboBox m_pAction;

		private Label mt_Type;

		private ComboBox m_pType;

		private Label mt_StartIP;

		private TextBox m_pStartIP;

		private Label mt_EndIP;

		private TextBox m_pEndIP;

		private GroupBox m_pGroupbox1;

		private Button m_pCancel;

		private Button m_pOk;

		private VirtualServer m_pVirtualServer;

		private IPSecurity m_pSecurityEntry;

		public string SecurityEntryID
		{
			get
			{
				if (this.m_pSecurityEntry != null)
				{
					return this.m_pSecurityEntry.ID;
				}
				return "";
			}
		}

		public AddEditIPSecurityEntryForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.m_pService.SelectedIndex = 0;
			this.m_pAction.SelectedIndex = 0;
			this.m_pType.SelectedIndex = 0;
		}

		public AddEditIPSecurityEntryForm(VirtualServer virtualServer, IPSecurity securityEntry)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pSecurityEntry = securityEntry;
			this.InitializeComponent();
			this.m_pEnabled.Checked = securityEntry.Enabled;
			this.m_pDescription.Text = securityEntry.Description;
			this.m_pService.SelectedIndex = securityEntry.Service - ServiceKind.SMTP;
			this.m_pAction.SelectedIndex = securityEntry.Action - IPSecurityAction.Allow;
			if (securityEntry.StartIP.Equals(securityEntry.EndIP))
			{
				this.m_pType.SelectedIndex = 0;
			}
			else
			{
				this.m_pType.SelectedIndex = 1;
			}
			this.m_pStartIP.Text = securityEntry.StartIP.ToString();
			this.m_pEndIP.Text = securityEntry.EndIP.ToString();
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(392, 243);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Add/Edit IP security entry";
			this.m_pEnabled = new CheckBox();
			this.m_pEnabled.Size = new Size(100, 20);
			this.m_pEnabled.Location = new Point(115, 20);
			this.m_pEnabled.Text = "Enabled";
			this.m_pEnabled.Checked = true;
			this.mt_Description = new Label();
			this.mt_Description.Size = new Size(100, 18);
			this.mt_Description.Location = new Point(10, 45);
			this.mt_Description.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Description.Text = "Description:";
			this.m_pDescription = new TextBox();
			this.m_pDescription.Size = new Size(270, 20);
			this.m_pDescription.Location = new Point(115, 45);
			this.mt_Service = new Label();
			this.mt_Service.Size = new Size(100, 18);
			this.mt_Service.Location = new Point(10, 70);
			this.mt_Service.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Service.Text = "Service:";
			this.m_pService = new ComboBox();
			this.m_pService.Size = new Size(100, 20);
			this.m_pService.Location = new Point(115, 70);
			this.m_pService.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pService.Items.Add(new WComboBoxItem("SMTP", ServiceKind.SMTP));
			this.m_pService.Items.Add(new WComboBoxItem("POP3", ServiceKind.POP3));
			this.m_pService.Items.Add(new WComboBoxItem("IMAP", ServiceKind.IMAP));
			this.m_pService.Items.Add(new WComboBoxItem("SMTP Relay", ServiceKind.Relay));
			this.mt_Action = new Label();
			this.mt_Action.Size = new Size(100, 18);
			this.mt_Action.Location = new Point(10, 95);
			this.mt_Action.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Action.Text = "Action:";
			this.m_pAction = new ComboBox();
			this.m_pAction.Size = new Size(100, 20);
			this.m_pAction.Location = new Point(115, 95);
			this.m_pAction.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pAction.Items.Add(new WComboBoxItem("Allow", IPSecurityAction.Allow));
			this.m_pAction.Items.Add(new WComboBoxItem("Deny", IPSecurityAction.Deny));
			this.mt_Type = new Label();
			this.mt_Type.Size = new Size(100, 18);
			this.mt_Type.Location = new Point(10, 120);
			this.mt_Type.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Type.Text = "Type:";
			this.m_pType = new ComboBox();
			this.m_pType.Size = new Size(100, 20);
			this.m_pType.Location = new Point(115, 120);
			this.m_pType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pType.SelectedIndexChanged += new EventHandler(this.m_pType_SelectedIndexChanged);
			this.m_pType.Items.Add("IP");
			this.m_pType.Items.Add("IP Range");
			this.mt_StartIP = new Label();
			this.mt_StartIP.Size = new Size(100, 18);
			this.mt_StartIP.Location = new Point(10, 145);
			this.mt_StartIP.TextAlign = ContentAlignment.MiddleRight;
			this.mt_StartIP.Text = "Start IP:";
			this.m_pStartIP = new TextBox();
			this.m_pStartIP.Size = new Size(270, 20);
			this.m_pStartIP.Location = new Point(115, 145);
			this.mt_EndIP = new Label();
			this.mt_EndIP.Size = new Size(100, 18);
			this.mt_EndIP.Location = new Point(10, 170);
			this.mt_EndIP.TextAlign = ContentAlignment.MiddleRight;
			this.mt_EndIP.Text = "End IP:";
			this.m_pEndIP = new TextBox();
			this.m_pEndIP.Size = new Size(270, 20);
			this.m_pEndIP.Location = new Point(115, 170);
			this.m_pGroupbox1 = new GroupBox();
			this.m_pGroupbox1.Size = new Size(390, 3);
			this.m_pGroupbox1.Location = new Point(3, 205);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(235, 220);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(310, 220);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pEnabled);
			base.Controls.Add(this.mt_Description);
			base.Controls.Add(this.m_pDescription);
			base.Controls.Add(this.mt_Service);
			base.Controls.Add(this.m_pService);
			base.Controls.Add(this.mt_Action);
			base.Controls.Add(this.m_pAction);
			base.Controls.Add(this.mt_Type);
			base.Controls.Add(this.m_pType);
			base.Controls.Add(this.mt_StartIP);
			base.Controls.Add(this.m_pStartIP);
			base.Controls.Add(this.mt_EndIP);
			base.Controls.Add(this.m_pEndIP);
			base.Controls.Add(this.m_pGroupbox1);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pType.SelectedIndex == 0)
			{
				this.m_pEndIP.Enabled = false;
				return;
			}
			if (this.m_pType.SelectedIndex == 1)
			{
				this.m_pEndIP.Enabled = true;
			}
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			IPAddress iPAddress = null;
			IPAddress iPAddress2 = null;
			if (this.m_pDescription.Text == "")
			{
				MessageBox.Show(this, "Please fill description !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			try
			{
				iPAddress = IPAddress.Parse(this.m_pStartIP.Text);
				if (this.m_pType.SelectedIndex == 0)
				{
					iPAddress2 = iPAddress;
				}
				else
				{
					try
					{
						iPAddress2 = IPAddress.Parse(this.m_pEndIP.Text);
					}
					catch
					{
						MessageBox.Show(this, "Invalid end IP value !", "Invalid IP value", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}
				}
			}
			catch
			{
				MessageBox.Show(this, "Invalid start IP value !", "Invalid IP value", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (iPAddress.AddressFamily == iPAddress2.AddressFamily)
			{
				if (this.m_pSecurityEntry == null)
				{
					this.m_pSecurityEntry = this.m_pVirtualServer.IpSecurity.Add(this.m_pEnabled.Checked, this.m_pDescription.Text, (ServiceKind)((WComboBoxItem)this.m_pService.SelectedItem).Tag, (IPSecurityAction)((WComboBoxItem)this.m_pAction.SelectedItem).Tag, iPAddress, iPAddress2);
				}
				else
				{
					this.m_pSecurityEntry.Enabled = this.m_pEnabled.Checked;
					this.m_pSecurityEntry.Description = this.m_pDescription.Text;
					this.m_pSecurityEntry.Service = (ServiceKind)((WComboBoxItem)this.m_pService.SelectedItem).Tag;
					this.m_pSecurityEntry.Action = (IPSecurityAction)((WComboBoxItem)this.m_pAction.SelectedItem).Tag;
					this.m_pSecurityEntry.StartIP = iPAddress;
					this.m_pSecurityEntry.EndIP = iPAddress2;
					this.m_pSecurityEntry.Commit();
				}
				base.DialogResult = DialogResult.OK;
				base.Close();
				return;
			}
			MessageBox.Show(this, "Start IP and End IP must be from same address familily, you can't mix IPv4 and IPv6 addresses !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}
}

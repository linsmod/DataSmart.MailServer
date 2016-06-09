using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.MonitoringForms
{
    public class SipRegistrationForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private Label mt_VirtualServer;

		private ComboBox m_pVirtualServer;

		private Label mt_AOR;

		private TextBox m_pAOR;

		private Button m_pGetAOR;

		private Label mt_Contact;

		private TextBox m_pContact;

		private NumericUpDown m_pExpires;

		private NumericUpDown m_pPriority;

		private ToolStrip m_pToolbar;

		private ListView m_pContacts;

		private GroupBox m_pSeparator2;

		private Button m_pCancel;

		private Button m_pOk;

		private Server m_pServer;

		private SipRegistration m_pRegistration;

		private List<string> m_pContactsToRemove;

		public VirtualServer VirtualServer
		{
			get
			{
				return (VirtualServer)((WComboBoxItem)this.m_pVirtualServer.SelectedItem).Tag;
			}
		}

		public string AddressOfRecord
		{
			get
			{
				return this.m_pAOR.Text;
			}
		}

		public SipRegistrationForm(Server server)
		{
			this.m_pServer = server;
			this.m_pContactsToRemove = new List<string>();
			this.InitializeComponent();
			this.m_pToolbar.Items[3].Enabled = false;
			foreach (VirtualServer virtualServer in this.m_pServer.VirtualServers)
			{
				this.m_pVirtualServer.Items.Add(new WComboBoxItem(virtualServer.Name, virtualServer));
			}
			if (this.m_pVirtualServer.Items.Count > 0)
			{
				this.m_pVirtualServer.SelectedIndex = 0;
			}
		}

		public SipRegistrationForm(Server server, SipRegistration registration)
		{
			this.m_pServer = server;
			this.m_pRegistration = registration;
			this.m_pContactsToRemove = new List<string>();
			this.InitializeComponent();
			foreach (VirtualServer virtualServer in this.m_pServer.VirtualServers)
			{
				this.m_pVirtualServer.Items.Add(new WComboBoxItem(virtualServer.Name, virtualServer));
			}
			if (this.m_pVirtualServer.Items.Count > 0)
			{
				this.m_pVirtualServer.SelectedIndex = 0;
			}
			this.m_pVirtualServer.Enabled = false;
			this.m_pAOR.Enabled = false;
			this.m_pAOR.Text = registration.AddressOfRecord;
			this.LoadContacts();
		}

		private void InitializeComponent()
		{
			base.StartPosition = FormStartPosition.CenterScreen;
			base.ClientSize = new Size(500, 320);
			this.Text = "Add/Update SIP registration.";
			base.Icon = ResManager.GetIcon("rule.ico");
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetIcon("rule.ico").ToBitmap();
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(450, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = "SIP registration info.";
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(485, 3);
			this.m_pSeparator1.Location = new Point(7, 50);
			this.m_pSeparator1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.mt_VirtualServer = new Label();
			this.mt_VirtualServer.Size = new Size(140, 20);
			this.mt_VirtualServer.Location = new Point(0, 65);
			this.mt_VirtualServer.TextAlign = ContentAlignment.MiddleRight;
			this.mt_VirtualServer.Text = "Virtual Server:";
			this.m_pVirtualServer = new ComboBox();
			this.m_pVirtualServer.Size = new Size(315, 20);
			this.m_pVirtualServer.Location = new Point(145, 65);
			this.m_pVirtualServer.DropDownStyle = ComboBoxStyle.DropDownList;
			this.mt_AOR = new Label();
			this.mt_AOR.Size = new Size(140, 20);
			this.mt_AOR.Location = new Point(0, 90);
			this.mt_AOR.TextAlign = ContentAlignment.MiddleRight;
			this.mt_AOR.Text = "Address of Record:";
			this.m_pAOR = new TextBox();
			this.m_pAOR.Size = new Size(315, 20);
			this.m_pAOR.Location = new Point(145, 90);
			this.m_pGetAOR = new Button();
			this.m_pGetAOR.Size = new Size(25, 20);
			this.m_pGetAOR.Location = new Point(465, 90);
			this.m_pGetAOR.Text = "...";
			this.m_pGetAOR.Click += new EventHandler(this.m_pGetAOR_Click);
			this.m_pGetAOR.Enabled = false;
			this.mt_Contact = new Label();
			this.mt_Contact.Size = new Size(140, 20);
			this.mt_Contact.Location = new Point(0, 115);
			this.mt_Contact.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Contact.Text = "Contact URI:";
			this.m_pContact = new TextBox();
			this.m_pContact.Size = new Size(160, 20);
			this.m_pContact.Location = new Point(145, 115);
			this.m_pExpires = new NumericUpDown();
			this.m_pExpires.Size = new Size(50, 20);
			this.m_pExpires.Location = new Point(310, 115);
			this.m_pExpires.Minimum = 60m;
			this.m_pExpires.Maximum = 9999m;
			this.m_pPriority = new NumericUpDown();
			this.m_pPriority.Size = new Size(45, 20);
			this.m_pPriority.Location = new Point(365, 115);
			this.m_pPriority.DecimalPlaces = 2;
			this.m_pPriority.Minimum = 0.1m;
			this.m_pPriority.Maximum = 1m;
			this.m_pPriority.Value = 1m;
			this.m_pToolbar = new ToolStrip();
			this.m_pToolbar.Location = new Point(430, 113);
			this.m_pToolbar.Size = new Size(60, 25);
			this.m_pToolbar.Dock = DockStyle.None;
			this.m_pToolbar.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pToolbar.BackColor = this.BackColor;
			this.m_pToolbar.Renderer = new ToolBarRendererEx();
			this.m_pToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pToolbar_ItemClicked);
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton.Tag = "add";
			toolStripButton.ToolTipText = "Add";
			this.m_pToolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Enabled = false;
			toolStripButton2.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton2.Tag = "delete";
			toolStripButton2.ToolTipText = "Delete";
			this.m_pToolbar.Items.Add(toolStripButton2);
			this.m_pToolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
			toolStripButton3.Tag = "refresh";
			toolStripButton3.ToolTipText = "Refresh";
			this.m_pToolbar.Items.Add(toolStripButton3);
			this.m_pContacts = new ListView();
			this.m_pContacts.Size = new Size(480, 130);
			this.m_pContacts.Location = new Point(10, 145);
			this.m_pContacts.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pContacts.View = View.Details;
			this.m_pContacts.FullRowSelect = true;
			this.m_pContacts.HideSelection = false;
			this.m_pContacts.Columns.Add("Contact URI", 340);
			this.m_pContacts.Columns.Add("Expires", 60);
			this.m_pContacts.Columns.Add("Priority", 50);
			this.m_pContacts.SelectedIndexChanged += new EventHandler(this.m_pContacts_SelectedIndexChanged);
			this.m_pSeparator2 = new GroupBox();
			this.m_pSeparator2.Size = new Size(485, 4);
			this.m_pSeparator2.Location = new Point(7, 285);
			this.m_pSeparator2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(340, 295);
			this.m_pCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(415, 295);
			this.m_pOk.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.mt_Info);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.mt_VirtualServer);
			base.Controls.Add(this.m_pVirtualServer);
			base.Controls.Add(this.mt_AOR);
			base.Controls.Add(this.m_pAOR);
			base.Controls.Add(this.m_pGetAOR);
			base.Controls.Add(this.mt_Contact);
			base.Controls.Add(this.m_pContact);
			base.Controls.Add(this.m_pExpires);
			base.Controls.Add(this.m_pPriority);
			base.Controls.Add(this.m_pToolbar);
			base.Controls.Add(this.m_pContacts);
			base.Controls.Add(this.m_pSeparator2);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pGetAOR_Click(object sender, EventArgs e)
		{
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				if (this.m_pContact.Text.Length < 3)
				{
					MessageBox.Show(this, "Please specify Contact URI !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				ListViewItem listViewItem = new ListViewItem(this.m_pContact.Text);
				listViewItem.SubItems.Add(this.m_pExpires.Value.ToString());
				listViewItem.SubItems.Add(this.m_pPriority.Value.ToString("f2"));
				listViewItem.Tag = true;
				this.m_pContacts.Items.Add(listViewItem);
				this.m_pContact.Text = "";
				return;
			}
			else
			{
				if (e.ClickedItem.Tag.ToString() == "delete")
				{
					ListViewItem listViewItem2 = this.m_pContacts.SelectedItems[0];
					if (!(bool)listViewItem2.Tag)
					{
						this.m_pContactsToRemove.Add(listViewItem2.Text);
					}
					listViewItem2.Remove();
					return;
				}
				if (e.ClickedItem.Tag.ToString() == "refresh")
				{
					this.LoadContacts();
				}
				return;
			}
		}

		private void m_pContacts_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pContacts.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				return;
			}
			this.m_pToolbar.Items[1].Enabled = false;
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pAOR.Text.Length < 3)
			{
				MessageBox.Show(this, "Please specify Address of Record !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			List<string> list = new List<string>();
			foreach (string current in this.m_pContactsToRemove)
			{
				list.Add("<" + current + ">;expires=0");
			}
			foreach (ListViewItem listViewItem in this.m_pContacts.Items)
			{
				if ((bool)listViewItem.Tag)
				{
					list.Add(string.Concat(new string[]
					{
						"<",
						listViewItem.Text,
						">;expires=",
						listViewItem.SubItems[1].Text,
						";qvalue=",
						listViewItem.SubItems[2].Text.Replace(',', '.')
					}));
				}
			}
			((VirtualServer)((WComboBoxItem)this.m_pVirtualServer.SelectedItem).Tag).SipRegistrations.Set(this.m_pAOR.Text, list.ToArray());
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		private void LoadContacts()
		{
			this.m_pContacts.Items.Clear();
			this.m_pRegistration.Refresh();
			SipRegistrationContact[] contacts = this.m_pRegistration.Contacts;
			for (int i = 0; i < contacts.Length; i++)
			{
				SipRegistrationContact sipRegistrationContact = contacts[i];
				ListViewItem listViewItem = new ListViewItem(sipRegistrationContact.ContactUri);
				listViewItem.SubItems.Add(sipRegistrationContact.Expires.ToString());
				listViewItem.SubItems.Add(sipRegistrationContact.Priority.ToString("f2"));
				listViewItem.Tag = false;
				this.m_pContacts.Items.Add(listViewItem);
			}
		}
	}
}

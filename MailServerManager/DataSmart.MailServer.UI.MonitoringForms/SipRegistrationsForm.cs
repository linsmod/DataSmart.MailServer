using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.MonitoringForms
{
	public class SipRegistrationsForm : Form
	{
		private ToolStrip m_pToolbar;

		private WListView m_pRegistrations;

		private Server m_pServer;

		public SipRegistrationsForm(Server server, WFrame frame)
		{
			this.m_pServer = server;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadData();
		}

		private void InitializeComponent()
		{
			base.Size = new Size(472, 357);
			this.m_pToolbar = new ToolStrip();
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
			ToolStripButton toolStripButton4 = new ToolStripButton();
			toolStripButton4.Enabled = false;
			toolStripButton4.Image = ResManager.GetIcon("viewmessages.ico").ToBitmap();
			toolStripButton4.Tag = "view";
			toolStripButton4.ToolTipText = "View Contacts";
			this.m_pToolbar.Items.Add(toolStripButton4);
			this.m_pRegistrations = new WListView();
			this.m_pRegistrations.Size = new Size(445, 265);
			this.m_pRegistrations.Location = new Point(9, 47);
			this.m_pRegistrations.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pRegistrations.View = View.Details;
			this.m_pRegistrations.FullRowSelect = true;
			this.m_pRegistrations.HideSelection = false;
			this.m_pRegistrations.Columns.Add("User", 120, HorizontalAlignment.Left);
			this.m_pRegistrations.Columns.Add("Address of Record", 360, HorizontalAlignment.Left);
			this.m_pRegistrations.SelectedIndexChanged += new EventHandler(this.m_pRegistrations_SelectedIndexChanged);
			this.m_pRegistrations.DoubleClick += new EventHandler(this.m_pRegistrations_DoubleClick);
			base.Controls.Add(this.m_pRegistrations);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				SipRegistrationForm sipRegistrationForm = new SipRegistrationForm(this.m_pServer);
				if (sipRegistrationForm.ShowDialog(this) == DialogResult.OK)
				{
					SipRegistration sipRegistration = sipRegistrationForm.VirtualServer.SipRegistrations[sipRegistrationForm.AddressOfRecord];
					if (sipRegistration != null)
					{
						ListViewItem listViewItem = new ListViewItem(sipRegistration.UserName);
						listViewItem.SubItems.Add(sipRegistration.AddressOfRecord);
						listViewItem.Tag = sipRegistration;
						this.m_pRegistrations.Items.Add(listViewItem);
						return;
					}
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete")
			{
				SipRegistration sipRegistration2 = (SipRegistration)this.m_pRegistrations.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to remove SIP registration '" + sipRegistration2.AddressOfRecord + "' ?", "Remove Registration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					sipRegistration2.Owner.Remove(sipRegistration2);
					this.m_pRegistrations.SelectedItems[0].Remove();
					return;
				}
			}
			else
			{
				if (e.ClickedItem.Tag.ToString() == "refresh")
				{
					this.LoadData();
					return;
				}
				if (e.ClickedItem.Tag.ToString() == "view")
				{
					this.ViewContacts();
				}
			}
		}

		private void m_pRegistrations_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pRegistrations.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				this.m_pToolbar.Items[4].Enabled = true;
				return;
			}
			this.m_pToolbar.Items[1].Enabled = false;
			this.m_pToolbar.Items[4].Enabled = false;
		}

		private void m_pRegistrations_DoubleClick(object sender, EventArgs e)
		{
			this.ViewContacts();
		}

		private void LoadData()
		{
			this.m_pRegistrations.Items.Clear();
			foreach (VirtualServer virtualServer in this.m_pServer.VirtualServers)
			{
				virtualServer.SipRegistrations.Refresh();
				foreach (SipRegistration sipRegistration in virtualServer.SipRegistrations)
				{
					ListViewItem listViewItem = new ListViewItem(sipRegistration.UserName);
					listViewItem.SubItems.Add(sipRegistration.AddressOfRecord);
					listViewItem.Tag = sipRegistration;
					this.m_pRegistrations.Items.Add(listViewItem);
				}
			}
		}

		private void ViewContacts()
		{
			if (this.m_pRegistrations.SelectedItems.Count > 0)
			{
				SipRegistration registration = (SipRegistration)this.m_pRegistrations.SelectedItems[0].Tag;
				SipRegistrationForm sipRegistrationForm = new SipRegistrationForm(this.m_pServer, registration);
				sipRegistrationForm.ShowDialog(this);
			}
		}
	}
}

using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SystemForms
{
	public class GeneralForm : Form
	{
		private TabControl m_pTab;

		private Button m_pApply;

		private Label mt_TabGeneral_DnsServers;

		private TextBox m_pTabGeneral_DnsServer;

		private ToolStrip m_pTabGeneral_DnsServerToolbar;

		private ListView m_pTabGeneral_DnsServers;

		private NotificationForm m_pTabGeneral_Notification;

		private VirtualServer m_pVirtualServer;

		public GeneralForm(VirtualServer virtualServer)
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
			this.m_pApply = new Button();
			this.m_pApply.Size = new Size(70, 20);
			this.m_pApply.Location = new Point(450, 500);
			this.m_pApply.Text = "Apply";
			this.m_pApply.Click += new EventHandler(this.m_pApply_Click);
			this.m_pTab.TabPages.Add("General");
			this.mt_TabGeneral_DnsServers = new Label();
			this.mt_TabGeneral_DnsServers.Size = new Size(80, 20);
			this.mt_TabGeneral_DnsServers.Location = new Point(0, 10);
			this.mt_TabGeneral_DnsServers.TextAlign = ContentAlignment.MiddleRight;
			this.mt_TabGeneral_DnsServers.Text = "DNS servers:";
			this.m_pTabGeneral_DnsServer = new TextBox();
			this.m_pTabGeneral_DnsServer.Size = new Size(180, 20);
			this.m_pTabGeneral_DnsServer.Location = new Point(85, 10);
			this.m_pTabGeneral_DnsServerToolbar = new ToolStrip();
			this.m_pTabGeneral_DnsServerToolbar.Size = new Size(60, 25);
			this.m_pTabGeneral_DnsServerToolbar.Location = new Point(270, 8);
			this.m_pTabGeneral_DnsServerToolbar.Dock = DockStyle.None;
			this.m_pTabGeneral_DnsServerToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pTabGeneral_DnsServerToolbar.BackColor = this.BackColor;
			this.m_pTabGeneral_DnsServerToolbar.Renderer = new ToolBarRendererEx();
			this.m_pTabGeneral_DnsServerToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pTabGeneral_DnsServerToolbar_ItemClicked);
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton.Tag = "add";
			toolStripButton.ToolTipText = "Add";
			this.m_pTabGeneral_DnsServerToolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Enabled = false;
			toolStripButton2.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton2.Tag = "delete";
			toolStripButton2.ToolTipText = "Delete";
			this.m_pTabGeneral_DnsServerToolbar.Items.Add(toolStripButton2);
			this.m_pTabGeneral_DnsServerToolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Enabled = false;
			toolStripButton3.Image = ResManager.GetIcon("up.ico").ToBitmap();
			toolStripButton3.Tag = "up";
			this.m_pTabGeneral_DnsServerToolbar.Items.Add(toolStripButton3);
			ToolStripButton toolStripButton4 = new ToolStripButton();
			toolStripButton4.Enabled = false;
			toolStripButton4.Image = ResManager.GetIcon("down.ico").ToBitmap();
			toolStripButton4.Tag = "down";
			this.m_pTabGeneral_DnsServerToolbar.Items.Add(toolStripButton4);
			this.m_pTabGeneral_DnsServers = new ListView();
			this.m_pTabGeneral_DnsServers.Size = new Size(355, 100);
			this.m_pTabGeneral_DnsServers.Location = new Point(10, 35);
			this.m_pTabGeneral_DnsServers.View = View.Details;
			this.m_pTabGeneral_DnsServers.FullRowSelect = true;
			this.m_pTabGeneral_DnsServers.HideSelection = false;
			this.m_pTabGeneral_DnsServers.SelectedIndexChanged += new EventHandler(this.m_pTabGeneral_DnsServers_SelectedIndexChanged);
			this.m_pTabGeneral_DnsServers.Columns.Add("IP", 325);
			this.m_pTabGeneral_Notification = new NotificationForm();
			this.m_pTabGeneral_Notification.Size = new Size(485, 38);
			this.m_pTabGeneral_Notification.Location = new Point(10, 421);
			this.m_pTabGeneral_Notification.Icon = ResManager.GetIcon("warning.ico").ToBitmap();
			this.m_pTabGeneral_Notification.Visible = false;
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_DnsServers);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_DnsServer);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_DnsServerToolbar);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_DnsServers);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_Notification);
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

		private void m_pTabGeneral_DnsServerToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag != null)
			{
				if (e.ClickedItem.Tag.ToString() == "add")
				{
					try
					{
						IPAddress.Parse(this.m_pTabGeneral_DnsServer.Text);
					}
					catch
					{
						MessageBox.Show(this, "Invalid DNS server IP address !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}
					ListViewItem value = new ListViewItem(this.m_pTabGeneral_DnsServer.Text);
					this.m_pTabGeneral_DnsServers.Items.Add(value);
					this.m_pTabGeneral_DnsServer.Text = "";
				}
				else if (e.ClickedItem.Tag.ToString() == "delete")
				{
					this.m_pTabGeneral_DnsServers.SelectedItems[0].Remove();
				}
				else if (e.ClickedItem.Tag.ToString() == "up")
				{
					ListViewItem listViewItem = this.m_pTabGeneral_DnsServers.SelectedItems[0];
					ListViewItem item = this.m_pTabGeneral_DnsServers.Items[listViewItem.Index - 1];
					this.m_pTabGeneral_DnsServers.Items.Remove(item);
					this.m_pTabGeneral_DnsServers.Items.Insert(listViewItem.Index + 1, item);
				}
				else if (e.ClickedItem.Tag.ToString() == "down")
				{
					ListViewItem listViewItem2 = this.m_pTabGeneral_DnsServers.SelectedItems[0];
					ListViewItem item2 = this.m_pTabGeneral_DnsServers.Items[listViewItem2.Index + 1];
					this.m_pTabGeneral_DnsServers.Items.Remove(item2);
					this.m_pTabGeneral_DnsServers.Items.Insert(listViewItem2.Index, item2);
				}
			}
			this.m_pTabGeneral_DnsServers_SelectedIndexChanged(this, new EventArgs());
			this.AddNotifications();
		}

		private void m_pTabGeneral_DnsServers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pTabGeneral_DnsServers.SelectedItems.Count <= 0)
			{
				this.m_pTabGeneral_DnsServerToolbar.Items[1].Enabled = false;
				this.m_pTabGeneral_DnsServerToolbar.Items[3].Enabled = false;
				this.m_pTabGeneral_DnsServerToolbar.Items[4].Enabled = false;
				return;
			}
			this.m_pTabGeneral_DnsServerToolbar.Items[1].Enabled = true;
			if (this.m_pTabGeneral_DnsServers.SelectedItems[0].Index > 0)
			{
				this.m_pTabGeneral_DnsServerToolbar.Items[3].Enabled = true;
			}
			else
			{
				this.m_pTabGeneral_DnsServerToolbar.Items[3].Enabled = false;
			}
			if (this.m_pTabGeneral_DnsServers.SelectedItems[0].Index < this.m_pTabGeneral_DnsServers.Items.Count - 1)
			{
				this.m_pTabGeneral_DnsServerToolbar.Items[4].Enabled = true;
				return;
			}
			this.m_pTabGeneral_DnsServerToolbar.Items[4].Enabled = false;
		}

		private void m_pApply_Click(object sender, EventArgs e)
		{
			this.SaveData(false);
		}

		private void m_pTestDns_Click(object sender, EventArgs e)
		{
		}

		private void LoadData()
		{
			try
			{
				SystemSettings systemSettings = this.m_pVirtualServer.SystemSettings;
				IPAddress[] dnsServers = systemSettings.DnsServers;
				for (int i = 0; i < dnsServers.Length; i++)
				{
					IPAddress iPAddress = dnsServers[i];
					ListViewItem value = new ListViewItem(iPAddress.ToString());
					this.m_pTabGeneral_DnsServers.Items.Add(value);
				}
				this.AddNotifications();
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
				SystemSettings systemSettings = this.m_pVirtualServer.SystemSettings;
				List<IPAddress> list = new List<IPAddress>();
				foreach (ListViewItem listViewItem in this.m_pTabGeneral_DnsServers.Items)
				{
					list.Add(IPAddress.Parse(listViewItem.Text));
				}
				systemSettings.DnsServers = list.ToArray();
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

		private void AddNotifications()
		{
			this.m_pTabGeneral_Notification.Visible = false;
			if (this.m_pTabGeneral_DnsServers.Items.Count == 0)
			{
				this.m_pTabGeneral_Notification.Visible = true;
				this.m_pTabGeneral_Notification.Text = "You need to specifiy at least 1 DNS server.\n";
			}
		}
	}
}

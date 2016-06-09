using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System.NetworkToolkit;
using System.NetworkToolkit.SIP.Proxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SystemForms
{
	public class SipServiceForm : Form
	{
		private TabControl m_pTab;

		private Button m_pApply;

		private CheckBox m_pTabGeneral_Enabled;

		private Label mt_TabGeneral_ProxyType;

		private ComboBox m_pTabGeneral_ProxyType;

		private Label mt_TabGeneral_MinExpires;

		private NumericUpDown m_pTabGeneral_MinExpires;

		private Label mt_TabGeneral_Bindings;

		private ToolStrip m_pTabGeneral_BindingsToolbar;

		private ListView m_pTabGeneral_Bindings;

		private ToolStrip m_pTabGateways_GatewaysToolbar;

		private ListView m_pTabGateways_Gateways;

		private VirtualServer m_pVirtualServer;

		public SipServiceForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.LoadData();
		}

		private void InitializeComponent()
		{
			this.m_pTab = new TabControl();
			this.m_pTab.Size = new Size(515, 520);
			this.m_pTab.Location = new Point(5, 0);
			this.m_pTab.TabPages.Add(new TabPage("General"));
			this.m_pTab.TabPages.Add(new TabPage("Gateways"));
			this.m_pApply = new Button();
			this.m_pApply.Size = new Size(70, 20);
			this.m_pApply.Location = new Point(450, 530);
			this.m_pApply.Text = "Apply";
			this.m_pApply.Click += new EventHandler(this.m_pApply_Click);
			base.Controls.Add(this.m_pTab);
			base.Controls.Add(this.m_pApply);
			this.m_pTabGeneral_Enabled = new CheckBox();
			this.m_pTabGeneral_Enabled.Size = new Size(70, 20);
			this.m_pTabGeneral_Enabled.Location = new Point(170, 10);
			this.m_pTabGeneral_Enabled.Text = "Enabled";
			this.mt_TabGeneral_ProxyType = new Label();
			this.mt_TabGeneral_ProxyType.Size = new Size(155, 20);
			this.mt_TabGeneral_ProxyType.Location = new Point(10, 40);
			this.mt_TabGeneral_ProxyType.TextAlign = ContentAlignment.MiddleRight;
			this.mt_TabGeneral_ProxyType.Text = "Proxy Type:";
			this.m_pTabGeneral_ProxyType = new ComboBox();
			this.m_pTabGeneral_ProxyType.Size = new Size(100, 20);
			this.m_pTabGeneral_ProxyType.Location = new Point(170, 40);
			this.m_pTabGeneral_ProxyType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pTabGeneral_ProxyType.Items.Add("B2BUA");
			this.m_pTabGeneral_ProxyType.Items.Add("Statefull");
			this.mt_TabGeneral_MinExpires = new Label();
			this.mt_TabGeneral_MinExpires.Size = new Size(155, 20);
			this.mt_TabGeneral_MinExpires.Location = new Point(10, 65);
			this.mt_TabGeneral_MinExpires.TextAlign = ContentAlignment.MiddleRight;
			this.mt_TabGeneral_MinExpires.Text = "Minimum Expires:";
			this.m_pTabGeneral_MinExpires = new NumericUpDown();
			this.m_pTabGeneral_MinExpires.Size = new Size(70, 20);
			this.m_pTabGeneral_MinExpires.Location = new Point(170, 65);
			this.m_pTabGeneral_MinExpires.Minimum = 60m;
			this.m_pTabGeneral_MinExpires.Maximum = 9999m;
			this.mt_TabGeneral_Bindings = new Label();
			this.mt_TabGeneral_Bindings.Size = new Size(70, 20);
			this.mt_TabGeneral_Bindings.Location = new Point(10, 325);
			this.mt_TabGeneral_Bindings.Text = "IP Bindings:";
			this.m_pTabGeneral_BindingsToolbar = new ToolStrip();
			this.m_pTabGeneral_BindingsToolbar.Size = new Size(95, 25);
			this.m_pTabGeneral_BindingsToolbar.Location = new Point(425, 325);
			this.m_pTabGeneral_BindingsToolbar.Dock = DockStyle.None;
			this.m_pTabGeneral_BindingsToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pTabGeneral_BindingsToolbar.BackColor = this.BackColor;
			this.m_pTabGeneral_BindingsToolbar.Renderer = new ToolBarRendererEx();
			this.m_pTabGeneral_BindingsToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pTabGeneral_BindingsToolbar_ItemClicked);
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton.Tag = "add";
			toolStripButton.ToolTipText = "Add";
			this.m_pTabGeneral_BindingsToolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Enabled = false;
			toolStripButton2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			toolStripButton2.Tag = "edit";
			toolStripButton2.ToolTipText = "edit";
			this.m_pTabGeneral_BindingsToolbar.Items.Add(toolStripButton2);
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Enabled = false;
			toolStripButton3.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton3.Tag = "delete";
			toolStripButton3.ToolTipText = "Delete";
			this.m_pTabGeneral_BindingsToolbar.Items.Add(toolStripButton3);
			this.m_pTabGeneral_Bindings = new ListView();
			this.m_pTabGeneral_Bindings.Size = new Size(485, 100);
			this.m_pTabGeneral_Bindings.Location = new Point(10, 350);
			this.m_pTabGeneral_Bindings.View = View.Details;
			this.m_pTabGeneral_Bindings.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.m_pTabGeneral_Bindings.HideSelection = false;
			this.m_pTabGeneral_Bindings.FullRowSelect = true;
			this.m_pTabGeneral_Bindings.MultiSelect = false;
			this.m_pTabGeneral_Bindings.SelectedIndexChanged += new EventHandler(this.m_pTabGeneral_Bindings_SelectedIndexChanged);
			this.m_pTabGeneral_Bindings.Columns.Add("Host Name", 100, HorizontalAlignment.Left);
			this.m_pTabGeneral_Bindings.Columns.Add("IP", 140, HorizontalAlignment.Left);
			this.m_pTabGeneral_Bindings.Columns.Add("Protocol", 60, HorizontalAlignment.Left);
			this.m_pTabGeneral_Bindings.Columns.Add("Port", 50, HorizontalAlignment.Left);
			this.m_pTabGeneral_Bindings.Columns.Add("SSL", 50, HorizontalAlignment.Left);
			this.m_pTabGeneral_Bindings.Columns.Add("Certificate", 60, HorizontalAlignment.Left);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_Enabled);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_ProxyType);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_ProxyType);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_MinExpires);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_MinExpires);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_Bindings);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_BindingsToolbar);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_Bindings);
			this.m_pTabGateways_GatewaysToolbar = new ToolStrip();
			this.m_pTabGateways_GatewaysToolbar.Dock = DockStyle.None;
			this.m_pTabGateways_GatewaysToolbar.Location = new Point(430, 15);
			this.m_pTabGateways_GatewaysToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pTabGateways_GatewaysToolbar.BackColor = this.BackColor;
			this.m_pTabGateways_GatewaysToolbar.Renderer = new ToolBarRendererEx();
			this.m_pTabGateways_GatewaysToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pTabGateways_GatewaysToolbar_ItemClicked);
			ToolStripButton toolStripButton4 = new ToolStripButton();
			toolStripButton4.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton4.Tag = "add";
			toolStripButton4.ToolTipText = "Add";
			this.m_pTabGateways_GatewaysToolbar.Items.Add(toolStripButton4);
			ToolStripButton toolStripButton5 = new ToolStripButton();
			toolStripButton5.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			toolStripButton5.Tag = "edit";
			toolStripButton5.ToolTipText = "Edit";
			this.m_pTabGateways_GatewaysToolbar.Items.Add(toolStripButton5);
			ToolStripButton toolStripButton6 = new ToolStripButton();
			toolStripButton6.Enabled = false;
			toolStripButton6.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton6.Tag = "delete";
			toolStripButton6.ToolTipText = "Delete";
			this.m_pTabGateways_GatewaysToolbar.Items.Add(toolStripButton6);
			this.m_pTabGateways_Gateways = new ListView();
			this.m_pTabGateways_Gateways.Size = new Size(495, 415);
			this.m_pTabGateways_Gateways.Location = new Point(5, 40);
			this.m_pTabGateways_Gateways.View = View.Details;
			this.m_pTabGateways_Gateways.FullRowSelect = true;
			this.m_pTabGateways_Gateways.HideSelection = false;
			this.m_pTabGateways_Gateways.Columns.Add("URI scheme", 80);
			this.m_pTabGateways_Gateways.Columns.Add("Transport", 60);
			this.m_pTabGateways_Gateways.Columns.Add("Host", 240);
			this.m_pTabGateways_Gateways.Columns.Add("Port", 60);
			this.m_pTabGateways_Gateways.SelectedIndexChanged += new EventHandler(this.m_pTabGateways_Gateways_SelectedIndexChanged);
			this.m_pTabGateways_Gateways.DoubleClick += new EventHandler(this.m_pTabGateways_Gateways_DoubleClick);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTabGateways_GatewaysToolbar);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTabGateways_Gateways);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (!base.Visible)
			{
				this.SaveData(true);
			}
		}

		private void m_pTabGeneral_BindingsToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				AddEditBindInfoForm addEditBindInfoForm = new AddEditBindInfoForm(this.m_pVirtualServer.Server, true, 5060, 5061);
				if (addEditBindInfoForm.ShowDialog(this) == DialogResult.OK)
				{
					ListViewItem listViewItem = new ListViewItem();
					listViewItem.Text = addEditBindInfoForm.HostName;
					if (addEditBindInfoForm.IP.ToString() == "0.0.0.0")
					{
						listViewItem.SubItems.Add("Any IPv4");
					}
					else if (addEditBindInfoForm.IP.ToString() == "0:0:0:0:0:0:0:0")
					{
						listViewItem.SubItems.Add("Any IPv6");
					}
					else
					{
						listViewItem.SubItems.Add(addEditBindInfoForm.IP.ToString());
					}
					listViewItem.SubItems.Add(addEditBindInfoForm.Protocol.ToString());
					listViewItem.SubItems.Add(addEditBindInfoForm.Port.ToString());
					listViewItem.SubItems.Add(addEditBindInfoForm.SslMode.ToString());
					listViewItem.SubItems.Add(Convert.ToString(addEditBindInfoForm.Certificate != null));
					listViewItem.Tag = new IPBindInfo(addEditBindInfoForm.HostName, addEditBindInfoForm.Protocol, addEditBindInfoForm.IP, addEditBindInfoForm.Port, addEditBindInfoForm.SslMode, addEditBindInfoForm.Certificate);
					listViewItem.Selected = true;
					this.m_pTabGeneral_Bindings.Items.Add(listViewItem);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "edit")
			{
				if (this.m_pTabGeneral_Bindings.SelectedItems.Count > 0)
				{
					ListViewItem listViewItem2 = this.m_pTabGeneral_Bindings.SelectedItems[0];
					AddEditBindInfoForm addEditBindInfoForm2 = new AddEditBindInfoForm(this.m_pVirtualServer.Server, true, 5060, 5061, (IPBindInfo)listViewItem2.Tag);
					if (addEditBindInfoForm2.ShowDialog(this) == DialogResult.OK)
					{
						listViewItem2.Text = addEditBindInfoForm2.HostName;
						if (addEditBindInfoForm2.IP.ToString() == "0.0.0.0")
						{
							listViewItem2.SubItems[1].Text = "Any IPv4";
						}
						else if (addEditBindInfoForm2.IP.ToString() == "0:0:0:0:0:0:0:0")
						{
							listViewItem2.SubItems[1].Text = "Any IPv6";
						}
						else
						{
							listViewItem2.SubItems[1].Text = addEditBindInfoForm2.IP.ToString();
						}
						listViewItem2.SubItems[2].Text = addEditBindInfoForm2.Protocol.ToString();
						listViewItem2.SubItems[3].Text = addEditBindInfoForm2.Port.ToString();
						listViewItem2.SubItems[4].Text = addEditBindInfoForm2.SslMode.ToString();
						listViewItem2.SubItems[5].Text = Convert.ToString(addEditBindInfoForm2.Certificate != null);
						listViewItem2.Tag = new IPBindInfo(addEditBindInfoForm2.HostName, addEditBindInfoForm2.Protocol, addEditBindInfoForm2.IP, addEditBindInfoForm2.Port, addEditBindInfoForm2.SslMode, addEditBindInfoForm2.Certificate);
						return;
					}
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete" && this.m_pTabGeneral_Bindings.SelectedItems.Count > 0 && MessageBox.Show(this, string.Concat(new string[]
			{
				"Are you sure you want to delete binding '",
				this.m_pTabGeneral_Bindings.SelectedItems[0].SubItems[0].Text,
				":",
				this.m_pTabGeneral_Bindings.SelectedItems[0].SubItems[1].Text,
				"' ?"
			}), "Confirm:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				this.m_pTabGeneral_Bindings.SelectedItems[0].Remove();
			}
		}

		private void m_pTabGeneral_Bindings_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pTabGeneral_Bindings.SelectedItems.Count > 0)
			{
				this.m_pTabGeneral_BindingsToolbar.Items[1].Enabled = true;
				this.m_pTabGeneral_BindingsToolbar.Items[2].Enabled = true;
				return;
			}
			this.m_pTabGeneral_BindingsToolbar.Items[1].Enabled = false;
			this.m_pTabGeneral_BindingsToolbar.Items[2].Enabled = false;
		}

		private void m_pTabGateways_GatewaysToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				SystemSipServiceGatewayForm systemSipServiceGatewayForm = new SystemSipServiceGatewayForm();
				if (systemSipServiceGatewayForm.ShowDialog(this) == DialogResult.OK)
				{
					SipGateway tag = this.m_pVirtualServer.SystemSettings.SIP.Gateways.Add(systemSipServiceGatewayForm.UriScheme, systemSipServiceGatewayForm.Transport, systemSipServiceGatewayForm.Host, systemSipServiceGatewayForm.Port, systemSipServiceGatewayForm.Realm, systemSipServiceGatewayForm.UserName, systemSipServiceGatewayForm.Password);
					ListViewItem listViewItem = new ListViewItem(systemSipServiceGatewayForm.UriScheme);
					listViewItem.SubItems.Add(systemSipServiceGatewayForm.Transport);
					listViewItem.SubItems.Add(systemSipServiceGatewayForm.Host);
					listViewItem.SubItems.Add(systemSipServiceGatewayForm.Port.ToString());
					listViewItem.Tag = tag;
					this.m_pTabGateways_Gateways.Items.Add(listViewItem);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "edit")
			{
				SipGateway sipGateway = (SipGateway)this.m_pTabGateways_Gateways.SelectedItems[0].Tag;
				SystemSipServiceGatewayForm systemSipServiceGatewayForm2 = new SystemSipServiceGatewayForm(sipGateway.UriScheme, sipGateway.Transport, sipGateway.Host, sipGateway.Port, sipGateway.Realm, sipGateway.UserName, sipGateway.Password);
				if (systemSipServiceGatewayForm2.ShowDialog(this) == DialogResult.OK)
				{
					sipGateway.UriScheme = systemSipServiceGatewayForm2.UriScheme;
					sipGateway.Transport = systemSipServiceGatewayForm2.Transport;
					sipGateway.Host = systemSipServiceGatewayForm2.Host;
					sipGateway.Port = systemSipServiceGatewayForm2.Port;
					sipGateway.Realm = systemSipServiceGatewayForm2.Realm;
					sipGateway.UserName = systemSipServiceGatewayForm2.UserName;
					sipGateway.Password = systemSipServiceGatewayForm2.Password;
					this.m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = systemSipServiceGatewayForm2.UriScheme;
					this.m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = systemSipServiceGatewayForm2.Transport;
					this.m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = systemSipServiceGatewayForm2.Host;
					this.m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = systemSipServiceGatewayForm2.Port.ToString();
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete")
			{
				SipGateway sipGateway2 = (SipGateway)this.m_pTabGateways_Gateways.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to remove SIP selected gateway ?", "Remove Gateway", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					sipGateway2.Remove();
					this.m_pTabGateways_Gateways.SelectedItems[0].Remove();
				}
			}
		}

		private void m_pTabGateways_Gateways_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pTabGateways_Gateways.SelectedItems.Count > 0)
			{
				this.m_pTabGateways_GatewaysToolbar.Items[1].Enabled = true;
				this.m_pTabGateways_GatewaysToolbar.Items[2].Enabled = true;
				return;
			}
			this.m_pTabGateways_GatewaysToolbar.Items[1].Enabled = false;
			this.m_pTabGateways_GatewaysToolbar.Items[2].Enabled = false;
		}

		private void m_pTabGateways_Gateways_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pTabGateways_Gateways.SelectedItems.Count > 0)
			{
				SipGateway sipGateway = (SipGateway)this.m_pTabGateways_Gateways.SelectedItems[0].Tag;
				SystemSipServiceGatewayForm systemSipServiceGatewayForm = new SystemSipServiceGatewayForm(sipGateway.UriScheme, sipGateway.Transport, sipGateway.Host, sipGateway.Port, sipGateway.Realm, sipGateway.UserName, sipGateway.Password);
				if (systemSipServiceGatewayForm.ShowDialog(this) == DialogResult.OK)
				{
					sipGateway.UriScheme = systemSipServiceGatewayForm.UriScheme;
					sipGateway.Transport = systemSipServiceGatewayForm.Transport;
					sipGateway.Host = systemSipServiceGatewayForm.Host;
					sipGateway.Port = systemSipServiceGatewayForm.Port;
					sipGateway.Realm = systemSipServiceGatewayForm.Realm;
					sipGateway.UserName = systemSipServiceGatewayForm.UserName;
					sipGateway.Password = systemSipServiceGatewayForm.Password;
					this.m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = systemSipServiceGatewayForm.UriScheme;
					this.m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = systemSipServiceGatewayForm.Transport;
					this.m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = systemSipServiceGatewayForm.Host;
					this.m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = systemSipServiceGatewayForm.Port.ToString();
				}
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
				SipSettings sIP = this.m_pVirtualServer.SystemSettings.SIP;
				this.m_pTabGeneral_Enabled.Checked = sIP.Enabled;
				if ((sIP.ProxyMode & SIP_ProxyMode.B2BUA) != (SIP_ProxyMode)0)
				{
					this.m_pTabGeneral_ProxyType.SelectedIndex = 0;
				}
				else
				{
					this.m_pTabGeneral_ProxyType.SelectedIndex = 1;
				}
				this.m_pTabGeneral_MinExpires.Value = Convert.ToDecimal(sIP.MinimumExpires);
				IPBindInfo[] binds = sIP.Binds;
				for (int i = 0; i < binds.Length; i++)
				{
					IPBindInfo iPBindInfo = binds[i];
					ListViewItem listViewItem = new ListViewItem();
					listViewItem.Text = iPBindInfo.HostName;
					if (iPBindInfo.IP.ToString() == "0.0.0.0")
					{
						listViewItem.SubItems.Add("Any IPv4");
					}
					else if (iPBindInfo.IP.ToString() == "0:0:0:0:0:0:0:0")
					{
						listViewItem.SubItems.Add("Any IPv6");
					}
					else
					{
						listViewItem.SubItems.Add(iPBindInfo.IP.ToString());
					}
					listViewItem.SubItems.Add(iPBindInfo.Protocol.ToString());
					listViewItem.SubItems.Add(iPBindInfo.Port.ToString());
					listViewItem.SubItems.Add(iPBindInfo.SslMode.ToString());
					listViewItem.SubItems.Add(Convert.ToString(iPBindInfo.Certificate != null));
					listViewItem.Tag = iPBindInfo;
					this.m_pTabGeneral_Bindings.Items.Add(listViewItem);
				}
				foreach (SipGateway sipGateway in this.m_pVirtualServer.SystemSettings.SIP.Gateways)
				{
					ListViewItem listViewItem2 = new ListViewItem(sipGateway.UriScheme);
					listViewItem2.SubItems.Add(sipGateway.Transport);
					listViewItem2.SubItems.Add(sipGateway.Host);
					listViewItem2.SubItems.Add(sipGateway.Port.ToString());
					listViewItem2.Tag = sipGateway;
					this.m_pTabGateways_Gateways.Items.Add(listViewItem2);
				}
				this.m_pTabGateways_Gateways_SelectedIndexChanged(null, null);
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
				SipSettings sIP = this.m_pVirtualServer.SystemSettings.SIP;
				sIP.Enabled = this.m_pTabGeneral_Enabled.Checked;
				if (this.m_pTabGeneral_ProxyType.SelectedIndex == 0)
				{
					sIP.ProxyMode = (SIP_ProxyMode.Registrar | SIP_ProxyMode.B2BUA);
				}
				else
				{
					sIP.ProxyMode = (SIP_ProxyMode.Registrar | SIP_ProxyMode.Statefull);
				}
				sIP.MinimumExpires = (int)this.m_pTabGeneral_MinExpires.Value;
				List<IPBindInfo> list = new List<IPBindInfo>();
				foreach (ListViewItem listViewItem in this.m_pTabGeneral_Bindings.Items)
				{
					list.Add((IPBindInfo)listViewItem.Tag);
				}
				sIP.Binds = list.ToArray();
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

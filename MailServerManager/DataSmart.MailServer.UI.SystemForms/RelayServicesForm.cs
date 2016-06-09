using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System.NetworkToolkit;
using System.NetworkToolkit.SMTP.Relay;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SystemForms
{
	public class RelayServicesForm : Form
	{
		private TabControl m_pTab;

		private Button m_pApply;

		private RadioButton m_pTabGeneral_SendSmartHost;

		private Label mt_TabGeneral_SmartHostsBalanceMode;

		private ComboBox m_pTabGeneral_SmartHostsBalanceMode;

		private ToolStrip m_pTabGeneral_SmartHosts_Toolbar;

		private ListView m_pTabGeneral_SmartHosts;

		private RadioButton m_pTabGeneral_SendDns;

		private Label mt_TabGeneral_SessionTimeout;

		private NumericUpDown m_pTabGeneral_SessionTimeout;

		private Label mt_TabGeneral_SessTimeoutSec;

		private Label mt_TabGeneral_MaxConnections;

		private NumericUpDown m_pTabGeneral_MaxConnections;

		private Label mt_TabGeneral_MaxConnsPerIP;

		private NumericUpDown m_pTabGeneral_MaxConnsPerIP;

		private Label mt_TabGeneral_MaxConnsPerIP0;

		private Label mt_TabGeneral_RelayInterval;

		private NumericUpDown m_pTabGeneral_RelayInterval;

		private Label mt_TabGeneral_RelayIntervalSeconds;

		private Label mt_TabGeneral_RelayRetryInterval;

		private NumericUpDown m_pTabGeneral_RelayRetryInterval;

		private Label mt_TabGeneral_RelayRetryIntervSec;

		private Label mt_TabGeneral_SendUndelWarning;

		private NumericUpDown m_pTabGeneral_SendUndelWarning;

		private Label mt_TabGeneral_SendUndelWarnMinutes;

		private Label mt_TabGeneral_SendUndelivered;

		private NumericUpDown m_pTabGeneral_SendUndelivered;

		private Label mt_TabGeneral_SendUndeliveredHours;

		private CheckBox m_pTabGeneral_StoreUndeliveredMsgs;

		private CheckBox m_pTabGeneral_UseTlsIfPossible;

		private NotificationForm m_pTabGeneral_Notification;

		private ToolStrip m_pTabBindings_BindingsToolbar;

		private ListView m_pTabBindings_Bindings;

		private NotificationForm m_pTabBindings_Notification;

		private VirtualServer m_pVirtualServer;

		public RelayServicesForm(VirtualServer virtualServer)
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
			this.m_pTab.TabPages.Add(new TabPage("IP Bindings"));
			this.m_pApply = new Button();
			this.m_pApply.Size = new Size(70, 20);
			this.m_pApply.Location = new Point(450, 530);
			this.m_pApply.Text = "Apply";
			this.m_pApply.Click += new EventHandler(this.m_pApply_Click);
			this.m_pTabGeneral_SendSmartHost = new RadioButton();
			this.m_pTabGeneral_SendSmartHost.Size = new Size(250, 20);
			this.m_pTabGeneral_SendSmartHost.Location = new Point(10, 15);
			this.m_pTabGeneral_SendSmartHost.Text = "Send mails through SmartHost";
			this.m_pTabGeneral_SendSmartHost.CheckedChanged += new EventHandler(this.m_pTabGeneral_SendSmartHost_CheckedChanged);
			this.mt_TabGeneral_SmartHostsBalanceMode = new Label();
			this.mt_TabGeneral_SmartHostsBalanceMode.Size = new Size(160, 20);
			this.mt_TabGeneral_SmartHostsBalanceMode.Location = new Point(10, 40);
			this.mt_TabGeneral_SmartHostsBalanceMode.TextAlign = ContentAlignment.MiddleRight;
			this.mt_TabGeneral_SmartHostsBalanceMode.Text = "Smart hosts balance mode:";
			this.m_pTabGeneral_SmartHostsBalanceMode = new ComboBox();
			this.m_pTabGeneral_SmartHostsBalanceMode.Size = new Size(100, 20);
			this.m_pTabGeneral_SmartHostsBalanceMode.Location = new Point(180, 40);
			this.m_pTabGeneral_SmartHostsBalanceMode.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pTabGeneral_SmartHostsBalanceMode.Items.Add("Load-balance");
			this.m_pTabGeneral_SmartHostsBalanceMode.Items.Add("Fail-over");
			this.m_pTabGeneral_SmartHostsBalanceMode.SelectedIndex = 0;
			this.m_pTabGeneral_SmartHosts_Toolbar = new ToolStrip();
			this.m_pTabGeneral_SmartHosts_Toolbar.Size = new Size(95, 25);
			this.m_pTabGeneral_SmartHosts_Toolbar.Location = new Point(373, 40);
			this.m_pTabGeneral_SmartHosts_Toolbar.Dock = DockStyle.None;
			this.m_pTabGeneral_SmartHosts_Toolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pTabGeneral_SmartHosts_Toolbar.BackColor = this.BackColor;
			this.m_pTabGeneral_SmartHosts_Toolbar.Renderer = new ToolBarRendererEx();
			this.m_pTabGeneral_SmartHosts_Toolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pTabGeneral_SmartHosts_Toolbar_ItemClicked);
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton.Tag = "add";
			toolStripButton.ToolTipText = "Add";
			this.m_pTabGeneral_SmartHosts_Toolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Enabled = false;
			toolStripButton2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			toolStripButton2.Tag = "edit";
			toolStripButton2.ToolTipText = "edit";
			this.m_pTabGeneral_SmartHosts_Toolbar.Items.Add(toolStripButton2);
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Enabled = false;
			toolStripButton3.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton3.Tag = "delete";
			toolStripButton3.ToolTipText = "Delete";
			this.m_pTabGeneral_SmartHosts_Toolbar.Items.Add(toolStripButton3);
			this.m_pTabGeneral_SmartHosts_Toolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton4 = new ToolStripButton();
			toolStripButton4.Enabled = false;
			toolStripButton4.Image = ResManager.GetIcon("up.ico").ToBitmap();
			toolStripButton4.Tag = "up";
			this.m_pTabGeneral_SmartHosts_Toolbar.Items.Add(toolStripButton4);
			ToolStripButton toolStripButton5 = new ToolStripButton();
			toolStripButton5.Enabled = false;
			toolStripButton5.Image = ResManager.GetIcon("down.ico").ToBitmap();
			toolStripButton5.Tag = "down";
			this.m_pTabGeneral_SmartHosts_Toolbar.Items.Add(toolStripButton5);
			this.m_pTabGeneral_SmartHosts = new ListView();
			this.m_pTabGeneral_SmartHosts.Size = new Size(465, 100);
			this.m_pTabGeneral_SmartHosts.Location = new Point(30, 65);
			this.m_pTabGeneral_SmartHosts.View = View.Details;
			this.m_pTabGeneral_SmartHosts.FullRowSelect = true;
			this.m_pTabGeneral_SmartHosts.HideSelection = false;
			this.m_pTabGeneral_SmartHosts.SelectedIndexChanged += new EventHandler(this.m_pTabGeneral_SmartHosts_SelectedIndexChanged);
			this.m_pTabGeneral_SmartHosts.Columns.Add("Host", 200);
			this.m_pTabGeneral_SmartHosts.Columns.Add("Port", 60);
			this.m_pTabGeneral_SmartHosts.Columns.Add("SSL Mode", 80);
			this.m_pTabGeneral_SmartHosts.Columns.Add("User Name", 100);
			this.m_pTabGeneral_SendDns = new RadioButton();
			this.m_pTabGeneral_SendDns.Size = new Size(250, 20);
			this.m_pTabGeneral_SendDns.Location = new Point(10, 180);
			this.m_pTabGeneral_SendDns.Text = "Send mails directly using DNS";
			this.m_pTabGeneral_SendDns.CheckedChanged += new EventHandler(this.m_pTabGeneral_SendSmartHost_CheckedChanged);
			this.mt_TabGeneral_SessionTimeout = new Label();
			this.mt_TabGeneral_SessionTimeout.Size = new Size(200, 20);
			this.mt_TabGeneral_SessionTimeout.Location = new Point(10, 210);
			this.mt_TabGeneral_SessionTimeout.TextAlign = ContentAlignment.MiddleRight;
			this.mt_TabGeneral_SessionTimeout.Text = "Session Idle Timeout:";
			this.m_pTabGeneral_SessionTimeout = new NumericUpDown();
			this.m_pTabGeneral_SessionTimeout.Size = new Size(70, 20);
			this.m_pTabGeneral_SessionTimeout.Location = new Point(215, 210);
			this.m_pTabGeneral_SessionTimeout.Minimum = 10m;
			this.m_pTabGeneral_SessionTimeout.Maximum = 99999m;
			this.mt_TabGeneral_SessTimeoutSec = new Label();
			this.mt_TabGeneral_SessTimeoutSec.Size = new Size(70, 20);
			this.mt_TabGeneral_SessTimeoutSec.Location = new Point(290, 210);
			this.mt_TabGeneral_SessTimeoutSec.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_TabGeneral_SessTimeoutSec.Text = "seconds";
			this.mt_TabGeneral_MaxConnections = new Label();
			this.mt_TabGeneral_MaxConnections.Size = new Size(200, 20);
			this.mt_TabGeneral_MaxConnections.Location = new Point(10, 240);
			this.mt_TabGeneral_MaxConnections.TextAlign = ContentAlignment.MiddleRight;
			this.mt_TabGeneral_MaxConnections.Text = "Maximum Connections:";
			this.m_pTabGeneral_MaxConnections = new NumericUpDown();
			this.m_pTabGeneral_MaxConnections.Size = new Size(70, 20);
			this.m_pTabGeneral_MaxConnections.Location = new Point(215, 240);
			this.m_pTabGeneral_MaxConnections.Minimum = 1m;
			this.m_pTabGeneral_MaxConnections.Maximum = 99999m;
			this.mt_TabGeneral_MaxConnsPerIP = new Label();
			this.mt_TabGeneral_MaxConnsPerIP.Size = new Size(200, 20);
			this.mt_TabGeneral_MaxConnsPerIP.Location = new Point(1, 265);
			this.mt_TabGeneral_MaxConnsPerIP.TextAlign = ContentAlignment.MiddleRight;
			this.mt_TabGeneral_MaxConnsPerIP.Text = "Maximum Connections per IP:";
			this.m_pTabGeneral_MaxConnsPerIP = new NumericUpDown();
			this.m_pTabGeneral_MaxConnsPerIP.Size = new Size(70, 20);
			this.m_pTabGeneral_MaxConnsPerIP.Location = new Point(215, 265);
			this.m_pTabGeneral_MaxConnsPerIP.Minimum = 0m;
			this.m_pTabGeneral_MaxConnsPerIP.Maximum = 99999m;
			this.mt_TabGeneral_MaxConnsPerIP0 = new Label();
			this.mt_TabGeneral_MaxConnsPerIP0.Size = new Size(164, 20);
			this.mt_TabGeneral_MaxConnsPerIP0.Location = new Point(290, 265);
			this.mt_TabGeneral_MaxConnsPerIP0.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_TabGeneral_MaxConnsPerIP0.Text = "(0 for unlimited)";
			this.mt_TabGeneral_RelayInterval = new Label();
			this.mt_TabGeneral_RelayInterval.Size = new Size(200, 20);
			this.mt_TabGeneral_RelayInterval.Location = new Point(10, 290);
			this.mt_TabGeneral_RelayInterval.TextAlign = ContentAlignment.MiddleRight;
			this.mt_TabGeneral_RelayInterval.Text = "Relay Interval:";
			this.m_pTabGeneral_RelayInterval = new NumericUpDown();
			this.m_pTabGeneral_RelayInterval.Size = new Size(70, 20);
			this.m_pTabGeneral_RelayInterval.Location = new Point(215, 290);
			this.m_pTabGeneral_RelayInterval.Minimum = 1m;
			this.m_pTabGeneral_RelayInterval.Maximum = 9999m;
			this.mt_TabGeneral_RelayIntervalSeconds = new Label();
			this.mt_TabGeneral_RelayIntervalSeconds.Size = new Size(50, 20);
			this.mt_TabGeneral_RelayIntervalSeconds.Location = new Point(290, 290);
			this.mt_TabGeneral_RelayIntervalSeconds.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_TabGeneral_RelayIntervalSeconds.Text = "seconds";
			this.mt_TabGeneral_RelayRetryInterval = new Label();
			this.mt_TabGeneral_RelayRetryInterval.Size = new Size(200, 20);
			this.mt_TabGeneral_RelayRetryInterval.Location = new Point(10, 315);
			this.mt_TabGeneral_RelayRetryInterval.TextAlign = ContentAlignment.MiddleRight;
			this.mt_TabGeneral_RelayRetryInterval.Text = "Relay Retry Interval:";
			this.m_pTabGeneral_RelayRetryInterval = new NumericUpDown();
			this.m_pTabGeneral_RelayRetryInterval.Size = new Size(70, 20);
			this.m_pTabGeneral_RelayRetryInterval.Location = new Point(215, 315);
			this.m_pTabGeneral_RelayRetryInterval.Minimum = 1m;
			this.m_pTabGeneral_RelayRetryInterval.Maximum = 9999m;
			this.mt_TabGeneral_RelayRetryIntervSec = new Label();
			this.mt_TabGeneral_RelayRetryIntervSec.Size = new Size(50, 20);
			this.mt_TabGeneral_RelayRetryIntervSec.Location = new Point(290, 315);
			this.mt_TabGeneral_RelayRetryIntervSec.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_TabGeneral_RelayRetryIntervSec.Text = "seconds";
			this.mt_TabGeneral_SendUndelWarning = new Label();
			this.mt_TabGeneral_SendUndelWarning.Size = new Size(200, 20);
			this.mt_TabGeneral_SendUndelWarning.Location = new Point(10, 345);
			this.mt_TabGeneral_SendUndelWarning.TextAlign = ContentAlignment.MiddleRight;
			this.mt_TabGeneral_SendUndelWarning.Text = "Send undelivered warning after:";
			this.m_pTabGeneral_SendUndelWarning = new NumericUpDown();
			this.m_pTabGeneral_SendUndelWarning.Size = new Size(70, 20);
			this.m_pTabGeneral_SendUndelWarning.Location = new Point(215, 345);
			this.m_pTabGeneral_SendUndelWarning.Minimum = 1m;
			this.m_pTabGeneral_SendUndelWarning.Maximum = 9999m;
			this.mt_TabGeneral_SendUndelWarnMinutes = new Label();
			this.mt_TabGeneral_SendUndelWarnMinutes.Size = new Size(50, 20);
			this.mt_TabGeneral_SendUndelWarnMinutes.Location = new Point(290, 345);
			this.mt_TabGeneral_SendUndelWarnMinutes.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_TabGeneral_SendUndelWarnMinutes.Text = "minutes";
			this.mt_TabGeneral_SendUndelivered = new Label();
			this.mt_TabGeneral_SendUndelivered.Size = new Size(200, 20);
			this.mt_TabGeneral_SendUndelivered.Location = new Point(10, 370);
			this.mt_TabGeneral_SendUndelivered.TextAlign = ContentAlignment.MiddleRight;
			this.mt_TabGeneral_SendUndelivered.Text = "Send undelivered after:";
			this.m_pTabGeneral_SendUndelivered = new NumericUpDown();
			this.m_pTabGeneral_SendUndelivered.Size = new Size(70, 20);
			this.m_pTabGeneral_SendUndelivered.Location = new Point(215, 370);
			this.m_pTabGeneral_SendUndelivered.Minimum = 1m;
			this.m_pTabGeneral_SendUndelivered.Maximum = 999m;
			this.mt_TabGeneral_SendUndeliveredHours = new Label();
			this.mt_TabGeneral_SendUndeliveredHours.Size = new Size(50, 20);
			this.mt_TabGeneral_SendUndeliveredHours.Location = new Point(290, 370);
			this.mt_TabGeneral_SendUndeliveredHours.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_TabGeneral_SendUndeliveredHours.Text = "hours";
			this.m_pTabGeneral_StoreUndeliveredMsgs = new CheckBox();
			this.m_pTabGeneral_StoreUndeliveredMsgs.Size = new Size(250, 20);
			this.m_pTabGeneral_StoreUndeliveredMsgs.Location = new Point(215, 395);
			this.m_pTabGeneral_StoreUndeliveredMsgs.Text = "Store undelivered messages";
			this.m_pTabGeneral_UseTlsIfPossible = new CheckBox();
			this.m_pTabGeneral_UseTlsIfPossible.Size = new Size(250, 20);
			this.m_pTabGeneral_UseTlsIfPossible.Location = new Point(215, 415);
			this.m_pTabGeneral_UseTlsIfPossible.Text = "Use TLS if possible";
			this.m_pTabGeneral_Notification = new NotificationForm();
			this.m_pTabGeneral_Notification.Size = new Size(485, 38);
			this.m_pTabGeneral_Notification.Location = new Point(10, 440);
			this.m_pTabGeneral_Notification.Icon = ResManager.GetIcon("warning.ico").ToBitmap();
			this.m_pTabGeneral_Notification.Visible = false;
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_SendSmartHost);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_SmartHostsBalanceMode);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_SmartHostsBalanceMode);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_SmartHosts_Toolbar);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_SmartHosts);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_SendDns);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_SessionTimeout);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_SessionTimeout);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_SessTimeoutSec);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_MaxConnections);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_MaxConnections);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_MaxConnsPerIP);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_MaxConnsPerIP);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_MaxConnsPerIP0);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_RelayInterval);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_RelayInterval);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_RelayIntervalSeconds);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_RelayRetryInterval);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_RelayRetryInterval);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_RelayRetryIntervSec);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_SendUndelWarning);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_SendUndelWarning);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_SendUndelWarnMinutes);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_SendUndelivered);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_SendUndelivered);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_SendUndeliveredHours);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_StoreUndeliveredMsgs);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_UseTlsIfPossible);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_Notification);
			this.m_pTabBindings_BindingsToolbar = new ToolStrip();
			this.m_pTabBindings_BindingsToolbar.Size = new Size(95, 25);
			this.m_pTabBindings_BindingsToolbar.Location = new Point(425, 13);
			this.m_pTabBindings_BindingsToolbar.Dock = DockStyle.None;
			this.m_pTabBindings_BindingsToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pTabBindings_BindingsToolbar.BackColor = this.BackColor;
			this.m_pTabBindings_BindingsToolbar.Renderer = new ToolBarRendererEx();
			this.m_pTabBindings_BindingsToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pTabBindings_BindingsToolbar_ItemClicked);
			ToolStripButton toolStripButton6 = new ToolStripButton();
			toolStripButton6.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton6.Tag = "add";
			toolStripButton6.ToolTipText = "Add";
			this.m_pTabBindings_BindingsToolbar.Items.Add(toolStripButton6);
			ToolStripButton toolStripButton7 = new ToolStripButton();
			toolStripButton7.Enabled = false;
			toolStripButton7.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			toolStripButton7.Tag = "edit";
			toolStripButton7.ToolTipText = "edit";
			this.m_pTabBindings_BindingsToolbar.Items.Add(toolStripButton7);
			ToolStripButton toolStripButton8 = new ToolStripButton();
			toolStripButton8.Enabled = false;
			toolStripButton8.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton8.Tag = "delete";
			toolStripButton8.ToolTipText = "Delete";
			this.m_pTabBindings_BindingsToolbar.Items.Add(toolStripButton8);
			this.m_pTabBindings_Bindings = new ListView();
			this.m_pTabBindings_Bindings.Size = new Size(485, 375);
			this.m_pTabBindings_Bindings.Location = new Point(10, 40);
			this.m_pTabBindings_Bindings.View = View.Details;
			this.m_pTabBindings_Bindings.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.m_pTabBindings_Bindings.HideSelection = false;
			this.m_pTabBindings_Bindings.FullRowSelect = true;
			this.m_pTabBindings_Bindings.MultiSelect = false;
			this.m_pTabBindings_Bindings.SelectedIndexChanged += new EventHandler(this.m_pTabBindings_Bindings_SelectedIndexChanged);
			this.m_pTabBindings_Bindings.Columns.Add("Host Name", 250, HorizontalAlignment.Left);
			this.m_pTabBindings_Bindings.Columns.Add("IP", 190, HorizontalAlignment.Left);
			this.m_pTabBindings_Notification = new NotificationForm();
			this.m_pTabBindings_Notification.Size = new Size(485, 38);
			this.m_pTabBindings_Notification.Location = new Point(10, 421);
			this.m_pTabBindings_Notification.Icon = ResManager.GetIcon("warning.ico").ToBitmap();
			this.m_pTabBindings_Notification.Visible = false;
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTabBindings_BindingsToolbar);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTabBindings_Bindings);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTabBindings_Notification);
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

		private void m_pTabGeneral_SendSmartHost_CheckedChanged(object sender, EventArgs e)
		{
			if (this.m_pTabGeneral_SendSmartHost.Checked)
			{
				this.m_pTabGeneral_SmartHostsBalanceMode.Enabled = true;
				this.m_pTabGeneral_SmartHosts_Toolbar.Enabled = true;
				this.m_pTabGeneral_SmartHosts.Enabled = true;
			}
			else
			{
				this.m_pTabGeneral_SmartHostsBalanceMode.Enabled = false;
				this.m_pTabGeneral_SmartHosts_Toolbar.Enabled = false;
				this.m_pTabGeneral_SmartHosts.Enabled = false;
			}
			this.AddNotifications();
		}

		private void m_pTabGeneral_SmartHosts_Toolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag != null)
			{
				if (e.ClickedItem.Tag.ToString() == "add")
				{
					AddEditSmartHostForm addEditSmartHostForm = new AddEditSmartHostForm();
					if (addEditSmartHostForm.ShowDialog(this) == DialogResult.OK)
					{
						ListViewItem listViewItem = new ListViewItem();
						listViewItem.Text = addEditSmartHostForm.Host;
						listViewItem.SubItems.Add(addEditSmartHostForm.Port.ToString());
						listViewItem.SubItems.Add(addEditSmartHostForm.SslMode.ToString());
						listViewItem.SubItems.Add(addEditSmartHostForm.UserName);
						listViewItem.Tag = new Relay_SmartHost(addEditSmartHostForm.Host, addEditSmartHostForm.Port, addEditSmartHostForm.SslMode, addEditSmartHostForm.UserName, addEditSmartHostForm.Password);
						this.m_pTabGeneral_SmartHosts.Items.Add(listViewItem);
					}
				}
				else if (e.ClickedItem.Tag.ToString() == "edit")
				{
					if (this.m_pTabGeneral_SmartHosts.SelectedItems.Count > 0)
					{
						ListViewItem listViewItem2 = this.m_pTabGeneral_SmartHosts.SelectedItems[0];
						AddEditSmartHostForm addEditSmartHostForm2 = new AddEditSmartHostForm((Relay_SmartHost)listViewItem2.Tag);
						if (addEditSmartHostForm2.ShowDialog(this) == DialogResult.OK)
						{
							listViewItem2.Text = addEditSmartHostForm2.Host;
							listViewItem2.SubItems[1].Text = addEditSmartHostForm2.Port.ToString();
							listViewItem2.SubItems[2].Text = addEditSmartHostForm2.SslMode.ToString();
							listViewItem2.SubItems[3].Text = addEditSmartHostForm2.UserName;
							listViewItem2.Tag = new Relay_SmartHost(addEditSmartHostForm2.Host, addEditSmartHostForm2.Port, addEditSmartHostForm2.SslMode, addEditSmartHostForm2.UserName, addEditSmartHostForm2.Password);
						}
					}
				}
				else if (e.ClickedItem.Tag.ToString() == "delete")
				{
					if (this.m_pTabGeneral_SmartHosts.SelectedItems.Count > 0 && MessageBox.Show(this, "Are you sure you want to delete smart host '" + this.m_pTabGeneral_SmartHosts.SelectedItems[0].Text + "' ?", "Confirm:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						this.m_pTabGeneral_SmartHosts.SelectedItems[0].Remove();
					}
				}
				else if (e.ClickedItem.Tag.ToString() == "up")
				{
					if (this.m_pTabGeneral_SmartHosts.SelectedItems.Count > 0 && this.m_pTabGeneral_SmartHosts.SelectedItems[0].Index > 0)
					{
						ListViewItem listViewItem3 = this.m_pTabGeneral_SmartHosts.SelectedItems[0];
						ListViewItem item = this.m_pTabGeneral_SmartHosts.Items[listViewItem3.Index - 1];
						this.m_pTabGeneral_SmartHosts.Items.Remove(item);
						this.m_pTabGeneral_SmartHosts.Items.Insert(listViewItem3.Index + 1, item);
					}
				}
				else if (e.ClickedItem.Tag.ToString() == "down" && this.m_pTabGeneral_SmartHosts.SelectedItems.Count > 0 && this.m_pTabGeneral_SmartHosts.SelectedItems[0].Index < this.m_pTabGeneral_SmartHosts.Items.Count - 1)
				{
					ListViewItem listViewItem4 = this.m_pTabGeneral_SmartHosts.SelectedItems[0];
					ListViewItem item2 = this.m_pTabGeneral_SmartHosts.Items[listViewItem4.Index + 1];
					this.m_pTabGeneral_SmartHosts.Items.Remove(item2);
					this.m_pTabGeneral_SmartHosts.Items.Insert(listViewItem4.Index, item2);
				}
			}
			this.m_pTabGeneral_SmartHosts_SelectedIndexChanged(this, new EventArgs());
			this.AddNotifications();
		}

		private void m_pTabGeneral_SmartHosts_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pTabGeneral_SmartHosts.SelectedItems.Count <= 0)
			{
				this.m_pTabGeneral_SmartHosts_Toolbar.Items[1].Enabled = false;
				this.m_pTabGeneral_SmartHosts_Toolbar.Items[2].Enabled = false;
				this.m_pTabGeneral_SmartHosts_Toolbar.Items[4].Enabled = false;
				this.m_pTabGeneral_SmartHosts_Toolbar.Items[5].Enabled = false;
				return;
			}
			this.m_pTabGeneral_SmartHosts_Toolbar.Items[1].Enabled = true;
			this.m_pTabGeneral_SmartHosts_Toolbar.Items[2].Enabled = true;
			if (this.m_pTabGeneral_SmartHosts.SelectedItems[0].Index > 0)
			{
				this.m_pTabGeneral_SmartHosts_Toolbar.Items[4].Enabled = true;
			}
			else
			{
				this.m_pTabGeneral_SmartHosts_Toolbar.Items[4].Enabled = false;
			}
			if (this.m_pTabGeneral_SmartHosts.SelectedItems[0].Index < this.m_pTabGeneral_SmartHosts.Items.Count - 1)
			{
				this.m_pTabGeneral_SmartHosts_Toolbar.Items[5].Enabled = true;
				return;
			}
			this.m_pTabGeneral_SmartHosts_Toolbar.Items[5].Enabled = false;
		}

		private void m_pTabBindings_BindingsToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				AddEditBindInfoForm addEditBindInfoForm = new AddEditBindInfoForm(this.m_pVirtualServer.Server, false, false, false, 0, 0, null);
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
					this.m_pTabBindings_Bindings.Items.Add(listViewItem);
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "edit")
			{
				if (this.m_pTabBindings_Bindings.SelectedItems.Count > 0)
				{
					ListViewItem listViewItem2 = this.m_pTabBindings_Bindings.SelectedItems[0];
					AddEditBindInfoForm addEditBindInfoForm2 = new AddEditBindInfoForm(this.m_pVirtualServer.Server, false, false, false, 0, 0, (IPBindInfo)listViewItem2.Tag);
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
						listViewItem2.SubItems[2].Text = addEditBindInfoForm2.Port.ToString();
						listViewItem2.SubItems[3].Text = addEditBindInfoForm2.SslMode.ToString();
						listViewItem2.SubItems[4].Text = Convert.ToString(addEditBindInfoForm2.Certificate != null);
						listViewItem2.Tag = new IPBindInfo(addEditBindInfoForm2.HostName, addEditBindInfoForm2.Protocol, addEditBindInfoForm2.IP, addEditBindInfoForm2.Port, addEditBindInfoForm2.SslMode, addEditBindInfoForm2.Certificate);
					}
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete" && this.m_pTabBindings_Bindings.SelectedItems.Count > 0 && MessageBox.Show(this, string.Concat(new string[]
			{
				"Are you sure you want to delete binding '",
				this.m_pTabBindings_Bindings.SelectedItems[0].SubItems[0].Text,
				":",
				this.m_pTabBindings_Bindings.SelectedItems[0].SubItems[1].Text,
				"' ?"
			}), "Confirm:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				this.m_pTabBindings_Bindings.SelectedItems[0].Remove();
			}
			this.AddNotifications();
		}

		private void m_pTabBindings_Bindings_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pTabBindings_Bindings.SelectedItems.Count > 0)
			{
				this.m_pTabBindings_BindingsToolbar.Items[1].Enabled = true;
				this.m_pTabBindings_BindingsToolbar.Items[2].Enabled = true;
				return;
			}
			this.m_pTabBindings_BindingsToolbar.Items[1].Enabled = false;
			this.m_pTabBindings_BindingsToolbar.Items[2].Enabled = false;
		}

		private void m_pApply_Click(object sender, EventArgs e)
		{
			this.SaveData(false);
		}

		private void LoadData()
		{
			try
			{
				Relay_Settings relay = this.m_pVirtualServer.SystemSettings.Relay;
				if (relay.RelayMode == Relay_Mode.SmartHost)
				{
					this.m_pTabGeneral_SendSmartHost.Checked = true;
				}
				else
				{
					this.m_pTabGeneral_SendDns.Checked = true;
				}
				if (relay.SmartHostsBalanceMode == BalanceMode.LoadBalance)
				{
					this.m_pTabGeneral_SmartHostsBalanceMode.SelectedIndex = 0;
				}
				else
				{
					this.m_pTabGeneral_SmartHostsBalanceMode.SelectedIndex = 1;
				}
				this.m_pTabGeneral_SessionTimeout.Value = relay.SessionIdleTimeOut;
				this.m_pTabGeneral_MaxConnections.Value = relay.MaximumConnections;
				this.m_pTabGeneral_MaxConnsPerIP.Value = relay.MaximumConnectionsPerIP;
				this.m_pTabGeneral_RelayInterval.Value = relay.RelayInterval;
				this.m_pTabGeneral_RelayRetryInterval.Value = relay.RelayRetryInterval;
				this.m_pTabGeneral_SendUndelWarning.Value = relay.SendUndeliveredWarningAfter;
				this.m_pTabGeneral_SendUndelivered.Value = relay.SendUndeliveredAfter;
				this.m_pTabGeneral_StoreUndeliveredMsgs.Checked = relay.StoreUndeliveredMessages;
				this.m_pTabGeneral_UseTlsIfPossible.Checked = relay.UseTlsIfPossible;
				Relay_SmartHost[] smartHosts = relay.SmartHosts;
				for (int i = 0; i < smartHosts.Length; i++)
				{
					Relay_SmartHost relay_SmartHost = smartHosts[i];
					ListViewItem listViewItem = new ListViewItem();
					listViewItem.Text = relay_SmartHost.Host;
					listViewItem.SubItems.Add(relay_SmartHost.Port.ToString());
					listViewItem.SubItems.Add(relay_SmartHost.SslMode.ToString());
					listViewItem.SubItems.Add(relay_SmartHost.UserName);
					listViewItem.Tag = relay_SmartHost;
					this.m_pTabGeneral_SmartHosts.Items.Add(listViewItem);
				}
				IPBindInfo[] binds = relay.Binds;
				for (int j = 0; j < binds.Length; j++)
				{
					IPBindInfo iPBindInfo = binds[j];
					ListViewItem listViewItem2 = new ListViewItem();
					listViewItem2.Text = iPBindInfo.HostName;
					if (iPBindInfo.IP.ToString() == "0.0.0.0")
					{
						listViewItem2.SubItems.Add("Any IPv4");
					}
					else if (iPBindInfo.IP.ToString() == "0:0:0:0:0:0:0:0")
					{
						listViewItem2.SubItems.Add("Any IPv6");
					}
					else
					{
						listViewItem2.SubItems.Add(iPBindInfo.IP.ToString());
					}
					listViewItem2.SubItems.Add(iPBindInfo.Port.ToString());
					listViewItem2.SubItems.Add(iPBindInfo.SslMode.ToString());
					listViewItem2.SubItems.Add(Convert.ToString(iPBindInfo.Certificate != null));
					listViewItem2.Tag = iPBindInfo;
					this.m_pTabBindings_Bindings.Items.Add(listViewItem2);
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
				Relay_Settings relay = this.m_pVirtualServer.SystemSettings.Relay;
				if (this.m_pTabGeneral_SendSmartHost.Checked)
				{
					relay.RelayMode = Relay_Mode.SmartHost;
				}
				else
				{
					relay.RelayMode = Relay_Mode.Dns;
				}
				if (this.m_pTabGeneral_SmartHostsBalanceMode.SelectedIndex == 0)
				{
					relay.SmartHostsBalanceMode = BalanceMode.LoadBalance;
				}
				else
				{
					relay.SmartHostsBalanceMode = BalanceMode.FailOver;
				}
				relay.SessionIdleTimeOut = (int)this.m_pTabGeneral_SessionTimeout.Value;
				relay.MaximumConnections = (int)this.m_pTabGeneral_MaxConnections.Value;
				relay.MaximumConnectionsPerIP = (int)this.m_pTabGeneral_MaxConnsPerIP.Value;
				relay.RelayInterval = (int)this.m_pTabGeneral_RelayInterval.Value;
				relay.RelayRetryInterval = (int)this.m_pTabGeneral_RelayRetryInterval.Value;
				relay.SendUndeliveredWarningAfter = (int)this.m_pTabGeneral_SendUndelWarning.Value;
				relay.SendUndeliveredAfter = (int)this.m_pTabGeneral_SendUndelivered.Value;
				relay.StoreUndeliveredMessages = this.m_pTabGeneral_StoreUndeliveredMsgs.Checked;
				relay.UseTlsIfPossible = this.m_pTabGeneral_UseTlsIfPossible.Checked;
				List<Relay_SmartHost> list = new List<Relay_SmartHost>();
				foreach (ListViewItem listViewItem in this.m_pTabGeneral_SmartHosts.Items)
				{
					list.Add((Relay_SmartHost)listViewItem.Tag);
				}
				relay.SmartHosts = list.ToArray();
				List<IPBindInfo> list2 = new List<IPBindInfo>();
				foreach (ListViewItem listViewItem2 in this.m_pTabBindings_Bindings.Items)
				{
					list2.Add((IPBindInfo)listViewItem2.Tag);
				}
				relay.Binds = list2.ToArray();
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
			if (this.m_pTabGeneral_SendSmartHost.Checked && this.m_pTabGeneral_SmartHosts.Items.Count == 0)
			{
				this.m_pTabGeneral_Notification.Visible = true;
				this.m_pTabGeneral_Notification.Text = "Relay needs at least 1 Smart Host to function.\n";
			}
			this.m_pTabBindings_Notification.Visible = false;
			this.m_pTabBindings_Notification.Text = "";
			if (this.m_pTabBindings_Bindings.Items.Count == 0)
			{
				this.m_pTabBindings_Notification.Visible = true;
				this.m_pTabBindings_Notification.Text = "Relay needs at least 1 IP Binding(hostname, ip address) to function.\n";
			}
			foreach (ListViewItem listViewItem in this.m_pTabBindings_Bindings.Items)
			{
				if (listViewItem.Text == "")
				{
					this.m_pTabBindings_Notification.Visible = true;
					NotificationForm expr_D4 = this.m_pTabBindings_Notification;
					expr_D4.Text += "Host Name is missing one or more binding(s).\n";
					break;
				}
			}
		}
	}
}

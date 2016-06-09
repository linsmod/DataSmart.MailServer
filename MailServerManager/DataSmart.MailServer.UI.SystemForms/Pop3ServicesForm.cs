using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System.NetworkToolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SystemForms
{
	public class Pop3ServicesForm : Form
	{
		private TabControl m_pTab;

		private Button m_pApply;

		private CheckBox m_pEnabled;

		private Label mt_GreetingText;

		private TextBox m_pGreetingText;

		private Label mt_SessionTimeout;

		private NumericUpDown m_pSessionTimeout;

		private Label mt_SessTimeoutSec;

		private Label mt_MaxConnections;

		private Label mt_MaxConnsPerIP;

		private NumericUpDown m_pMaxConnsPerIP;

		private Label mt_MaxConnsPerIP0;

		private NumericUpDown m_pMaxConnections;

		private Label mt_MaxBadCommands;

		private NumericUpDown m_pMaxBadCommands;

		private Label mt_TabGeneral_Bindings;

		private ToolStrip m_pTabGeneral_BindingsToolbar;

		private ListView m_pTabGeneral_Bindings;

		private VirtualServer m_pVirtualServer;

		public Pop3ServicesForm(VirtualServer virtualServer)
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
			this.m_pApply = new Button();
			this.m_pApply.Size = new Size(70, 20);
			this.m_pApply.Location = new Point(450, 530);
			this.m_pApply.Text = "Apply";
			this.m_pApply.Click += new EventHandler(this.m_pApply_Click);
			this.m_pEnabled = new CheckBox();
			this.m_pEnabled.Size = new Size(70, 20);
			this.m_pEnabled.Location = new Point(170, 10);
			this.m_pEnabled.Text = "Enabled";
			this.mt_GreetingText = new Label();
			this.mt_GreetingText.Size = new Size(155, 20);
			this.mt_GreetingText.Location = new Point(10, 65);
			this.mt_GreetingText.TextAlign = ContentAlignment.MiddleRight;
			this.mt_GreetingText.Text = "Greeting Text:";
			this.m_pGreetingText = new TextBox();
			this.m_pGreetingText.Size = new Size(250, 20);
			this.m_pGreetingText.Location = new Point(170, 65);
			this.mt_SessionTimeout = new Label();
			this.mt_SessionTimeout.Size = new Size(155, 20);
			this.mt_SessionTimeout.Location = new Point(10, 130);
			this.mt_SessionTimeout.TextAlign = ContentAlignment.MiddleRight;
			this.mt_SessionTimeout.Text = "Session Idle Timeout:";
			this.m_pSessionTimeout = new NumericUpDown();
			this.m_pSessionTimeout.Size = new Size(70, 20);
			this.m_pSessionTimeout.Location = new Point(170, 130);
			this.m_pSessionTimeout.Minimum = 10m;
			this.m_pSessionTimeout.Maximum = 99999m;
			this.mt_SessTimeoutSec = new Label();
			this.mt_SessTimeoutSec.Size = new Size(25, 20);
			this.mt_SessTimeoutSec.Location = new Point(245, 130);
			this.mt_SessTimeoutSec.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_SessTimeoutSec.Text = "sec.";
			this.mt_MaxConnections = new Label();
			this.mt_MaxConnections.Size = new Size(155, 20);
			this.mt_MaxConnections.Location = new Point(10, 170);
			this.mt_MaxConnections.TextAlign = ContentAlignment.MiddleRight;
			this.mt_MaxConnections.Text = "Maximum Connections:";
			this.m_pMaxConnections = new NumericUpDown();
			this.m_pMaxConnections.Size = new Size(70, 20);
			this.m_pMaxConnections.Location = new Point(170, 170);
			this.m_pMaxConnections.Minimum = 1m;
			this.m_pMaxConnections.Maximum = 99999m;
			this.mt_MaxConnsPerIP = new Label();
			this.mt_MaxConnsPerIP.Size = new Size(164, 20);
			this.mt_MaxConnsPerIP.Location = new Point(1, 195);
			this.mt_MaxConnsPerIP.TextAlign = ContentAlignment.MiddleRight;
			this.mt_MaxConnsPerIP.Text = "Maximum Connections per IP:";
			this.m_pMaxConnsPerIP = new NumericUpDown();
			this.m_pMaxConnsPerIP.Size = new Size(70, 20);
			this.m_pMaxConnsPerIP.Location = new Point(170, 195);
			this.m_pMaxConnsPerIP.Minimum = 0m;
			this.m_pMaxConnsPerIP.Maximum = 99999m;
			this.mt_MaxConnsPerIP0 = new Label();
			this.mt_MaxConnsPerIP0.Size = new Size(164, 20);
			this.mt_MaxConnsPerIP0.Location = new Point(245, 195);
			this.mt_MaxConnsPerIP0.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_MaxConnsPerIP0.Text = "(0 for unlimited)";
			this.mt_MaxBadCommands = new Label();
			this.mt_MaxBadCommands.Size = new Size(155, 20);
			this.mt_MaxBadCommands.Location = new Point(10, 220);
			this.mt_MaxBadCommands.TextAlign = ContentAlignment.MiddleRight;
			this.mt_MaxBadCommands.Text = "Maximum Bad Commands:";
			this.m_pMaxBadCommands = new NumericUpDown();
			this.m_pMaxBadCommands.Size = new Size(70, 20);
			this.m_pMaxBadCommands.Location = new Point(170, 220);
			this.m_pMaxBadCommands.Minimum = 1m;
			this.m_pMaxBadCommands.Maximum = 99999m;
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
			this.m_pTabGeneral_Bindings.Columns.Add("Host Name", 130, HorizontalAlignment.Left);
			this.m_pTabGeneral_Bindings.Columns.Add("IP", 140, HorizontalAlignment.Left);
			this.m_pTabGeneral_Bindings.Columns.Add("Port", 50, HorizontalAlignment.Left);
			this.m_pTabGeneral_Bindings.Columns.Add("SSL", 50, HorizontalAlignment.Left);
			this.m_pTabGeneral_Bindings.Columns.Add("Certificate", 60, HorizontalAlignment.Left);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pEnabled);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_GreetingText);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pGreetingText);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_SessionTimeout);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pSessionTimeout);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_SessTimeoutSec);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_MaxConnections);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pMaxConnections);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_MaxConnsPerIP);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pMaxConnsPerIP);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_MaxConnsPerIP0);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_MaxBadCommands);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pMaxBadCommands);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_TabGeneral_Bindings);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_BindingsToolbar);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTabGeneral_Bindings);
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

		private void m_pTabGeneral_BindingsToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				AddEditBindInfoForm addEditBindInfoForm = new AddEditBindInfoForm(this.m_pVirtualServer.Server, true, WellKnownPorts.POP3, WellKnownPorts.POP3_SSL);
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
					AddEditBindInfoForm addEditBindInfoForm2 = new AddEditBindInfoForm(this.m_pVirtualServer.Server, false, WellKnownPorts.POP3, WellKnownPorts.POP3_SSL, (IPBindInfo)listViewItem2.Tag);
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

		private void m_pApply_Click(object sender, EventArgs e)
		{
			this.SaveData(false);
		}

		private void LoadData()
		{
			try
			{
				POP3_Settings pOP = this.m_pVirtualServer.SystemSettings.POP3;
				this.m_pEnabled.Checked = pOP.Enabled;
				this.m_pGreetingText.Text = pOP.GreetingText;
				this.m_pSessionTimeout.Value = pOP.SessionIdleTimeOut;
				this.m_pMaxConnections.Value = pOP.MaximumConnections;
				this.m_pMaxConnsPerIP.Value = pOP.MaximumConnectionsPerIP;
				this.m_pMaxBadCommands.Value = pOP.MaximumBadCommands;
				IPBindInfo[] binds = pOP.Binds;
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
					listViewItem.SubItems.Add(iPBindInfo.Port.ToString());
					listViewItem.SubItems.Add(iPBindInfo.SslMode.ToString());
					listViewItem.SubItems.Add(Convert.ToString(iPBindInfo.Certificate != null));
					listViewItem.Tag = iPBindInfo;
					this.m_pTabGeneral_Bindings.Items.Add(listViewItem);
				}
				this.m_pTabGeneral_Bindings_SelectedIndexChanged(this, new EventArgs());
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
				POP3_Settings pOP = this.m_pVirtualServer.SystemSettings.POP3;
				pOP.Enabled = this.m_pEnabled.Checked;
				pOP.GreetingText = this.m_pGreetingText.Text;
				pOP.SessionIdleTimeOut = (int)this.m_pSessionTimeout.Value;
				pOP.MaximumConnections = (int)this.m_pMaxConnections.Value;
				pOP.MaximumConnectionsPerIP = (int)this.m_pMaxConnsPerIP.Value;
				pOP.MaximumBadCommands = (int)this.m_pMaxBadCommands.Value;
				List<IPBindInfo> list = new List<IPBindInfo>();
				foreach (ListViewItem listViewItem in this.m_pTabGeneral_Bindings.Items)
				{
					list.Add((IPBindInfo)listViewItem.Tag);
				}
				pOP.Binds = list.ToArray();
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

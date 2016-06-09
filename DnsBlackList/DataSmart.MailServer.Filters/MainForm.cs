using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DataSmart.MailServer.Filters
{
	public class MainForm : Form
	{
		private TabControl m_pTab;

		private GroupBox m_pGroupBox1;

		private Button m_pCancel;

		private Button m_pOk;

		private CheckBox m_pGeneral_CheckHelo;

		private CheckBox m_pGeneral_LogRejections;

		private Label mt_BlackList_ErrorText;

		private TextBox m_pBlackList_ErrorText;

		private TextBox m_pBlackList_IP;

		private Button m_pBlackList_Add;

		private ListBox m_pBlackList_IPs;

		private Button m_pBlackList_Remove;

		private ListView m_pServers;

		private Button m_pAdd;

		private Button m_pDelete;

		private Button m_pMoveUp;

		private Button m_pMoveDown;

		private TabPage tabPage1;

		private TabPage tabPage2;

		private TabPage tabPage3;

		private DataSet m_pDsSettings;

		public MainForm()
		{
			this.InitializeComponent();
			this.LoadSettings();
		}

		private void InitializeComponent()
		{
			this.m_pTab = new TabControl();
			this.tabPage1 = new TabPage();
			this.tabPage2 = new TabPage();
			this.tabPage3 = new TabPage();
			this.m_pGeneral_CheckHelo = new CheckBox();
			this.m_pGeneral_LogRejections = new CheckBox();
			this.mt_BlackList_ErrorText = new Label();
			this.m_pBlackList_ErrorText = new TextBox();
			this.m_pBlackList_IP = new TextBox();
			this.m_pBlackList_Add = new Button();
			this.m_pBlackList_IPs = new ListBox();
			this.m_pBlackList_Remove = new Button();
			this.m_pServers = new ListView();
			this.m_pAdd = new Button();
			this.m_pDelete = new Button();
			this.m_pMoveUp = new Button();
			this.m_pMoveDown = new Button();
			this.m_pGroupBox1 = new GroupBox();
			this.m_pCancel = new Button();
			this.m_pOk = new Button();
			this.m_pTab.SuspendLayout();
			base.SuspendLayout();
			this.m_pTab.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTab.Controls.Add(this.tabPage1);
			this.m_pTab.Controls.Add(this.tabPage2);
			this.m_pTab.Controls.Add(this.tabPage3);
			this.m_pTab.Location = new Point(3, 3);
			this.m_pTab.Name = "m_pTab";
			this.m_pTab.SelectedIndex = 0;
			this.m_pTab.Size = new Size(457, 236);
			this.m_pTab.TabIndex = 0;
			this.tabPage1.Location = new Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new Size(449, 194);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "General";
			this.tabPage1.Visible = false;
			this.tabPage2.Location = new Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new Size(449, 194);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Black List";
			this.tabPage2.Visible = false;
			this.tabPage3.Location = new Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new Size(449, 210);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "DNSBL";
			this.tabPage3.Visible = false;
			this.m_pGeneral_CheckHelo.Location = new Point(20, 20);
			this.m_pGeneral_CheckHelo.Name = "m_pGeneral_CheckHelo";
			this.m_pGeneral_CheckHelo.Size = new Size(350, 20);
			this.m_pGeneral_CheckHelo.TabIndex = 0;
			this.m_pGeneral_CheckHelo.Text = "Check HELO/EHLO name for non authenticated users";
			this.m_pGeneral_LogRejections.Location = new Point(20, 40);
			this.m_pGeneral_LogRejections.Name = "m_pGeneral_LogRejections";
			this.m_pGeneral_LogRejections.Size = new Size(350, 20);
			this.m_pGeneral_LogRejections.TabIndex = 0;
			this.m_pGeneral_LogRejections.Text = "Log rejections";
			this.mt_BlackList_ErrorText.Location = new Point(0, 20);
			this.mt_BlackList_ErrorText.Name = "mt_BlackList_ErrorText";
			this.mt_BlackList_ErrorText.Size = new Size(70, 20);
			this.mt_BlackList_ErrorText.TabIndex = 0;
			this.mt_BlackList_ErrorText.Text = "Error Text:";
			this.mt_BlackList_ErrorText.TextAlign = ContentAlignment.MiddleRight;
			this.m_pBlackList_ErrorText.Location = new Point(75, 20);
			this.m_pBlackList_ErrorText.Name = "m_pBlackList_ErrorText";
			this.m_pBlackList_ErrorText.Size = new Size(370, 20);
			this.m_pBlackList_ErrorText.TabIndex = 0;
			this.m_pBlackList_IP.Location = new Point(75, 45);
			this.m_pBlackList_IP.Name = "m_pBlackList_IP";
			this.m_pBlackList_IP.Size = new Size(200, 20);
			this.m_pBlackList_IP.TabIndex = 0;
			this.m_pBlackList_Add.Location = new Point(280, 45);
			this.m_pBlackList_Add.Name = "m_pBlackList_Add";
			this.m_pBlackList_Add.Size = new Size(70, 20);
			this.m_pBlackList_Add.TabIndex = 0;
			this.m_pBlackList_Add.Text = "Add";
			this.m_pBlackList_Add.Click += new EventHandler(this.m_pBlackList_Add_Click);
			this.m_pBlackList_IPs.Location = new Point(75, 70);
			this.m_pBlackList_IPs.Name = "m_pBlackList_IPs";
			this.m_pBlackList_IPs.Size = new Size(200, 130);
			this.m_pBlackList_IPs.TabIndex = 0;
			this.m_pBlackList_IPs.SelectedIndexChanged += new EventHandler(this.m_pBlackList_IPs_SelectedIndexChanged);
			this.m_pBlackList_Remove.Location = new Point(280, 70);
			this.m_pBlackList_Remove.Name = "m_pBlackList_Remove";
			this.m_pBlackList_Remove.Size = new Size(70, 20);
			this.m_pBlackList_Remove.TabIndex = 0;
			this.m_pBlackList_Remove.Text = "Remove";
			this.m_pBlackList_Remove.Click += new EventHandler(this.m_pBlackList_Remove_Click);
			this.m_pServers.FullRowSelect = true;
			this.m_pServers.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.m_pServers.HideSelection = false;
			this.m_pServers.Location = new Point(10, 20);
			this.m_pServers.MultiSelect = false;
			this.m_pServers.Name = "m_pServers";
			this.m_pServers.Size = new Size(350, 150);
			this.m_pServers.TabIndex = 0;
			this.m_pServers.UseCompatibleStateImageBehavior = false;
			this.m_pServers.View = View.Details;
			this.m_pServers.SelectedIndexChanged += new EventHandler(this.m_pServers_SelectedIndexChanged);
			this.m_pAdd.Location = new Point(370, 20);
			this.m_pAdd.Name = "m_pAdd";
			this.m_pAdd.Size = new Size(70, 20);
			this.m_pAdd.TabIndex = 0;
			this.m_pAdd.Text = "Add";
			this.m_pAdd.Click += new EventHandler(this.m_pAdd_Click);
			this.m_pDelete.Location = new Point(370, 45);
			this.m_pDelete.Name = "m_pDelete";
			this.m_pDelete.Size = new Size(70, 20);
			this.m_pDelete.TabIndex = 0;
			this.m_pDelete.Text = "Delete";
			this.m_pDelete.Click += new EventHandler(this.m_pDelete_Click);
			this.m_pMoveUp.Location = new Point(370, 85);
			this.m_pMoveUp.Name = "m_pMoveUp";
			this.m_pMoveUp.Size = new Size(70, 20);
			this.m_pMoveUp.TabIndex = 0;
			this.m_pMoveUp.Text = "Up";
			this.m_pMoveUp.Click += new EventHandler(this.m_pMoveUp_Click);
			this.m_pMoveDown.Location = new Point(370, 110);
			this.m_pMoveDown.Name = "m_pMoveDown";
			this.m_pMoveDown.Size = new Size(70, 20);
			this.m_pMoveDown.TabIndex = 0;
			this.m_pMoveDown.Text = "Down";
			this.m_pMoveDown.Click += new EventHandler(this.m_pMoveDown_Click);
			this.m_pGroupBox1.Location = new Point(5, 235);
			this.m_pGroupBox1.Name = "m_pGroupBox1";
			this.m_pGroupBox1.Size = new Size(455, 4);
			this.m_pGroupBox1.TabIndex = 1;
			this.m_pGroupBox1.TabStop = false;
			this.m_pCancel.Location = new Point(305, 245);
			this.m_pCancel.Name = "m_pCancel";
			this.m_pCancel.Size = new Size(70, 23);
			this.m_pCancel.TabIndex = 2;
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk.Location = new Point(380, 245);
			this.m_pOk.Name = "m_pOk";
			this.m_pOk.Size = new Size(70, 23);
			this.m_pOk.TabIndex = 3;
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.ClientSize = new Size(454, 277);
			base.Controls.Add(this.m_pTab);
			base.Controls.Add(this.m_pGroupBox1);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.Name = "wfrm_Main";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Mail Server DNSBL Filter Settings";
			this.m_pTab.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void m_pBlackList_Add_Click(object sender, EventArgs e)
		{
			foreach (string text in this.m_pBlackList_IPs.Items)
			{
				if (text.ToLower() == this.m_pBlackList_IP.Text.ToLower())
				{
					this.m_pBlackList_IP.Text = "";
					return;
				}
			}
			this.m_pBlackList_IPs.Items.Add(this.m_pBlackList_IP.Text);
			this.m_pBlackList_IP.Text = "";
		}

		private void m_pBlackList_IPs_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pBlackList_IPs.SelectedItems.Count > 0)
			{
				this.m_pBlackList_Remove.Enabled = true;
				return;
			}
			this.m_pBlackList_Remove.Enabled = false;
		}

		private void m_pBlackList_Remove_Click(object sender, EventArgs e)
		{
			while (this.m_pBlackList_IPs.SelectedItems.Count > 0)
			{
				this.m_pBlackList_IPs.Items.Remove(this.m_pBlackList_IPs.SelectedItems[0]);
			}
		}

		private void m_pServers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pServers.SelectedItems.Count > 0)
			{
				this.m_pDelete.Enabled = true;
				if (this.m_pServers.Items.Count > 0)
				{
					if (this.m_pServers.SelectedItems[0].Index > 0)
					{
						this.m_pMoveUp.Enabled = true;
					}
					if (this.m_pServers.SelectedItems[0].Index < this.m_pServers.Items.Count - 1)
					{
						this.m_pMoveDown.Enabled = true;
						return;
					}
				}
			}
			else
			{
				this.m_pDelete.Enabled = false;
				this.m_pMoveUp.Enabled = false;
				this.m_pMoveDown.Enabled = false;
			}
		}

		private void m_pAdd_Click(object sender, EventArgs e)
		{
			DnsBlackListForm dnsBlackListForm = new DnsBlackListForm();
			if (dnsBlackListForm.ShowDialog() == DialogResult.OK)
			{
				DataRow dataRow = this.m_pDsSettings.Tables["Servers"].NewRow();
				dataRow["Cost"] = DateTime.Now.Ticks;
				dataRow["Server"] = dnsBlackListForm.Server;
				dataRow["DefaultRejectionText"] = dnsBlackListForm.DefaultRejectionText;
				this.m_pDsSettings.Tables["Servers"].Rows.Add(dataRow);
				ListViewItem listViewItem = new ListViewItem();
				listViewItem.Tag = dataRow;
				listViewItem.Text = dnsBlackListForm.Server;
				listViewItem.SubItems.Add(dnsBlackListForm.DefaultRejectionText);
				this.m_pServers.Items.Add(listViewItem);
			}
		}

		private void m_pDelete_Click(object sender, EventArgs e)
		{
			if (this.m_pServers.SelectedItems.Count > 0)
			{
				((DataRow)this.m_pServers.SelectedItems[0].Tag).Delete();
				this.m_pServers.SelectedItems[0].Remove();
			}
		}

		private void m_pMoveUp_Click(object sender, EventArgs e)
		{
			if (this.m_pServers.SelectedItems.Count > 0 && this.m_pServers.SelectedItems[0].Index > 0)
			{
				this.SwapItems(this.m_pServers.SelectedItems[0], this.m_pServers.Items[this.m_pServers.SelectedItems[0].Index - 1]);
			}
		}

		private void m_pMoveDown_Click(object sender, EventArgs e)
		{
			if (this.m_pServers.SelectedItems.Count > 0 && this.m_pServers.SelectedItems[0].Index < this.m_pServers.Items.Count - 1)
			{
				this.SwapItems(this.m_pServers.SelectedItems[0], this.m_pServers.Items[this.m_pServers.SelectedItems[0].Index + 1]);
			}
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			this.SaveSettings();
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		private void LoadSettings()
		{
			this.m_pDsSettings = new DataSet();
			this.m_pDsSettings.Tables.Add("General");
			this.m_pDsSettings.Tables["General"].Columns.Add("CheckHelo");
			this.m_pDsSettings.Tables["General"].Columns.Add("LogRejections");
			this.m_pDsSettings.Tables.Add("BlackListSettings");
			this.m_pDsSettings.Tables["BlackListSettings"].Columns.Add("ErrorText");
			this.m_pDsSettings.Tables.Add("BlackList");
			this.m_pDsSettings.Tables["BlackList"].Columns.Add("IP");
			this.m_pDsSettings.Tables.Add("Servers");
			this.m_pDsSettings.Tables["Servers"].Columns.Add("Cost");
			this.m_pDsSettings.Tables["Servers"].Columns.Add("Server");
			this.m_pDsSettings.Tables["Servers"].Columns.Add("DefaultRejectionText");
			if (File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\DnsBlackList.xml"))
			{
				this.m_pDsSettings.ReadXml(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\DnsBlackList.xml");
			}
			if (this.m_pDsSettings.Tables["General"].Rows.Count == 0)
			{
				DataRow dataRow = this.m_pDsSettings.Tables["General"].NewRow();
				dataRow["CheckHelo"] = false;
				dataRow["LogRejections"] = false;
				this.m_pDsSettings.Tables["General"].Rows.Add(dataRow);
			}
			this.m_pGeneral_CheckHelo.Checked = ConvertEx.ToBoolean(this.m_pDsSettings.Tables["General"].Rows[0]["CheckHelo"]);
			this.m_pGeneral_LogRejections.Checked = ConvertEx.ToBoolean(this.m_pDsSettings.Tables["General"].Rows[0]["LogRejections"]);
			if (this.m_pDsSettings.Tables["BlackListSettings"].Rows.Count == 0)
			{
				DataRow dataRow2 = this.m_pDsSettings.Tables["BlackListSettings"].NewRow();
				dataRow2["ErrorText"] = "Your IP is in server black list !";
				this.m_pDsSettings.Tables["BlackListSettings"].Rows.Add(dataRow2);
			}
			this.m_pBlackList_ErrorText.Text = this.m_pDsSettings.Tables["BlackListSettings"].Rows[0]["ErrorText"].ToString();
			foreach (DataRow dataRow3 in this.m_pDsSettings.Tables["BlackList"].Rows)
			{
				this.m_pBlackList_IPs.Items.Add(dataRow3["IP"].ToString());
			}
			foreach (DataRow dataRow4 in this.m_pDsSettings.Tables["Servers"].Rows)
			{
				ListViewItem listViewItem = new ListViewItem();
				listViewItem.Tag = dataRow4;
				listViewItem.Text = dataRow4["Server"].ToString();
				listViewItem.SubItems.Add(dataRow4["DefaultRejectionText"].ToString());
				this.m_pServers.Items.Add(listViewItem);
			}
			this.m_pServers_SelectedIndexChanged(this, new EventArgs());
		}

		private void SaveSettings()
		{
			this.m_pDsSettings.Tables["General"].Rows[0]["CheckHelo"] = this.m_pGeneral_CheckHelo.Checked;
			this.m_pDsSettings.Tables["General"].Rows[0]["LogRejections"] = this.m_pGeneral_LogRejections.Checked;
			this.m_pDsSettings.WriteXml(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\DnsBlackList.xml");
		}

		private void SwapItems(ListViewItem item1, ListViewItem item2)
		{
			DataRow dataRow = (DataRow)item1.Tag;
			DataRow dataRow2 = (DataRow)item2.Tag;
			string value = dataRow["Cost"].ToString();
			string text = dataRow["Server"].ToString();
			string text2 = dataRow["DefaultRejectionText"].ToString();
			dataRow["Cost"] = dataRow2["Cost"];
			dataRow["Server"] = dataRow2["Server"];
			dataRow["DefaultRejectionText"] = dataRow2["DefaultRejectionText"];
			dataRow2["Cost"] = value;
			dataRow2["Server"] = text;
			dataRow2["DefaultRejectionText"] = text2;
			item1.Text = item2.Text;
			item1.SubItems[1].Text = item2.SubItems[1].Text;
			item2.Text = text;
			item2.SubItems[1].Text = text2;
			if (item1.Selected)
			{
				item2.Selected = true;
				return;
			}
			if (item2.Selected)
			{
				item1.Selected = true;
			}
		}
	}
}

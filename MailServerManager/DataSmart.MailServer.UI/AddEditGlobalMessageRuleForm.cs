using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System.NetworkToolkit;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class AddEditGlobalMessageRuleForm : Form
	{
		private TabControl m_pTab;

		private TabPage m_pTab_General;

		private TabPage m_pTab_Actions;

		private Button m_pHelp;

		private Button m_pCancel;

		private Button m_pOk;

		private PictureBox m_pTab_General_Icon;

		private Label mt_Tab_General_Info;

		private GroupBox m_pTab_General_Separator1;

		private CheckBox m_pTab_General_Enabled;

		private Label mt_Tab_General_Description;

		private TextBox m_pTab_General_Description;

		private Label mt_Tab_General_CheckNextRule;

		private ComboBox m_pTab_General_CheckNextRule;

		private Label mt_Tab_General_MatchExpression;

		private ToolStrip m_pTab_General_MatchExprToolbar;

		private WRichTextBox m_pTab_General_MatchExpression;

		private Button m_pTab_General_Create;

		private PictureBox m_pTab_Actions_Icon;

		private Label mt_Tab_Actions_Info;

		private GroupBox m_pTab_Actions_Separator1;

		private ToolStrip m_pTab_Actions_ActionsToolbar;

		private ListView m_pTab_Actions_Actions;

		private VirtualServer m_pVirtualServer;

		private GlobalMessageRule m_pRule;

		public AddEditGlobalMessageRuleForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.m_pTab.TabPages.Remove(this.m_pTab_Actions);
			this.m_pTab_General_MatchExpression.Height -= 25;
			this.m_pTab_General_Create.Visible = true;
		}

		public AddEditGlobalMessageRuleForm(VirtualServer virtualServer, GlobalMessageRule rule)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pRule = rule;
			this.InitializeComponent();
			this.m_pTab_General_Enabled.Checked = rule.Enabled;
			if (rule.CheckNextRule == GlobalMessageRule_CheckNextRule.Always)
			{
				this.m_pTab_General_CheckNextRule.SelectedIndex = 0;
			}
			else if (rule.CheckNextRule == GlobalMessageRule_CheckNextRule.IfMatches)
			{
				this.m_pTab_General_CheckNextRule.SelectedIndex = 1;
			}
			else if (rule.CheckNextRule == GlobalMessageRule_CheckNextRule.IfNotMatches)
			{
				this.m_pTab_General_CheckNextRule.SelectedIndex = 2;
			}
			this.m_pTab_General_Description.Text = rule.Description;
			this.m_pTab_General_MatchExpression.Text = rule.MatchExpression;
			this.m_pTab_General_MatchExpression_TextChanged(this, new EventArgs());
			this.LoadActions();
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(492, 373);
			this.MinimumSize = new Size(500, 400);
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Add/Edit Global Message Rule";
			base.Icon = ResManager.GetIcon("rule.ico");
			this.m_pTab = new TabControl();
			this.m_pTab.Size = new Size(493, 335);
			this.m_pTab.Location = new Point(0, 5);
			this.m_pTab.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTab.TabPages.Add(new TabPage("General"));
			this.m_pTab_General = this.m_pTab.TabPages[0];
			this.m_pTab_General.Size = new Size(485, 309);
			this.m_pTab.TabPages.Add(new TabPage("Actions"));
			this.m_pTab_Actions = this.m_pTab.TabPages[1];
			this.m_pTab_Actions.Size = new Size(485, 309);
			this.m_pHelp = new Button();
			this.m_pHelp.Size = new Size(70, 21);
			this.m_pHelp.Location = new Point(10, 350);
			this.m_pHelp.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.m_pHelp.Click += new EventHandler(this.m_pHelp_Click);
			this.m_pHelp.Text = "Help";
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 21);
			this.m_pCancel.Location = new Point(335, 350);
			this.m_pCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pCancel.Text = "Cancel";
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(71, 21);
			this.m_pOk.Location = new Point(410, 350);
			this.m_pOk.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			this.m_pOk.Text = "Ok";
			base.Controls.Add(this.m_pTab);
			base.Controls.Add(this.m_pHelp);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
			this.m_pTab_General_Icon = new PictureBox();
			this.m_pTab_General_Icon.Size = new Size(32, 32);
			this.m_pTab_General_Icon.Location = new Point(10, 10);
			this.m_pTab_General_Icon.Image = ResManager.GetIcon("rule.ico").ToBitmap();
			this.mt_Tab_General_Info = new Label();
			this.mt_Tab_General_Info.Size = new Size(200, 32);
			this.mt_Tab_General_Info.Location = new Point(50, 10);
			this.mt_Tab_General_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_General_Info.Text = "Specify global message rule info.";
			this.m_pTab_General_Separator1 = new GroupBox();
			this.m_pTab_General_Separator1.Size = new Size(475, 3);
			this.m_pTab_General_Separator1.Location = new Point(7, 50);
			this.m_pTab_General_Separator1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTab_General_Enabled = new CheckBox();
			this.m_pTab_General_Enabled.Size = new Size(100, 20);
			this.m_pTab_General_Enabled.Location = new Point(105, 60);
			this.m_pTab_General_Enabled.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
			this.m_pTab_General_Enabled.Text = "Enabled";
			this.m_pTab_General_Enabled.Checked = true;
			this.mt_Tab_General_Description = new Label();
			this.mt_Tab_General_Description.Size = new Size(100, 20);
			this.mt_Tab_General_Description.Location = new Point(0, 85);
			this.mt_Tab_General_Description.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Tab_General_Description.Text = "Description:";
			this.m_pTab_General_Description = new TextBox();
			this.m_pTab_General_Description.Size = new Size(365, 20);
			this.m_pTab_General_Description.Location = new Point(105, 85);
			this.m_pTab_General_Description.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.mt_Tab_General_CheckNextRule = new Label();
			this.mt_Tab_General_CheckNextRule.Size = new Size(100, 20);
			this.mt_Tab_General_CheckNextRule.Location = new Point(0, 110);
			this.mt_Tab_General_CheckNextRule.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
			this.mt_Tab_General_CheckNextRule.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Tab_General_CheckNextRule.Text = "Check Next Rule:";
			this.m_pTab_General_CheckNextRule = new ComboBox();
			this.m_pTab_General_CheckNextRule.Size = new Size(160, 20);
			this.m_pTab_General_CheckNextRule.Location = new Point(105, 110);
			this.m_pTab_General_CheckNextRule.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
			this.m_pTab_General_CheckNextRule.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pTab_General_CheckNextRule.Items.Add(new WComboBoxItem("Always", GlobalMessageRule_CheckNextRule.Always));
			this.m_pTab_General_CheckNextRule.Items.Add(new WComboBoxItem("If this rule matches", GlobalMessageRule_CheckNextRule.IfMatches));
			this.m_pTab_General_CheckNextRule.Items.Add(new WComboBoxItem("If this rule does not match", GlobalMessageRule_CheckNextRule.IfNotMatches));
			this.m_pTab_General_CheckNextRule.SelectedIndex = 0;
			this.mt_Tab_General_MatchExpression = new Label();
			this.mt_Tab_General_MatchExpression.Size = new Size(100, 20);
			this.mt_Tab_General_MatchExpression.Location = new Point(10, 140);
			this.mt_Tab_General_MatchExpression.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
			this.mt_Tab_General_MatchExpression.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_General_MatchExpression.Text = "Match Expression:";
			this.m_pTab_General_MatchExprToolbar = new ToolStrip();
			this.m_pTab_General_MatchExprToolbar.AutoSize = false;
			this.m_pTab_General_MatchExprToolbar.Size = new Size(26, 25);
			this.m_pTab_General_MatchExprToolbar.Location = new Point(450, 135);
			this.m_pTab_General_MatchExprToolbar.Dock = DockStyle.None;
			this.m_pTab_General_MatchExprToolbar.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pTab_General_MatchExprToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pTab_General_MatchExprToolbar.BackColor = this.BackColor;
			this.m_pTab_General_MatchExprToolbar.Renderer = new ToolBarRendererEx();
			this.m_pTab_General_MatchExprToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pTab_General_MatchExprToolbar_ItemClicked);
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Image = ResManager.GetIcon("checksyntax.ico").ToBitmap();
			toolStripButton.Tag = "checksyntax";
			this.m_pTab_General_MatchExprToolbar.Items.Add(toolStripButton);
			this.m_pTab_General_MatchExpression = new WRichTextBox();
			this.m_pTab_General_MatchExpression.Size = new Size(465, 140);
			this.m_pTab_General_MatchExpression.Location = new Point(10, 160);
			this.m_pTab_General_MatchExpression.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				this.m_pTab_General_MatchExpression.Font = new Font("Courier New", 10f, FontStyle.Regular, GraphicsUnit.Point, 0);
			}
			this.m_pTab_General_MatchExpression.TextChanged += new EventHandler(this.m_pTab_General_MatchExpression_TextChanged);
			this.m_pTab_General_Create = new Button();
			this.m_pTab_General_Create.Size = new Size(70, 20);
			this.m_pTab_General_Create.Location = new Point(405, 285);
			this.m_pTab_General_Create.Text = "Create";
			this.m_pTab_General_Create.Visible = false;
			this.m_pTab_General_Create.Click += new EventHandler(this.m_pTab_General_Create_Click);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Icon);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_Info);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Separator1);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Enabled);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_Description);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Description);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_CheckNextRule);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_CheckNextRule);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_MatchExpression);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_MatchExprToolbar);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_MatchExpression);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Create);
			this.m_pTab_Actions_Icon = new PictureBox();
			this.m_pTab_Actions_Icon.Size = new Size(32, 32);
			this.m_pTab_Actions_Icon.Location = new Point(10, 10);
			this.m_pTab_Actions_Icon.Image = ResManager.GetIcon("ruleaction.ico").ToBitmap();
			this.mt_Tab_Actions_Info = new Label();
			this.mt_Tab_Actions_Info.Size = new Size(200, 32);
			this.mt_Tab_Actions_Info.Location = new Point(50, 10);
			this.mt_Tab_Actions_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_Actions_Info.Text = "Specify user message rule actions.";
			this.m_pTab_Actions_Separator1 = new GroupBox();
			this.m_pTab_Actions_Separator1.Size = new Size(475, 3);
			this.m_pTab_Actions_Separator1.Location = new Point(7, 50);
			this.m_pTab_Actions_Separator1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTab_Actions_ActionsToolbar = new ToolStrip();
			this.m_pTab_Actions_ActionsToolbar.AutoSize = false;
			this.m_pTab_Actions_ActionsToolbar.Size = new Size(72, 25);
			this.m_pTab_Actions_ActionsToolbar.Location = new Point(405, 55);
			this.m_pTab_Actions_ActionsToolbar.Dock = DockStyle.None;
			this.m_pTab_Actions_ActionsToolbar.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pTab_Actions_ActionsToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pTab_Actions_ActionsToolbar.BackColor = this.BackColor;
			this.m_pTab_Actions_ActionsToolbar.Renderer = new ToolBarRendererEx();
			this.m_pTab_Actions_ActionsToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pTab_Actions_ActionsToolbar_ItemClicked);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton2.Tag = "add";
			this.m_pTab_Actions_ActionsToolbar.Items.Add(toolStripButton2);
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Enabled = false;
			toolStripButton3.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			toolStripButton3.Tag = "edit";
			this.m_pTab_Actions_ActionsToolbar.Items.Add(toolStripButton3);
			ToolStripButton toolStripButton4 = new ToolStripButton();
			toolStripButton4.Enabled = false;
			toolStripButton4.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton4.Tag = "delete";
			this.m_pTab_Actions_ActionsToolbar.Items.Add(toolStripButton4);
			this.m_pTab_Actions_Actions = new ListView();
			this.m_pTab_Actions_Actions.Size = new Size(465, 220);
			this.m_pTab_Actions_Actions.Location = new Point(10, 80);
			this.m_pTab_Actions_Actions.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTab_Actions_Actions.View = View.Details;
			this.m_pTab_Actions_Actions.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.m_pTab_Actions_Actions.BorderStyle = BorderStyle.FixedSingle;
			this.m_pTab_Actions_Actions.FullRowSelect = true;
			this.m_pTab_Actions_Actions.HideSelection = false;
			this.m_pTab_Actions_Actions.DoubleClick += new EventHandler(this.m_pActions_DoubleClick);
			this.m_pTab_Actions_Actions.SelectedIndexChanged += new EventHandler(this.m_pActions_SelectedIndexChanged);
			this.m_pTab_Actions_Actions.Columns.Add("Action", 160, HorizontalAlignment.Left);
			this.m_pTab_Actions_Actions.Columns.Add("Description", 280, HorizontalAlignment.Left);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Actions_Icon);
			this.m_pTab.TabPages[1].Controls.Add(this.mt_Tab_Actions_Info);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Actions_Separator1);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Actions_ActionsToolbar);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Actions_Actions);
		}

		private void m_pTab_General_MatchExprToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "checksyntax")
			{
				this.CheckSyntax(true);
			}
		}

		private void m_pTab_General_MatchExpression_TextChanged(object sender, EventArgs e)
		{
			this.m_pTab_General_MatchExpression.SuspendPaint = true;
			string text = this.m_pTab_General_MatchExpression.Text;
			int selectionStart = this.m_pTab_General_MatchExpression.SelectionStart;
			StringReader stringReader = new StringReader(text);
			while (stringReader.Available > 0L)
			{
				stringReader.ReadToFirstChar();
				int position = stringReader.Position;
				string text2 = stringReader.ReadWord(false);
				if (text2 == null)
				{
					break;
				}
				if (text2 == "")
				{
					text2 = stringReader.ReadSpecifiedLength(1);
				}
				if (text2.StartsWith("\"") && text2.EndsWith("\""))
				{
					this.m_pTab_General_MatchExpression.SelectionStart = position;
					this.m_pTab_General_MatchExpression.SelectionLength = text2.Length;
					this.m_pTab_General_MatchExpression.SelectionColor = Color.Brown;
				}
				else
				{
					bool flag = false;
					string[] array = new string[]
					{
						"and",
						"or",
						"not"
					};
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string text3 = array2[i];
						if (text2.ToLower() == text3.ToLower())
						{
							flag = true;
							break;
						}
					}
					bool flag2 = false;
					string[] array3 = new string[]
					{
						"smtp.mail_from",
						"smtp.rcpt_to",
						"smtp.ehlo",
						"smtp.authenticated",
						"smtp.user",
						"smtp.remote_ip",
						"message.size",
						"message.header",
						"message.all_headers",
						"message.body_text",
						"message.body_html",
						"message.content_md5",
						"sys.date_time",
						"sys.date",
						"sys.time",
						"sys.day_of_week",
						"sys.day_of_month",
						"sys.day_of_year"
					};
					string[] array4 = array3;
					for (int j = 0; j < array4.Length; j++)
					{
						string text4 = array4[j];
						if (text2.ToLower() == text4.ToLower())
						{
							flag2 = true;
							break;
						}
					}
					if (flag)
					{
						this.m_pTab_General_MatchExpression.SelectionStart = position;
						this.m_pTab_General_MatchExpression.SelectionLength = text2.Length;
						this.m_pTab_General_MatchExpression.SelectionColor = Color.Blue;
					}
					else if (flag2)
					{
						this.m_pTab_General_MatchExpression.SelectionStart = position;
						this.m_pTab_General_MatchExpression.SelectionLength = text2.Length;
						this.m_pTab_General_MatchExpression.SelectionColor = Color.DarkMagenta;
					}
					else
					{
						this.m_pTab_General_MatchExpression.SelectionStart = position;
						this.m_pTab_General_MatchExpression.SelectionLength = text2.Length;
						this.m_pTab_General_MatchExpression.SelectionColor = Color.Black;
					}
				}
			}
			this.m_pTab_General_MatchExpression.SelectionStart = selectionStart;
			this.m_pTab_General_MatchExpression.SelectionLength = 0;
			this.m_pTab_General_MatchExpression.SuspendPaint = false;
		}

		private void m_pTab_General_Create_Click(object sender, EventArgs e)
		{
			if (!this.CheckSyntax(false))
			{
				return;
			}
			this.m_pRule = this.m_pVirtualServer.GlobalMessageRules.Add(this.m_pTab_General_Enabled.Checked, this.m_pTab_General_Description.Text, this.m_pTab_General_MatchExpression.Text, (GlobalMessageRule_CheckNextRule)((WComboBoxItem)this.m_pTab_General_CheckNextRule.SelectedItem).Tag);
			this.m_pTab_General_Create.Visible = false;
			this.m_pTab_General_MatchExpression.Height += 25;
			this.m_pTab.TabPages.Add(this.m_pTab_Actions);
		}

		private void m_pTab_Actions_ActionsToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				AddEditGlobalMessageRuleActionForm addEditGlobalMessageRuleActionForm = new AddEditGlobalMessageRuleActionForm(this.m_pRule);
				if (addEditGlobalMessageRuleActionForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadActions();
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "edit")
			{
				if (this.m_pTab_Actions_Actions.SelectedItems.Count > 0)
				{
					GlobalMessageRuleActionBase action = (GlobalMessageRuleActionBase)this.m_pTab_Actions_Actions.SelectedItems[0].Tag;
					AddEditGlobalMessageRuleActionForm addEditGlobalMessageRuleActionForm2 = new AddEditGlobalMessageRuleActionForm(this.m_pRule, action);
					if (addEditGlobalMessageRuleActionForm2.ShowDialog(this) == DialogResult.OK)
					{
						this.LoadActions();
						return;
					}
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete" && this.m_pTab_Actions_Actions.SelectedItems.Count > 0)
			{
				GlobalMessageRuleActionBase globalMessageRuleActionBase = (GlobalMessageRuleActionBase)this.m_pTab_Actions_Actions.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete Action '" + globalMessageRuleActionBase.Description + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					globalMessageRuleActionBase.Owner.Remove(globalMessageRuleActionBase);
					this.LoadActions();
				}
			}
		}

		private void m_pActions_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pTab_Actions_Actions.SelectedItems.Count > 0)
			{
				GlobalMessageRuleActionBase action = (GlobalMessageRuleActionBase)this.m_pTab_Actions_Actions.SelectedItems[0].Tag;
				AddEditGlobalMessageRuleActionForm addEditGlobalMessageRuleActionForm = new AddEditGlobalMessageRuleActionForm(this.m_pRule, action);
				if (addEditGlobalMessageRuleActionForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadActions();
				}
			}
		}

		private void m_pActions_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pTab_Actions_Actions.Items.Count > 0)
			{
				int arg_25_0 = this.m_pTab_Actions_Actions.SelectedItems.Count;
			}
		}

		private void m_pHelp_Click(object sender, EventArgs e)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo("explorer", Application.StartupPath + "\\Help\\Grules.htm");
			Process.Start(startInfo);
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (!this.CheckSyntax(false))
			{
				return;
			}
			if (this.m_pTab_General_Description.Text == "")
			{
				MessageBox.Show(this, "Please fill description !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.m_pRule == null)
			{
				this.m_pRule = this.m_pVirtualServer.GlobalMessageRules.Add(this.m_pTab_General_Enabled.Checked, this.m_pTab_General_Description.Text, this.m_pTab_General_MatchExpression.Text, (GlobalMessageRule_CheckNextRule)((WComboBoxItem)this.m_pTab_General_CheckNextRule.SelectedItem).Tag);
			}
			else
			{
				this.m_pRule.Enabled = this.m_pTab_General_Enabled.Checked;
				this.m_pRule.Description = this.m_pTab_General_Description.Text;
				this.m_pRule.MatchExpression = this.m_pTab_General_MatchExpression.Text;
				this.m_pRule.CheckNextRule = (GlobalMessageRule_CheckNextRule)((WComboBoxItem)this.m_pTab_General_CheckNextRule.SelectedItem).Tag;
				this.m_pRule.Commit();
			}
			base.DialogResult = DialogResult.OK;
		}

		private void LoadActions()
		{
			this.m_pTab_Actions_Actions.Items.Clear();
			foreach (GlobalMessageRuleActionBase globalMessageRuleActionBase in this.m_pRule.Actions)
			{
				ListViewItem listViewItem = new ListViewItem();
				if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.AutoResponse)
				{
					listViewItem.Text = "Auto Response";
				}
				else if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.AutoResponse)
				{
					listViewItem.Text = "Delete Message";
				}
				else if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.ForwardToEmail)
				{
					listViewItem.Text = "Forward To Email";
				}
				else if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.ForwardToHost)
				{
					listViewItem.Text = "Forward To Host";
				}
				else if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.StoreToDiskFolder)
				{
					listViewItem.Text = "Store To Disk Folder";
				}
				else if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.ExecuteProgram)
				{
					listViewItem.Text = "Execute Program";
				}
				else if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.MoveToIMAPFolder)
				{
					listViewItem.Text = "Move To IMAP Folder";
				}
				else if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.AddHeaderField)
				{
					listViewItem.Text = "Add Header Field";
				}
				else if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.RemoveHeaderField)
				{
					listViewItem.Text = "Remove Header Field";
				}
				else if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.SendErrorToClient)
				{
					listViewItem.Text = "Send Error To Client";
				}
				else if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.StoreToFTPFolder)
				{
					listViewItem.Text = "Store To FTP Folder";
				}
				else if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.PostToNNTPNewsGroup)
				{
					listViewItem.Text = "Post To NNTP Newsgroup";
				}
				else if (globalMessageRuleActionBase.ActionType == GlobalMessageRuleActionType.PostToHTTP)
				{
					listViewItem.Text = "Post To HTTP";
				}
				else
				{
					listViewItem.Text = globalMessageRuleActionBase.ActionType.ToString();
				}
				listViewItem.Tag = globalMessageRuleActionBase;
				listViewItem.SubItems.Add(globalMessageRuleActionBase.Description);
				this.m_pTab_Actions_Actions.Items.Add(listViewItem);
			}
			this.m_pActions_SelectedIndexChanged(this, new EventArgs());
		}

		private bool CheckSyntax(bool showOkMessageBox)
		{
			bool result;
			try
			{
				GlobalMessageRuleProcessor.CheckMatchExpressionSyntax(this.m_pTab_General_MatchExpression.Text);
				if (showOkMessageBox)
				{
					MessageBox.Show(this, "Syntax Ok !", "Syntax Check:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Syntax Error !\r\n\r\n" + ex.Message, "Syntax Check:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				result = false;
			}
			return result;
		}
	}
}

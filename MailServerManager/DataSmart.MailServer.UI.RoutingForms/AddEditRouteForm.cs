using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.RoutingForms
{
	public class AddEditRouteForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private CheckBox m_pEnabled;

		private Label mt_Description;

		private TextBox m_pDescription;

		private Label mt_Pattern;

		private TextBox m_pPattern;

		private Label mt_Action;

		private ComboBox m_pAction;

		private GroupBox m_pGroubBox1;

		private Button m_pCancel;

		private Button m_pOk;

		private Label mt_RouteToMailbox_Mailbox;

		private TextBox m_pRouteToMailbox_Mailbox;

		private Button m_pRouteToMailbox_GetMailbox;

		private Label mt_RouteToEmail_Email;

		private TextBox m_pRouteToEmail_Email;

		private Label mt_RouteToHost_Host;

		private TextBox m_pRouteToHost_Host;

		private NumericUpDown m_pRouteToHost_Port;

		private VirtualServer m_pVirtualServer;

		private Route m_pRoute;

		public string RouteID
		{
			get
			{
				if (this.m_pRoute != null)
				{
					return this.m_pRoute.ID;
				}
				return "";
			}
		}

		public AddEditRouteForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.m_pAction.SelectedIndex = 0;
		}

		public AddEditRouteForm(VirtualServer virtualServer, Route route)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pRoute = route;
			this.InitializeComponent();
			this.m_pEnabled.Checked = route.Enabled;
			this.m_pDescription.Text = route.Description;
			this.m_pPattern.Text = route.Pattern;
			if (route.Action.ActionType == RouteAction.RouteToMailbox)
			{
				this.m_pRouteToMailbox_Mailbox.Text = ((RouteAction_RouteToMailbox)route.Action).Mailbox;
				this.m_pAction.SelectedIndex = 0;
				return;
			}
			if (route.Action.ActionType == RouteAction.RouteToEmail)
			{
				this.m_pRouteToEmail_Email.Text = ((RouteAction_RouteToEmail)route.Action).EmailAddress;
				this.m_pAction.SelectedIndex = 1;
				return;
			}
			if (route.Action.ActionType == RouteAction.RouteToHost)
			{
				this.m_pRouteToHost_Host.Text = ((RouteAction_RouteToHost)route.Action).Host;
				this.m_pRouteToHost_Port.Value = ((RouteAction_RouteToHost)route.Action).Port;
				this.m_pAction.SelectedIndex = 2;
			}
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(392, 273);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Add/Edit route";
			base.Icon = ResManager.GetIcon("ruleaction.ico");
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetIcon("ruleaction.ico").ToBitmap();
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(200, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = "Specify route information.";
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(383, 3);
			this.m_pSeparator1.Location = new Point(7, 50);
			this.m_pEnabled = new CheckBox();
			this.m_pEnabled.Size = new Size(150, 20);
			this.m_pEnabled.Location = new Point(105, 65);
			this.m_pEnabled.Text = "Enabled";
			this.m_pEnabled.Checked = true;
			this.mt_Description = new Label();
			this.mt_Description.Size = new Size(100, 20);
			this.mt_Description.Location = new Point(0, 90);
			this.mt_Description.TextAlign = ContentAlignment.BottomRight;
			this.mt_Description.Text = "Description:";
			this.m_pDescription = new TextBox();
			this.m_pDescription.Size = new Size(275, 20);
			this.m_pDescription.Location = new Point(105, 90);
			this.mt_Pattern = new Label();
			this.mt_Pattern.Size = new Size(100, 20);
			this.mt_Pattern.Location = new Point(0, 115);
			this.mt_Pattern.TextAlign = ContentAlignment.BottomRight;
			this.mt_Pattern.Text = "Pattern:";
			this.m_pPattern = new TextBox();
			this.m_pPattern.Size = new Size(200, 20);
			this.m_pPattern.Location = new Point(105, 115);
			this.mt_Action = new Label();
			this.mt_Action.Size = new Size(100, 20);
			this.mt_Action.Location = new Point(0, 140);
			this.mt_Action.TextAlign = ContentAlignment.BottomRight;
			this.mt_Action.Text = "Action:";
			this.m_pAction = new ComboBox();
			this.m_pAction.Size = new Size(200, 20);
			this.m_pAction.Location = new Point(105, 140);
			this.m_pAction.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pAction.SelectedIndexChanged += new EventHandler(this.m_pAction_SelectedIndexChanged);
			this.m_pAction.Items.Add(new WComboBoxItem("Route To Mailbox", RouteAction.RouteToMailbox));
			this.m_pAction.Items.Add(new WComboBoxItem("Route To Email", RouteAction.RouteToEmail));
			this.m_pAction.Items.Add(new WComboBoxItem("Route To Host", RouteAction.RouteToHost));
			this.mt_RouteToMailbox_Mailbox = new Label();
			this.mt_RouteToMailbox_Mailbox.Size = new Size(100, 18);
			this.mt_RouteToMailbox_Mailbox.Location = new Point(0, 175);
			this.mt_RouteToMailbox_Mailbox.Visible = false;
			this.mt_RouteToMailbox_Mailbox.TextAlign = ContentAlignment.MiddleRight;
			this.mt_RouteToMailbox_Mailbox.Text = "Mailbox:";
			this.m_pRouteToMailbox_Mailbox = new TextBox();
			this.m_pRouteToMailbox_Mailbox.Size = new Size(200, 20);
			this.m_pRouteToMailbox_Mailbox.Location = new Point(105, 175);
			this.m_pRouteToMailbox_Mailbox.Visible = false;
			this.m_pRouteToMailbox_Mailbox.ReadOnly = true;
			this.m_pRouteToMailbox_GetMailbox = new Button();
			this.m_pRouteToMailbox_GetMailbox.Size = new Size(25, 20);
			this.m_pRouteToMailbox_GetMailbox.Location = new Point(315, 175);
			this.m_pRouteToMailbox_GetMailbox.Visible = false;
			this.m_pRouteToMailbox_GetMailbox.Text = "...";
			this.m_pRouteToMailbox_GetMailbox.Click += new EventHandler(this.m_pRouteToMailbox_GetMailbox_Click);
			this.mt_RouteToEmail_Email = new Label();
			this.mt_RouteToEmail_Email.Size = new Size(100, 18);
			this.mt_RouteToEmail_Email.Location = new Point(0, 175);
			this.mt_RouteToEmail_Email.Visible = false;
			this.mt_RouteToEmail_Email.TextAlign = ContentAlignment.MiddleRight;
			this.mt_RouteToEmail_Email.Text = "Email:";
			this.m_pRouteToEmail_Email = new TextBox();
			this.m_pRouteToEmail_Email.Size = new Size(200, 20);
			this.m_pRouteToEmail_Email.Location = new Point(105, 175);
			this.m_pRouteToEmail_Email.Visible = false;
			this.mt_RouteToHost_Host = new Label();
			this.mt_RouteToHost_Host.Size = new Size(100, 18);
			this.mt_RouteToHost_Host.Location = new Point(0, 175);
			this.mt_RouteToHost_Host.Visible = false;
			this.mt_RouteToHost_Host.TextAlign = ContentAlignment.MiddleRight;
			this.mt_RouteToHost_Host.Text = "Host:";
			this.m_pRouteToHost_Host = new TextBox();
			this.m_pRouteToHost_Host.Size = new Size(200, 20);
			this.m_pRouteToHost_Host.Location = new Point(105, 175);
			this.m_pRouteToHost_Host.Visible = false;
			this.m_pRouteToHost_Port = new NumericUpDown();
			this.m_pRouteToHost_Port.Size = new Size(70, 20);
			this.m_pRouteToHost_Port.Location = new Point(315, 175);
			this.m_pRouteToHost_Port.Visible = false;
			this.m_pRouteToHost_Port.Minimum = 1m;
			this.m_pRouteToHost_Port.Maximum = 99999m;
			this.m_pRouteToHost_Port.Value = 25m;
			this.m_pGroubBox1 = new GroupBox();
			this.m_pGroubBox1.Size = new Size(387, 3);
			this.m_pGroubBox1.Location = new Point(5, 235);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(240, 245);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(315, 245);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.mt_Info);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.m_pEnabled);
			base.Controls.Add(this.mt_Description);
			base.Controls.Add(this.m_pDescription);
			base.Controls.Add(this.mt_Pattern);
			base.Controls.Add(this.m_pPattern);
			base.Controls.Add(this.mt_Action);
			base.Controls.Add(this.m_pAction);
			base.Controls.Add(this.mt_RouteToMailbox_Mailbox);
			base.Controls.Add(this.m_pRouteToMailbox_Mailbox);
			base.Controls.Add(this.m_pRouteToMailbox_GetMailbox);
			base.Controls.Add(this.mt_RouteToEmail_Email);
			base.Controls.Add(this.m_pRouteToEmail_Email);
			base.Controls.Add(this.mt_RouteToHost_Host);
			base.Controls.Add(this.m_pRouteToHost_Host);
			base.Controls.Add(this.m_pRouteToHost_Port);
			base.Controls.Add(this.m_pGroubBox1);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pAction_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.mt_RouteToMailbox_Mailbox.Visible = false;
			this.m_pRouteToMailbox_Mailbox.Visible = false;
			this.m_pRouteToMailbox_GetMailbox.Visible = false;
			this.mt_RouteToEmail_Email.Visible = false;
			this.m_pRouteToEmail_Email.Visible = false;
			this.mt_RouteToHost_Host.Visible = false;
			this.m_pRouteToHost_Host.Visible = false;
			this.m_pRouteToHost_Port.Visible = false;
			if (this.m_pAction.SelectedIndex == 0)
			{
				this.mt_RouteToMailbox_Mailbox.Visible = true;
				this.m_pRouteToMailbox_Mailbox.Visible = true;
				this.m_pRouteToMailbox_GetMailbox.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedIndex == 1)
			{
				this.mt_RouteToEmail_Email.Visible = true;
				this.m_pRouteToEmail_Email.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedIndex == 2)
			{
				this.mt_RouteToHost_Host.Visible = true;
				this.m_pRouteToHost_Host.Visible = true;
				this.m_pRouteToHost_Port.Visible = true;
			}
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pPattern.Text.Length == 0)
			{
				MessageBox.Show(this, "Please specify match pattern !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			RouteAction routeAction_enum = (RouteAction)((WComboBoxItem)this.m_pAction.SelectedItem).Tag;
			RouteActionBase action = null;
			if (routeAction_enum == RouteAction.RouteToMailbox)
			{
				if (this.m_pRouteToMailbox_Mailbox.Text == "")
				{
					MessageBox.Show(this, "Mailbox: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				action = new RouteAction_RouteToMailbox(this.m_pRouteToMailbox_Mailbox.Text);
			}
			else if (routeAction_enum == RouteAction.RouteToEmail)
			{
				if (this.m_pRouteToEmail_Email.Text == "")
				{
					MessageBox.Show(this, "Email: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				action = new RouteAction_RouteToEmail(this.m_pRouteToEmail_Email.Text);
			}
			else if (routeAction_enum == RouteAction.RouteToHost)
			{
				if (this.m_pRouteToHost_Host.Text == "")
				{
					MessageBox.Show(this, "Host: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				action = new RouteAction_RouteToHost(this.m_pRouteToHost_Host.Text, (int)this.m_pRouteToHost_Port.Value);
			}
			if (this.m_pRoute == null)
			{
				this.m_pRoute = this.m_pVirtualServer.Routes.Add(this.m_pDescription.Text, this.m_pPattern.Text, this.m_pEnabled.Checked, action);
			}
			else
			{
				this.m_pRoute.Description = this.m_pDescription.Text;
				this.m_pRoute.Pattern = this.m_pPattern.Text;
				this.m_pRoute.Enabled = this.m_pEnabled.Checked;
				this.m_pRoute.Action = action;
				this.m_pRoute.Commit();
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		private void m_pRouteToMailbox_GetMailbox_Click(object sender, EventArgs e)
		{
			SelectUserOrGroupForm selectUserOrGroupForm = new SelectUserOrGroupForm(this.m_pVirtualServer, false, false);
			if (selectUserOrGroupForm.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pRouteToMailbox_Mailbox.Text = selectUserOrGroupForm.SelectedUserOrGroup;
			}
		}
	}
}

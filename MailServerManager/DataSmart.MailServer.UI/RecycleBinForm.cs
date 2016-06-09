using DataSmart.MailServer.Management;
using System.NetworkToolkit;
using System.NetworkToolkit.IMAP;
using System.NetworkToolkit.Mail;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class RecycleBinForm : Form
	{
		private CheckBox m_pDeleteToRecycleBin;

		private NumericUpDown m_pDeleteAfterDays;

		private Label mt_DeleteAfterDays;

		private Button m_pApply;

		private GroupBox m_pGroupBox1;

		private Label mt_User;

		private TextBox m_pUser;

		private Button m_pGetUser;

		private Label mt_Between;

		private DateTimePicker m_pSince;

		private DateTimePicker m_pTo;

		private Button m_pGet;

		private Button m_pRestore;

		private WListView m_pMessages;

		private VirtualServer m_pVirtualServer;

		public RecycleBinForm(VirtualServer virtualServer, WFrame frame)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.LoadSettings();
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(492, 373);
			this.m_pDeleteToRecycleBin = new CheckBox();
			this.m_pDeleteToRecycleBin.Size = new Size(300, 20);
			this.m_pDeleteToRecycleBin.Location = new Point(10, 15);
			this.m_pDeleteToRecycleBin.Text = "Delete all messages to recycle bin";
			this.m_pDeleteToRecycleBin.CheckedChanged += new EventHandler(this.m_pDeleteToRecycleBin_CheckedChanged);
			this.m_pDeleteAfterDays = new NumericUpDown();
			this.m_pDeleteAfterDays.Size = new Size(50, 20);
			this.m_pDeleteAfterDays.Location = new Point(65, 45);
			this.m_pDeleteAfterDays.Minimum = 1m;
			this.m_pDeleteAfterDays.Maximum = 365m;
			this.m_pDeleteAfterDays.Value = 1m;
			this.mt_DeleteAfterDays = new Label();
			this.mt_DeleteAfterDays.Size = new Size(300, 20);
			this.mt_DeleteAfterDays.Location = new Point(125, 45);
			this.mt_DeleteAfterDays.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_DeleteAfterDays.Text = "Delete messages permanently after specified days";
			this.m_pApply = new Button();
			this.m_pApply.Size = new Size(70, 20);
			this.m_pApply.Location = new Point(415, 45);
			this.m_pApply.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pApply.Text = "Apply";
			this.m_pApply.Click += new EventHandler(this.m_pApply_Click);
			this.m_pGroupBox1 = new GroupBox();
			this.m_pGroupBox1.Size = new Size(480, 3);
			this.m_pGroupBox1.Location = new Point(5, 75);
			this.m_pGroupBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.mt_User = new Label();
			this.mt_User.Size = new Size(50, 20);
			this.mt_User.Location = new Point(10, 95);
			this.mt_User.TextAlign = ContentAlignment.MiddleRight;
			this.mt_User.Text = "User:";
			this.m_pUser = new TextBox();
			this.m_pUser.Size = new Size(165, 20);
			this.m_pUser.Location = new Point(65, 95);
			this.m_pGetUser = new Button();
			this.m_pGetUser.Size = new Size(25, 20);
			this.m_pGetUser.Location = new Point(235, 95);
			this.m_pGetUser.Text = "...";
			this.m_pGetUser.Click += new EventHandler(this.m_pGetUser_Click);
			this.mt_Between = new Label();
			this.mt_Between.Size = new Size(60, 20);
			this.mt_Between.Location = new Point(0, 120);
			this.mt_Between.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Between.Text = "Between:";
			this.m_pSince = new DateTimePicker();
			this.m_pSince.Size = new Size(80, 20);
			this.m_pSince.Location = new Point(65, 120);
			this.m_pSince.Format = DateTimePickerFormat.Short;
			this.m_pTo = new DateTimePicker();
			this.m_pTo.Size = new Size(80, 20);
			this.m_pTo.Location = new Point(150, 120);
			this.m_pTo.Format = DateTimePickerFormat.Short;
			this.m_pGet = new Button();
			this.m_pGet.Size = new Size(50, 20);
			this.m_pGet.Location = new Point(355, 120);
			this.m_pGet.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pGet.Text = "Get";
			this.m_pGet.Click += new EventHandler(this.m_pGet_Click);
			this.m_pRestore = new Button();
			this.m_pRestore.Size = new Size(70, 20);
			this.m_pRestore.Location = new Point(415, 120);
			this.m_pRestore.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pRestore.Text = "Restore";
			this.m_pRestore.Click += new EventHandler(this.m_pRestore_Click);
			this.m_pMessages = new WListView();
			this.m_pMessages.Size = new Size(475, 215);
			this.m_pMessages.Location = new Point(10, 150);
			this.m_pMessages.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pMessages.View = View.Details;
			this.m_pMessages.HideSelection = false;
			this.m_pMessages.FullRowSelect = true;
			this.m_pMessages.Columns.Add("User", 100, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Folder", 120, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Subject", 250, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Sender", 170, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Date", 120, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Date Deleted", 120, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Size KB", 60, HorizontalAlignment.Right);
			this.m_pMessages.SelectedIndexChanged += new EventHandler(this.m_pMessages_SelectedIndexChanged);
			base.Controls.Add(this.m_pDeleteToRecycleBin);
			base.Controls.Add(this.m_pDeleteAfterDays);
			base.Controls.Add(this.mt_DeleteAfterDays);
			base.Controls.Add(this.m_pApply);
			base.Controls.Add(this.m_pGroupBox1);
			base.Controls.Add(this.mt_User);
			base.Controls.Add(this.m_pUser);
			base.Controls.Add(this.m_pGetUser);
			base.Controls.Add(this.mt_Between);
			base.Controls.Add(this.m_pSince);
			base.Controls.Add(this.m_pTo);
			base.Controls.Add(this.m_pGet);
			base.Controls.Add(this.m_pRestore);
			base.Controls.Add(this.m_pMessages);
		}

		private void m_pDeleteToRecycleBin_CheckedChanged(object sender, EventArgs e)
		{
			if (this.m_pDeleteToRecycleBin.Checked)
			{
				IEnumerator enumerator = base.Controls.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Control control = (Control)enumerator.Current;
						control.Enabled = true;
					}
					goto IL_A8;
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			foreach (Control control2 in base.Controls)
			{
				if (!control2.Equals(this.m_pDeleteToRecycleBin) && !control2.Equals(this.m_pApply))
				{
					control2.Enabled = false;
				}
			}
			IL_A8:
			this.m_pMessages_SelectedIndexChanged(this, new EventArgs());
		}

		private void m_pApply_Click(object sender, EventArgs e)
		{
			this.m_pVirtualServer.RecycleBin.DeleteToRecycleBin = this.m_pDeleteToRecycleBin.Checked;
			this.m_pVirtualServer.RecycleBin.DeleteMessagesAfter = (int)this.m_pDeleteAfterDays.Value;
			this.m_pVirtualServer.RecycleBin.Commit();
		}

		private void m_pGetUser_Click(object sender, EventArgs e)
		{
			SelectUserOrGroupForm selectUserOrGroupForm = new SelectUserOrGroupForm(this.m_pVirtualServer, false, false);
			if (selectUserOrGroupForm.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pUser.Text = selectUserOrGroupForm.SelectedUserOrGroup;
			}
		}

		private void m_pGet_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.m_pMessages.Items.Clear();
				foreach (DataRow dataRow in this.m_pVirtualServer.RecycleBin.GetMessagesInfo(this.m_pUser.Text, this.m_pSince.Value.Date, this.m_pTo.Value.Date).Rows)
				{
					string text = dataRow["User"].ToString();
					DateTime arg_A6_0 = Convert.ToDateTime(dataRow["DeleteTime"]).Date;
					IMAP_Envelope iMAP_Envelope = null;
					try
					{
						iMAP_Envelope = IMAP_Envelope.Parse(new StringReader(dataRow["Envelope"].ToString()));
					}
					catch
					{
					}
					ListViewItem listViewItem = new ListViewItem(text);
					listViewItem.SubItems.Add(dataRow["Folder"].ToString());
					listViewItem.SubItems.Add(iMAP_Envelope.Subject);
					if (iMAP_Envelope.Sender != null && ((Mail_t_Mailbox)iMAP_Envelope.Sender[0]).DisplayName != null && ((Mail_t_Mailbox)iMAP_Envelope.Sender[0]).DisplayName != "")
					{
						listViewItem.SubItems.Add(((Mail_t_Mailbox)iMAP_Envelope.Sender[0]).DisplayName);
					}
					else if (iMAP_Envelope.Sender != null)
					{
						listViewItem.SubItems.Add(iMAP_Envelope.Sender.ToString());
					}
					else
					{
						listViewItem.SubItems.Add("<none>");
					}
					listViewItem.SubItems.Add(iMAP_Envelope.Date.ToString());
					listViewItem.SubItems.Add(Convert.ToDateTime(dataRow["DeleteTime"]).ToString());
					listViewItem.SubItems.Add((Convert.ToDecimal(dataRow["Size"]) / 1000m).ToString("f2"));
					listViewItem.ImageIndex = 0;
					listViewItem.Tag = dataRow;
					this.m_pMessages.Items.Add(listViewItem);
				}
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void m_pRestore_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				for (int i = 0; i < this.m_pMessages.SelectedItems.Count; i++)
				{
					ListViewItem listViewItem = this.m_pMessages.SelectedItems[i];
					this.m_pVirtualServer.RecycleBin.RestoreRecycleBinMessage(((DataRow)listViewItem.Tag)["MessageID"].ToString());
					listViewItem.Remove();
					i--;
				}
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void m_pMessages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pMessages.SelectedItems.Count > 0)
			{
				this.m_pRestore.Enabled = true;
				return;
			}
			this.m_pRestore.Enabled = false;
		}

		private void LoadSettings()
		{
			this.m_pDeleteToRecycleBin.Checked = this.m_pVirtualServer.RecycleBin.DeleteToRecycleBin;
			this.m_pDeleteAfterDays.Value = this.m_pVirtualServer.RecycleBin.DeleteMessagesAfter;
			this.m_pDeleteToRecycleBin_CheckedChanged(this, new EventArgs());
		}
	}
}

using DataSmart.MailServer.UI.Resources;
using System.NetworkToolkit;
using System.NetworkToolkit.IMAP;
using System.NetworkToolkit.Mail;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DataSmart.MailServer.Management;

namespace DataSmart.MailServer.UI.UserForms
{
	public class RecycleBinForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private Label mt_Between;

		private DateTimePicker m_pStartDate;

		private DateTimePicker m_pEndDate;

		private ToolStrip m_pToolbar;

		private WListView m_pMessages;

		private VirtualServer m_pVirtualServer;

		private DataSmart.MailServer.Management.User m_pUser;

		public RecycleBinForm(VirtualServer virtualServer, MailServer.Management.User user)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pUser = user;
			this.InitializeComponent();
			this.LoadData();
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(692, 473);
			this.MinimumSize = new Size(700, 500);
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "User '" + this.m_pUser.UserName + "' Recyclebin messages";
			base.Icon = ResManager.GetIcon("recyclebin16.ico");
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetImage("icon-recycle.png");
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(400, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = "User '" + this.m_pUser.UserName + "' Recyclebin messages";
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(682, 3);
			this.m_pSeparator1.Location = new Point(7, 50);
			this.m_pSeparator1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.mt_Between = new Label();
			this.mt_Between.Size = new Size(100, 20);
			this.mt_Between.Location = new Point(295, 57);
			this.mt_Between.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.mt_Between.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Between.Text = "Between:";
			this.m_pStartDate = new DateTimePicker();
			this.m_pStartDate.Size = new Size(100, 20);
			this.m_pStartDate.Location = new Point(400, 57);
			this.m_pStartDate.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pStartDate.Format = DateTimePickerFormat.Short;
			this.m_pStartDate.Value = DateTime.Today.AddDays(-1.0);
			this.m_pEndDate = new DateTimePicker();
			this.m_pEndDate.Size = new Size(100, 20);
			this.m_pEndDate.Location = new Point(505, 57);
			this.m_pEndDate.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pEndDate.Format = DateTimePickerFormat.Short;
			this.m_pToolbar = new ToolStrip();
			this.m_pToolbar.AutoSize = false;
			this.m_pToolbar.Size = new Size(100, 25);
			this.m_pToolbar.Location = new Point(617, 55);
			this.m_pToolbar.Dock = DockStyle.None;
			this.m_pToolbar.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pToolbar.BackColor = this.BackColor;
			this.m_pToolbar.Renderer = new ToolBarRendererEx();
			this.m_pToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pToolbar_ItemClicked);
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
			toolStripButton.Tag = "refresh";
			toolStripButton.ToolTipText = "Refresh";
			this.m_pToolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Image = ResManager.GetIcon("restore.ico").ToBitmap();
			toolStripButton2.Tag = "restore";
			toolStripButton2.Enabled = false;
			toolStripButton2.ToolTipText = "Restore Message";
			this.m_pToolbar.Items.Add(toolStripButton2);
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Image = ResManager.GetIcon("save.ico").ToBitmap();
			toolStripButton3.Tag = "save";
			toolStripButton3.Enabled = false;
			toolStripButton3.ToolTipText = "Save Message";
			this.m_pToolbar.Items.Add(toolStripButton3);
			ImageList imageList = new ImageList();
			imageList.Images.Add(ResManager.GetIcon("message16.ico"));
			this.m_pMessages = new WListView();
			this.m_pMessages.Size = new Size(682, 390);
			this.m_pMessages.Location = new Point(5, 80);
			this.m_pMessages.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pMessages.View = View.Details;
			this.m_pMessages.FullRowSelect = true;
			this.m_pMessages.HideSelection = false;
			this.m_pMessages.SmallImageList = imageList;
			this.m_pMessages.Columns.Add("Folder", 120, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Subject", 250, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Sender", 170, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Date", 120, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Date Deleted", 120, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Size KB", 60, HorizontalAlignment.Right);
			this.m_pMessages.SelectedIndexChanged += new EventHandler(this.m_pMessages_SelectedIndexChanged);
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.mt_Info);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.mt_Between);
			base.Controls.Add(this.m_pStartDate);
			base.Controls.Add(this.m_pEndDate);
			base.Controls.Add(this.m_pToolbar);
			base.Controls.Add(this.m_pMessages);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			this.Cursor = Cursors.WaitCursor;
			if (e.ClickedItem.Tag.ToString() == "refresh")
			{
				this.LoadData();
			}
			else
			{
				if (e.ClickedItem.Tag.ToString() == "save")
				{
					SaveFileDialog saveFileDialog = new SaveFileDialog();
					saveFileDialog.Filter = "Email Message (*.eml)|*.eml";
					saveFileDialog.FileName = this.m_pMessages.SelectedItems[0].SubItems[1].Text.Replace("\\", " ").Replace("/", " ").Replace(":", " ").Replace("*", " ").Replace("?", " ").Replace("<", " ").Replace(">", " ");
					if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
					{
						goto IL_1F0;
					}
					using (FileStream fileStream = File.Create(saveFileDialog.FileName))
					{
						ListViewItem listViewItem = this.m_pMessages.SelectedItems[0];
						this.m_pVirtualServer.RecycleBin.GetMessage(((DataRow)listViewItem.Tag)["MessageID"].ToString(), fileStream);
						goto IL_1F0;
					}
				}
				if (e.ClickedItem.Tag.ToString() == "restore")
				{
					this.Cursor = Cursors.WaitCursor;
					for (int i = 0; i < this.m_pMessages.SelectedItems.Count; i++)
					{
						ListViewItem listViewItem2 = this.m_pMessages.SelectedItems[i];
						this.m_pVirtualServer.RecycleBin.RestoreRecycleBinMessage(((DataRow)listViewItem2.Tag)["MessageID"].ToString());
						listViewItem2.Remove();
						i--;
					}
				}
			}
			IL_1F0:
			this.Cursor = Cursors.Default;
		}

		private void m_pMessages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pMessages.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				this.m_pToolbar.Items[2].Enabled = true;
				return;
			}
			this.m_pToolbar.Items[1].Enabled = false;
			this.m_pToolbar.Items[2].Enabled = false;
		}

		private void LoadData()
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.m_pMessages.Items.Clear();
				DataTable messagesInfo = this.m_pVirtualServer.RecycleBin.GetMessagesInfo(this.m_pUser.UserName, this.m_pStartDate.Value.Date, this.m_pEndDate.Value.Date);
				foreach (DataRow dataRow in messagesInfo.Rows)
				{
					dataRow["User"].ToString();
					DateTime arg_A8_0 = Convert.ToDateTime(dataRow["DeleteTime"]).Date;
					IMAP_Envelope iMAP_Envelope = null;
					try
					{
						iMAP_Envelope = IMAP_Envelope.Parse(new System.NetworkToolkit.StringReader(dataRow["Envelope"].ToString()));
					}
					catch
					{
					}
					ListViewItem listViewItem = new ListViewItem(dataRow["Folder"].ToString());
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
				this.mt_Info.Text = string.Concat(new object[]
				{
					"User '",
					this.m_pUser.UserName,
					"' Recyclebin messages (",
					messagesInfo.Rows.Count,
					")"
				});
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}
	}
}

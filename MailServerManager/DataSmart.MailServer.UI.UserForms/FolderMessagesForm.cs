using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System.NetworkToolkit;
using System.NetworkToolkit.IMAP;
using System.NetworkToolkit.Mail;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.UserForms
{
	public class FolderMessagesForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private ToolStrip m_pToolbar;

		private WListView m_pMessages;

		private VirtualServer m_pVirtualServer;

		private UserFolder m_pFolder;

		public FolderMessagesForm(VirtualServer virtualServer, UserFolder folder)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pFolder = folder;
			this.InitializeComponent();
			this.LoadData();
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(692, 473);
			this.MinimumSize = new Size(700, 500);
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = string.Concat(new string[]
			{
				"User '",
				this.m_pFolder.User.UserName,
				"' Folder '",
				this.m_pFolder.FolderFullPath,
				"' Messages"
			});
			base.Icon = ResManager.GetIcon("message.ico");
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetIcon("message.ico").ToBitmap();
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(400, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = string.Concat(new string[]
			{
				"User '",
				this.m_pFolder.User.UserName,
				"' Folder '",
				this.m_pFolder.FolderFullPath,
				"' Messages"
			});
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(682, 3);
			this.m_pSeparator1.Location = new Point(7, 50);
			this.m_pSeparator1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pToolbar = new ToolStrip();
			this.m_pToolbar.AutoSize = false;
			this.m_pToolbar.Size = new Size(100, 25);
			this.m_pToolbar.Location = new Point(595, 55);
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
			toolStripButton2.Image = ResManager.GetIcon("write.ico").ToBitmap();
			toolStripButton2.Tag = "write";
			toolStripButton2.ToolTipText = "Write Message";
			this.m_pToolbar.Items.Add(toolStripButton2);
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Image = ResManager.GetIcon("save.ico").ToBitmap();
			toolStripButton3.Tag = "save";
			toolStripButton3.Enabled = false;
			toolStripButton3.ToolTipText = "Save Message";
			this.m_pToolbar.Items.Add(toolStripButton3);
			ToolStripButton toolStripButton4 = new ToolStripButton();
			toolStripButton4.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton4.Tag = "delete";
			toolStripButton4.Enabled = false;
			toolStripButton4.ToolTipText = "Delete Message";
			this.m_pToolbar.Items.Add(toolStripButton4);
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
			this.m_pMessages.Columns.Add("Subject", 250, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Sender", 170, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Date", 120, HorizontalAlignment.Left);
			this.m_pMessages.Columns.Add("Size KB", 60, HorizontalAlignment.Right);
			this.m_pMessages.SelectedIndexChanged += new EventHandler(this.m_pMessages_SelectedIndexChanged);
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.mt_Info);
			base.Controls.Add(this.m_pSeparator1);
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
					saveFileDialog.FileName = this.m_pMessages.SelectedItems[0].Text.Replace("\\", " ").Replace("/", " ").Replace(":", " ").Replace("*", " ").Replace("?", " ").Replace("<", " ").Replace(">", " ");
					if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
					{
						goto IL_256;
					}
					using (FileStream fileStream = File.Create(saveFileDialog.FileName))
					{
						ListViewItem listViewItem = this.m_pMessages.SelectedItems[0];
						this.m_pFolder.GetMessage((string)((object[])listViewItem.Tag)[0], fileStream);
						goto IL_256;
					}
				}
				if (e.ClickedItem.Tag.ToString() == "write")
				{
					ComposeForm composeForm = new ComposeForm(this.m_pFolder);
					if (composeForm.ShowDialog(this) == DialogResult.OK)
					{
						MemoryStream memoryStream = new MemoryStream(composeForm.Message);
						memoryStream.Position = 0L;
						this.m_pFolder.StoreMessage(memoryStream);
					}
				}
				else if (e.ClickedItem.Tag.ToString() == "delete" && MessageBox.Show(this, "Are you sure you want to delete selected messages !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					for (int i = 0; i < this.m_pMessages.SelectedItems.Count; i++)
					{
						ListViewItem listViewItem2 = this.m_pMessages.SelectedItems[i];
						this.m_pFolder.DeleteMessage((string)((object[])listViewItem2.Tag)[0], (long)((int)((object[])listViewItem2.Tag)[1]));
						listViewItem2.Remove();
						i--;
					}
				}
			}
			IL_256:
			this.Cursor = Cursors.Default;
		}

		private void m_pMessages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pMessages.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[2].Enabled = true;
				this.m_pToolbar.Items[3].Enabled = true;
				return;
			}
			this.m_pToolbar.Items[2].Enabled = false;
			this.m_pToolbar.Items[3].Enabled = false;
		}

		private void LoadData()
		{
			this.m_pMessages.BeginUpdate();
			this.m_pMessages.Items.Clear();
			DataSet messagesInfo = this.m_pFolder.GetMessagesInfo();
			if (messagesInfo.Tables.Contains("MessagesInfo"))
			{
				foreach (DataRow dataRow in messagesInfo.Tables["MessagesInfo"].Rows)
				{
					IMAP_t_Fetch_r_i_Envelope iMAP_t_Fetch_r_i_Envelope = null;
					try
					{
						System.NetworkToolkit.StringReader stringReader = new System.NetworkToolkit.StringReader(dataRow["Envelope"].ToString());
						stringReader.ReadWord();
						stringReader = new System.NetworkToolkit.StringReader(stringReader.ReadParenthesized());
						iMAP_t_Fetch_r_i_Envelope = IMAP_t_Fetch_r_i_Envelope.Parse(stringReader);
					}
					catch
					{
						iMAP_t_Fetch_r_i_Envelope = new IMAP_t_Fetch_r_i_Envelope(DateTime.Now, "Mailserver parse error", null, null, null, null, null, null, null, null);
					}
					ListViewItem listViewItem = new ListViewItem(iMAP_t_Fetch_r_i_Envelope.Subject);
					listViewItem.ImageIndex = 0;
					listViewItem.Tag = new object[]
					{
						dataRow["ID"].ToString(),
						Convert.ToInt32(dataRow["UID"])
					};
					if (iMAP_t_Fetch_r_i_Envelope.Sender != null && ((Mail_t_Mailbox)iMAP_t_Fetch_r_i_Envelope.Sender[0]).DisplayName != null && ((Mail_t_Mailbox)iMAP_t_Fetch_r_i_Envelope.Sender[0]).DisplayName != "")
					{
						listViewItem.SubItems.Add(((Mail_t_Mailbox)iMAP_t_Fetch_r_i_Envelope.Sender[0]).DisplayName);
					}
					else if (iMAP_t_Fetch_r_i_Envelope.From != null && iMAP_t_Fetch_r_i_Envelope.From.Length > 0)
					{
						listViewItem.SubItems.Add(iMAP_t_Fetch_r_i_Envelope.From[0].ToString());
					}
					else
					{
						listViewItem.SubItems.Add("<none>");
					}
					listViewItem.SubItems.Add(iMAP_t_Fetch_r_i_Envelope.Date.ToString());
					listViewItem.SubItems.Add((Convert.ToDecimal(dataRow["Size"]) / 1000m).ToString("f2"));
					this.m_pMessages.Items.Add(listViewItem);
				}
				this.mt_Info.Text = string.Concat(new object[]
				{
					"User '",
					this.m_pFolder.User.UserName,
					"' Folder '",
					this.m_pFolder.FolderFullPath,
					"' Messages (",
					messagesInfo.Tables["MessagesInfo"].Rows.Count,
					")"
				});
			}
			this.m_pMessages.EndUpdate();
			this.m_pMessages_SelectedIndexChanged(null, null);
		}
	}
}

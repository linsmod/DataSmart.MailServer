using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System.NetworkToolkit.Mail;
using System.NetworkToolkit.MIME;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class ComposeForm : Form
	{
		private class Shell32
		{
			public struct SHITEMID
			{
				public ushort cb;

				[MarshalAs(UnmanagedType.LPArray)]
				public byte[] abID;
			}

			public struct ITEMIDLIST
			{
				public ComposeForm.Shell32.SHITEMID mkid;
			}

			public struct BROWSEINFO
			{
				public IntPtr hwndOwner;

				public IntPtr pidlRoot;

				public IntPtr pszDisplayName;

				[MarshalAs(UnmanagedType.LPTStr)]
				public string lpszTitle;

				public uint ulFlags;

				public IntPtr lpfn;

				public int lParam;

				public IntPtr iImage;
			}

			public struct SHFILEINFO
			{
				public const int NAMESIZE = 80;

				public IntPtr hIcon;

				public int iIcon;

				public uint dwAttributes;

				[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
				public string szDisplayName;

				[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
				public string szTypeName;
			}

			public const int MAX_PATH = 256;

			public const uint BIF_RETURNONLYFSDIRS = 1u;

			public const uint BIF_DONTGOBELOWDOMAIN = 2u;

			public const uint BIF_STATUSTEXT = 4u;

			public const uint BIF_RETURNFSANCESTORS = 8u;

			public const uint BIF_EDITBOX = 16u;

			public const uint BIF_VALIDATE = 32u;

			public const uint BIF_NEWDIALOGSTYLE = 64u;

			public const uint BIF_USENEWUI = 80u;

			public const uint BIF_BROWSEINCLUDEURLS = 128u;

			public const uint BIF_BROWSEFORCOMPUTER = 4096u;

			public const uint BIF_BROWSEFORPRINTER = 8192u;

			public const uint BIF_BROWSEINCLUDEFILES = 16384u;

			public const uint BIF_SHAREABLE = 32768u;

			public const uint SHGFI_ICON = 256u;

			public const uint SHGFI_DISPLAYNAME = 512u;

			public const uint SHGFI_TYPENAME = 1024u;

			public const uint SHGFI_ATTRIBUTES = 2048u;

			public const uint SHGFI_ICONLOCATION = 4096u;

			public const uint SHGFI_EXETYPE = 8192u;

			public const uint SHGFI_SYSICONINDEX = 16384u;

			public const uint SHGFI_LINKOVERLAY = 32768u;

			public const uint SHGFI_SELECTED = 65536u;

			public const uint SHGFI_ATTR_SPECIFIED = 131072u;

			public const uint SHGFI_LARGEICON = 0u;

			public const uint SHGFI_SMALLICON = 1u;

			public const uint SHGFI_OPENICON = 2u;

			public const uint SHGFI_SHELLICONSIZE = 4u;

			public const uint SHGFI_PIDL = 8u;

			public const uint SHGFI_USEFILEATTRIBUTES = 16u;

			public const uint SHGFI_ADDOVERLAYS = 32u;

			public const uint SHGFI_OVERLAYINDEX = 64u;

			public const uint FILE_ATTRIBUTE_DIRECTORY = 16u;

			public const uint FILE_ATTRIBUTE_NORMAL = 128u;

			[DllImport("Shell32.dll")]
			public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref ComposeForm.Shell32.SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
		}

		private class User32
		{
			[DllImport("User32.dll")]
			public static extern int DestroyIcon(IntPtr hIcon);
		}

		private Label mt_From;

		private TextBox m_pFrom;

		private Label mt_Subject;

		private TextBox m_pSubject;

		private Label mt_Attachments;

		private ListView m_pAttachments;

		private WRichEditEx m_pText;

		private Button m_pSend;

		private Button m_pCancel;

		private UserFolder m_pFolder;

		private byte[] m_Message;

		public byte[] Message
		{
			get
			{
				return this.m_Message;
			}
		}

		public ComposeForm(UserFolder folder)
		{
			this.m_pFolder = folder;
			this.InitializeComponent();
			this.m_pFrom.Text = string.Concat(new string[]
			{
				"\"",
				this.m_pFolder.User.VirtualServer.Server.UserName,
				"\" <",
				this.m_pFolder.User.VirtualServer.Server.UserName,
				"@localhost>"
			});
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(435, 390);
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Compose:";
			base.Icon = ResManager.GetIcon("write.ico");
			base.MaximizeBox = false;
			this.mt_From = new Label();
			this.mt_From.Size = new Size(78, 20);
			this.mt_From.Location = new Point(10, 10);
			this.mt_From.TextAlign = ContentAlignment.MiddleRight;
			this.mt_From.Text = "From:";
			this.m_pFrom = new TextBox();
			this.m_pFrom.Size = new Size(336, 20);
			this.m_pFrom.Location = new Point(90, 10);
			this.m_pFrom.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.mt_Subject = new Label();
			this.mt_Subject.Size = new Size(78, 20);
			this.mt_Subject.Location = new Point(10, 60);
			this.mt_Subject.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Subject.Text = "Subject:";
			this.m_pSubject = new TextBox();
			this.m_pSubject.Size = new Size(336, 20);
			this.m_pSubject.Location = new Point(90, 60);
			this.m_pSubject.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.mt_Attachments = new Label();
			this.mt_Attachments.Size = new Size(78, 20);
			this.mt_Attachments.Location = new Point(10, 85);
			this.mt_Attachments.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Attachments.Text = "Attachemnts:";
			this.m_pAttachments = new ListView();
			this.m_pAttachments.Size = new Size(336, 54);
			this.m_pAttachments.Location = new Point(90, 85);
			this.m_pAttachments.BorderStyle = BorderStyle.FixedSingle;
			this.m_pAttachments.View = View.SmallIcon;
			this.m_pAttachments.SmallImageList = new ImageList();
			this.m_pAttachments.MouseUp += new MouseEventHandler(this.m_pAttachments_MouseUp);
			this.m_pAttachments.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pText = new WRichEditEx();
			this.m_pText.Size = new Size(416, 200);
			this.m_pText.Location = new Point(10, 150);
			this.m_pText.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pSend = new Button();
			this.m_pSend.Size = new Size(70, 20);
			this.m_pSend.Location = new Point(280, 360);
			this.m_pSend.Text = "Send";
			this.m_pSend.Click += new EventHandler(this.m_pSend_ButtonPressed);
			this.m_pSend.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(355, 360);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_ButtonPressed);
			this.m_pCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			base.Controls.Add(this.mt_From);
			base.Controls.Add(this.m_pFrom);
			base.Controls.Add(this.mt_Subject);
			base.Controls.Add(this.m_pSubject);
			base.Controls.Add(this.mt_Attachments);
			base.Controls.Add(this.m_pAttachments);
			base.Controls.Add(this.m_pText);
			base.Controls.Add(this.m_pSend);
			base.Controls.Add(this.m_pCancel);
		}

		private void attach_Click(object sender, EventArgs e)
		{
			this.AddAttachment();
		}

		private void m_pAttachments_MouseUp(object sender, MouseEventArgs e)
		{
			string text = "Open";
			string text2 = "Add";
			string text3 = "Remove";
			if (e.Button == MouseButtons.Right && this.m_pAttachments.SelectedItems.Count > 0)
			{
				new ContextMenuStrip
				{
					Items = 
					{
						{
							text,
							ResManager.GetIcon("open.ico").ToBitmap(),
							new EventHandler(this.m_pAttachments_OpenClick)
						},
						new ToolStripSeparator(),
						{
							text2,
							ResManager.GetIcon("add.ico").ToBitmap(),
							new EventHandler(this.m_pAttachments_AddClick)
						},
						{
							text3,
							ResManager.GetIcon("delete.ico").ToBitmap(),
							new EventHandler(this.m_pAttachments_RemoveClick)
						}
					}
				}.Show(Control.MousePosition);
				return;
			}
			if (e.Button == MouseButtons.Right)
			{
				new ContextMenuStrip
				{
					Items = 
					{
						{
							text2,
							ResManager.GetIcon("add.ico").ToBitmap(),
							new EventHandler(this.m_pAttachments_AddClick)
						}
					}
				}.Show(Control.MousePosition);
			}
		}

		private void m_pAttachments_OpenClick(object sender, EventArgs e)
		{
			if (this.m_pAttachments.SelectedItems.Count > 0)
			{
				Process.Start(this.m_pAttachments.SelectedItems[0].Tag.ToString());
			}
		}

		private void m_pAttachments_AddClick(object sender, EventArgs e)
		{
			this.AddAttachment();
		}

		private void m_pAttachments_RemoveClick(object sender, EventArgs e)
		{
			foreach (ListViewItem listViewItem in this.m_pAttachments.SelectedItems)
			{
				listViewItem.Remove();
			}
		}

		private void m_pSend_ButtonPressed(object sender, EventArgs e)
		{
			if (this.m_pFrom.Text == "")
			{
				MessageBox.Show(this, "Please fill From: !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.m_pSubject.Text == "")
			{
				MessageBox.Show(this, "Please fill Subject: !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			this.m_Message = this.CreateMessage().ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		private void m_pCancel_ButtonPressed(object sender, EventArgs e)
		{
			base.Close();
		}

		private void AddAttachment()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.AddAttachment(openFileDialog.FileName);
			}
		}

		private void AddAttachment(string fileName)
		{
			string str = "";
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				if (fileStream.Length > 1000000L)
				{
					str = (fileStream.Length / 1000000m).ToString("f2") + " mb";
				}
				else if (fileStream.Length > 1000L)
				{
					str = (fileStream.Length / 1000m).ToString("f2") + " kb";
				}
				else
				{
					str = fileStream.Length.ToString() + " bytes";
				}
			}
			this.m_pAttachments.SmallImageList.Images.Add(this.GetFileIcon(fileName).ToBitmap());
			ListViewItem listViewItem = new ListViewItem(Path.GetFileName(fileName) + " (" + str + ")");
			listViewItem.ImageIndex = this.m_pAttachments.SmallImageList.Images.Count - 1;
			listViewItem.Tag = fileName;
			this.m_pAttachments.Items.Add(listViewItem);
		}

		private Icon GetFileIcon(string name)
		{
			Icon result;
			try
			{
				ComposeForm.Shell32.SHFILEINFO sHFILEINFO = default(ComposeForm.Shell32.SHFILEINFO);
				uint uFlags = 273u;
				ComposeForm.Shell32.SHGetFileInfo(name, 128u, ref sHFILEINFO, (uint)Marshal.SizeOf(sHFILEINFO), uFlags);
				Icon icon = (Icon)Icon.FromHandle(sHFILEINFO.hIcon).Clone();
				ComposeForm.User32.DestroyIcon(sHFILEINFO.hIcon);
				result = icon;
			}
			catch
			{
				result = ResManager.GetIcon("attach");
			}
			return result;
		}

		private Mail_Message CreateMessage()
		{
			Mail_Message mail_Message = new Mail_Message();
			mail_Message.MimeVersion = "1.0";
			mail_Message.MessageID = MIME_Utils.CreateMessageID();
			mail_Message.Date = DateTime.Now;
			mail_Message.From = Mail_h_MailboxList.Parse("From: " + this.m_pFrom.Text).Addresses;
			mail_Message.To = new Mail_t_AddressList();
			mail_Message.To.Add(new Mail_t_Mailbox(this.m_pFolder.User.FullName, this.m_pFolder.User.FullName + "@localhost"));
			mail_Message.Subject = this.m_pSubject.Text;
			MIME_b_MultipartMixed mIME_b_MultipartMixed = new MIME_b_MultipartMixed(new MIME_h_ContentType(MIME_MediaTypes.Multipart.mixed)
			{
				Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.')
			});
			mail_Message.Body = mIME_b_MultipartMixed;
			MIME_Entity mIME_Entity = new MIME_Entity();
			MIME_b_MultipartAlternative mIME_b_MultipartAlternative = new MIME_b_MultipartAlternative(new MIME_h_ContentType(MIME_MediaTypes.Multipart.alternative)
			{
				Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.')
			});
			mIME_Entity.Body = mIME_b_MultipartAlternative;
			mIME_b_MultipartMixed.BodyParts.Add(mIME_Entity);
			MIME_Entity mIME_Entity2 = new MIME_Entity();
			MIME_b_Text mIME_b_Text = new MIME_b_Text(MIME_MediaTypes.Text.plain);
			mIME_Entity2.Body = mIME_b_Text;
			mIME_b_Text.SetText(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, this.m_pText.Text);
			mIME_b_MultipartAlternative.BodyParts.Add(mIME_Entity2);
			MIME_Entity mIME_Entity3 = new MIME_Entity();
			MIME_b_Text mIME_b_Text2 = new MIME_b_Text(MIME_MediaTypes.Text.html);
			mIME_Entity3.Body = mIME_b_Text2;
			mIME_b_Text2.SetText(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, this.RtfToHtml());
			mIME_b_MultipartAlternative.BodyParts.Add(mIME_Entity3);
			foreach (ListViewItem listViewItem in this.m_pAttachments.Items)
			{
				mIME_b_MultipartMixed.BodyParts.Add(MIME_Message.CreateAttachment(listViewItem.Tag.ToString()));
			}
			return mail_Message;
		}

		private string RtfToHtml()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<html>\r\n");
			this.m_pText.SuspendPaint = true;
			this.m_pText.RichTextBox.SelectionStart = 0;
			this.m_pText.RichTextBox.SelectionLength = 1;
			Font selectionFont = this.m_pText.RichTextBox.SelectionFont;
			Color selectionColor = this.m_pText.RichTextBox.SelectionColor;
			Color selectionBackColor = this.m_pText.RichTextBox.SelectionBackColor;
			int num = 0;
			int num2 = 0;
			while (this.m_pText.RichTextBox.Text.Length > this.m_pText.RichTextBox.SelectionStart)
			{
				this.m_pText.RichTextBox.SelectionStart++;
				this.m_pText.RichTextBox.SelectionLength = 1;
				if (this.m_pText.RichTextBox.Text.Length == this.m_pText.RichTextBox.SelectionStart || selectionFont.Name != this.m_pText.RichTextBox.SelectionFont.Name || selectionFont.Size != this.m_pText.RichTextBox.SelectionFont.Size || selectionFont.Style != this.m_pText.RichTextBox.SelectionFont.Style || selectionColor != this.m_pText.RichTextBox.SelectionColor || selectionBackColor != this.m_pText.RichTextBox.SelectionBackColor)
				{
					string text = this.m_pText.RichTextBox.Text.Substring(num2, this.m_pText.RichTextBox.SelectionStart - num2);
					string text2 = "#" + selectionColor.R.ToString("X2") + selectionColor.G.ToString("X2") + selectionColor.B.ToString("X2");
					string text3 = "#" + selectionBackColor.R.ToString("X2") + selectionBackColor.G.ToString("X2") + selectionBackColor.B.ToString("X2");
					string text4 = "";
					string text5 = "";
					if (selectionFont.Bold)
					{
						text4 += "<b>";
						text5 += "</b>";
					}
					if (selectionFont.Italic)
					{
						text4 += "<i>";
						text5 += "</i>";
					}
					if (selectionFont.Underline)
					{
						text4 += "<u>";
						text5 += "</u>";
					}
					stringBuilder.Append(string.Concat(new object[]
					{
						"<span\n style=\"color:",
						text2,
						"; font-size:",
						selectionFont.Size,
						"pt; font-family:",
						selectionFont.FontFamily.Name,
						"; background-color:",
						text3,
						";\">",
						text4,
						text.Replace("\n", "</br>"),
						text5
					}));
					num2 = this.m_pText.RichTextBox.SelectionStart;
					selectionFont = this.m_pText.RichTextBox.SelectionFont;
					selectionColor = this.m_pText.RichTextBox.SelectionColor;
					selectionBackColor = this.m_pText.RichTextBox.SelectionBackColor;
					num++;
				}
			}
			for (int i = 0; i < num; i++)
			{
				stringBuilder.Append("</span>");
			}
			stringBuilder.Append("\r\n</html>\r\n");
			this.m_pText.SuspendPaint = false;
			return stringBuilder.ToString();
		}
	}
}

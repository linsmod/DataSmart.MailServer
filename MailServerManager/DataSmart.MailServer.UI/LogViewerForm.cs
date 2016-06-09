using System.NetworkToolkit;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class LogViewerForm : Form
	{
		private Label mt_SessionStartTime;

		private TextBox m_pSessionStartTime;

		private Label mt_UserName;

		private TextBox m_pUserName;

		private Label mt_SessionID;

		private TextBox m_pSessionID;

		private Label mt_RemoteEndPoint;

		private TextBox m_pRemoteEndPoint;

		private RichTextBox m_pLogText;

		public LogViewerForm(string logText, string highlightWord)
		{
			this.InitializeComponent();
			this.LoadLogText(logText, highlightWord);
		}

		private void InitializeComponent()
		{
			base.Size = new Size(500, 400);
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Log Viewer";
			this.mt_SessionStartTime = new Label();
			this.mt_SessionStartTime.Size = new Size(120, 20);
			this.mt_SessionStartTime.Location = new Point(0, 20);
			this.mt_SessionStartTime.TextAlign = ContentAlignment.MiddleRight;
			this.mt_SessionStartTime.Text = "Session Start:";
			this.m_pSessionStartTime = new TextBox();
			this.m_pSessionStartTime.Size = new Size(250, 20);
			this.m_pSessionStartTime.Location = new Point(125, 20);
			this.m_pSessionStartTime.ReadOnly = true;
			this.mt_UserName = new Label();
			this.mt_UserName.Size = new Size(120, 20);
			this.mt_UserName.Location = new Point(0, 45);
			this.mt_UserName.TextAlign = ContentAlignment.MiddleRight;
			this.mt_UserName.Text = "Authenticated User:";
			this.m_pUserName = new TextBox();
			this.m_pUserName.Size = new Size(250, 20);
			this.m_pUserName.Location = new Point(125, 45);
			this.m_pUserName.ReadOnly = true;
			this.mt_SessionID = new Label();
			this.mt_SessionID.Size = new Size(120, 20);
			this.mt_SessionID.Location = new Point(0, 70);
			this.mt_SessionID.TextAlign = ContentAlignment.MiddleRight;
			this.mt_SessionID.Text = "Session ID:";
			this.m_pSessionID = new TextBox();
			this.m_pSessionID.Size = new Size(250, 20);
			this.m_pSessionID.Location = new Point(125, 70);
			this.m_pSessionID.ReadOnly = true;
			this.mt_RemoteEndPoint = new Label();
			this.mt_RemoteEndPoint.Size = new Size(120, 20);
			this.mt_RemoteEndPoint.Location = new Point(0, 95);
			this.mt_RemoteEndPoint.TextAlign = ContentAlignment.MiddleRight;
			this.mt_RemoteEndPoint.Text = "Remote End Point:";
			this.m_pRemoteEndPoint = new TextBox();
			this.m_pRemoteEndPoint.Size = new Size(250, 20);
			this.m_pRemoteEndPoint.Location = new Point(125, 95);
			this.m_pRemoteEndPoint.ReadOnly = true;
			this.m_pLogText = new RichTextBox();
			this.m_pLogText.Size = new Size(483, 225);
			this.m_pLogText.Location = new Point(5, 125);
			this.m_pLogText.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pLogText.ReadOnly = true;
			base.Controls.Add(this.mt_SessionStartTime);
			base.Controls.Add(this.m_pSessionStartTime);
			base.Controls.Add(this.mt_UserName);
			base.Controls.Add(this.m_pUserName);
			base.Controls.Add(this.mt_SessionID);
			base.Controls.Add(this.m_pSessionID);
			base.Controls.Add(this.mt_RemoteEndPoint);
			base.Controls.Add(this.m_pRemoteEndPoint);
			base.Controls.Add(this.m_pLogText);
		}

		private void LoadLogText(string logText, string highlightWord)
		{
			string[] array = logText.Replace("\r", "").Split(new char[]
			{
				'\n'
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				string[] array3 = TextUtils.SplitQuotedString(text, '\t', true);
				if (array3.Length == 6)
				{
					if (this.m_pSessionStartTime.Text.Length == 0)
					{
						this.m_pSessionStartTime.Text = array3[1];
					}
					if (this.m_pUserName.Text.Length == 0)
					{
						this.m_pUserName.Text = array3[3];
					}
					if (this.m_pSessionID.Text.Length == 0)
					{
						this.m_pSessionID.Text = array3[0];
					}
					if (this.m_pRemoteEndPoint.Text.Length == 0)
					{
						this.m_pRemoteEndPoint.Text = array3[2];
					}
					string text2 = array3[4];
					string text3 = "\"" + array3[5] + "\"";
					this.m_pLogText.AppendText(text2);
					if (Environment.OSVersion.Platform != PlatformID.Unix)
					{
						this.m_pLogText.SelectionStart = this.m_pLogText.Text.Length - text2.Length;
						this.m_pLogText.SelectionLength = text2.Length;
						this.m_pLogText.SelectionColor = Color.DarkGreen;
					}
					this.m_pLogText.AppendText("  ");
					this.m_pLogText.AppendText(text3);
					if (Environment.OSVersion.Platform != PlatformID.Unix)
					{
						this.m_pLogText.SelectionStart = this.m_pLogText.Text.Length - text3.Length;
						this.m_pLogText.SelectionLength = text3.Length;
						this.m_pLogText.SelectionColor = Color.DarkMagenta;
						if (highlightWord.Length > 0 && text3.IndexOf(highlightWord) > -1)
						{
							this.m_pLogText.SelectionStart = this.m_pLogText.Text.Length - text3.Length + text3.IndexOf(highlightWord);
							this.m_pLogText.SelectionLength = highlightWord.Length;
							this.m_pLogText.SelectionFont = new Font(this.m_pLogText.Font, FontStyle.Bold);
							this.m_pLogText.SelectionColor = Color.Red;
						}
					}
					this.m_pLogText.AppendText("\n");
				}
			}
		}
	}
}

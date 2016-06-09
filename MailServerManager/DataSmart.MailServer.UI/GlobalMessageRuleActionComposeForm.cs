using System.NetworkToolkit.Mail;
using System.NetworkToolkit.MIME;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class GlobalMessageRuleActionComposeForm : Form
	{
		private TextBox m_pFrom;

		private Label mt_From;

		private Label mt_To;

		private TextBox m_pTo;

		private Label mt_Subject;

		private TextBox m_pSubject;

		private TextBox m_pBodyText;

		private GroupBox m_pGroupBox1;

		private Button m_pCancel;

		private Button m_pOk;

		private string m_Message = "";

		public string Message
		{
			get
			{
				return this.m_Message;
			}
		}

		public GlobalMessageRuleActionComposeForm()
		{
			this.InitializeComponent();
			this.m_pSubject.Text = "Auto Response: #SUBJECT";
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(498, 372);
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Compose Message";
			this.mt_From = new Label();
			this.mt_From.Size = new Size(86, 23);
			this.mt_From.Location = new Point(9, 13);
			this.mt_From.Text = "From:";
			this.m_pFrom = new TextBox();
			this.m_pFrom.Size = new Size(385, 20);
			this.m_pFrom.Location = new Point(101, 16);
			this.mt_To = new Label();
			this.mt_To.Size = new Size(86, 23);
			this.mt_To.Location = new Point(12, 40);
			this.mt_To.Text = "To:";
			this.m_pTo = new TextBox();
			this.m_pTo.Size = new Size(385, 20);
			this.m_pTo.Location = new Point(101, 42);
			this.mt_Subject = new Label();
			this.mt_Subject.Size = new Size(86, 23);
			this.mt_Subject.Location = new Point(9, 68);
			this.mt_Subject.Text = "Subject:";
			this.m_pSubject = new TextBox();
			this.m_pSubject.Size = new Size(385, 20);
			this.m_pSubject.Location = new Point(101, 68);
			this.m_pBodyText = new TextBox();
			this.m_pBodyText.Size = new Size(474, 207);
			this.m_pBodyText.Location = new Point(12, 104);
			this.m_pBodyText.AcceptsReturn = true;
			this.m_pBodyText.AcceptsTab = true;
			this.m_pBodyText.Multiline = true;
			this.m_pGroupBox1 = new GroupBox();
			this.m_pGroupBox1.Size = new Size(505, 4);
			this.m_pGroupBox1.Location = new Point(1, 327);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(67, 20);
			this.m_pCancel.Location = new Point(346, 340);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(67, 20);
			this.m_pOk.Location = new Point(419, 340);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.mt_From);
			base.Controls.Add(this.m_pFrom);
			base.Controls.Add(this.mt_To);
			base.Controls.Add(this.m_pTo);
			base.Controls.Add(this.mt_Subject);
			base.Controls.Add(this.m_pSubject);
			base.Controls.Add(this.m_pBodyText);
			base.Controls.Add(this.m_pGroupBox1);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			Mail_Message mail_Message = new Mail_Message();
			mail_Message.MimeVersion = "1.0";
			mail_Message.MessageID = MIME_Utils.CreateMessageID();
			mail_Message.Date = DateTime.Now;
			mail_Message.From = Mail_h_MailboxList.Parse("From: " + this.m_pFrom.Text).Addresses;
			if (!string.IsNullOrEmpty(this.m_pTo.Text))
			{
				mail_Message.To = Mail_h_AddressList.Parse("To: " + this.m_pTo.Text).Addresses;
			}
			mail_Message.Subject = this.m_pSubject.Text;
			MIME_b_Text mIME_b_Text = new MIME_b_Text(MIME_MediaTypes.Text.plain);
			mail_Message.Body = mIME_b_Text;
			mIME_b_Text.SetText(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, this.m_pBodyText.Text);
			this.m_Message = mail_Message.ToString(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
			base.DialogResult = DialogResult.OK;
		}
	}
}

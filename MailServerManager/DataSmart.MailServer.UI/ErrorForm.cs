using DataSmart.MailServer.UI.Resources;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class ErrorForm : Form
	{
		private TextBox m_pMessage;

		private PictureBox m_pImage;

		private GroupBox m_pGroupbox1;

		private Button m_pToggleExtended;

		private Button m_pClose;

		private TextBox m_pExtendedMessage;

		public ErrorForm(Exception x, StackTrace stack)
		{
			this.InitializeComponent();
			base.ClientSize = new Size(492, 168);
			this.m_pMessage.Text = x.Message;
			string text = "Message: " + x.Message + "\r\n";
			string text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				"Method: ",
				stack.GetFrame(0).GetMethod().DeclaringType.FullName,
				".",
				stack.GetFrame(0).GetMethod().Name,
				"()\r\n\r\n"
			});
			text = text + "Stack:\r\n" + x.StackTrace;
			this.m_pExtendedMessage.Text = text;
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(492, 373);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Error:";
			this.m_pMessage = new TextBox();
			this.m_pMessage.Size = new Size(240, 100);
			this.m_pMessage.Location = new Point(10, 10);
			this.m_pMessage.Multiline = true;
			this.m_pMessage.ReadOnly = true;
			this.m_pMessage.BorderStyle = BorderStyle.FixedSingle;
			this.m_pMessage.ScrollBars = ScrollBars.Horizontal;
			this.m_pImage = new PictureBox();
			this.m_pImage.Size = new Size(200, 100);
			this.m_pImage.Location = new Point(280, 10);
			this.m_pImage.SizeMode = PictureBoxSizeMode.StretchImage;
			this.m_pImage.Image = ResManager.GetImage("error.jpg");
			this.m_pGroupbox1 = new GroupBox();
			this.m_pGroupbox1.Size = new Size(485, 3);
			this.m_pGroupbox1.Location = new Point(5, 125);
			this.m_pToggleExtended = new Button();
			this.m_pToggleExtended.Size = new Size(70, 20);
			this.m_pToggleExtended.Location = new Point(335, 140);
			this.m_pToggleExtended.Text = "More";
			this.m_pToggleExtended.Click += new EventHandler(this.m_pToggleExtended_Click);
			this.m_pClose = new Button();
			this.m_pClose.Size = new Size(70, 20);
			this.m_pClose.Location = new Point(410, 140);
			this.m_pClose.Text = "Close";
			this.m_pClose.Click += new EventHandler(this.m_pClose_Click);
			this.m_pExtendedMessage = new TextBox();
			this.m_pExtendedMessage.Size = new Size(470, 185);
			this.m_pExtendedMessage.Location = new Point(10, 170);
			this.m_pExtendedMessage.Multiline = true;
			this.m_pExtendedMessage.ReadOnly = true;
			this.m_pExtendedMessage.BorderStyle = BorderStyle.FixedSingle;
			this.m_pExtendedMessage.ScrollBars = ScrollBars.Both;
			base.Controls.Add(this.m_pMessage);
			base.Controls.Add(this.m_pImage);
			base.Controls.Add(this.m_pGroupbox1);
			base.Controls.Add(this.m_pToggleExtended);
			base.Controls.Add(this.m_pClose);
			base.Controls.Add(this.m_pExtendedMessage);
		}

		private void m_pToggleExtended_Click(object sender, EventArgs e)
		{
			if (this.m_pToggleExtended.Text == "More")
			{
				this.m_pToggleExtended.Text = "Less";
				base.ClientSize = new Size(492, 373);
				return;
			}
			this.m_pToggleExtended.Text = "More";
			base.ClientSize = new Size(492, 168);
		}

		private void m_pClose_Click(object sender, EventArgs e)
		{
			base.Close();
		}
	}
}

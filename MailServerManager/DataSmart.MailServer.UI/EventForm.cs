using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class EventForm : Form
	{
		private PictureBox m_pImage;

		private Label mt_CreateDate;

		private Label m_pCreateDate;

		private Label mt_Type;

		private Label m_pType;

		private Label mt_Description;

		private TextBox m_pText;

		private Button m_pClose;

		public EventForm(DateTime date, string text)
		{
			this.InitializeComponent();
			base.Icon = ResManager.GetIcon("error.ico");
			this.m_pImage.Image = ResManager.GetIcon("error.ico").ToBitmap();
			this.m_pCreateDate.Text = date.ToString();
			this.m_pText.Text = text;
			this.m_pText.SelectionStart = 0;
			this.m_pText.SelectionLength = 0;
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(492, 373);
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Event info:";
			this.m_pImage = new PictureBox();
			this.m_pImage.Size = new Size(36, 36);
			this.m_pImage.Location = new Point(9, 15);
			this.mt_CreateDate = new Label();
			this.mt_CreateDate.Size = new Size(100, 20);
			this.mt_CreateDate.Location = new Point(9, 15);
			this.mt_CreateDate.TextAlign = ContentAlignment.MiddleRight;
			this.mt_CreateDate.Text = "Create Date:";
			this.m_pCreateDate = new Label();
			this.m_pCreateDate.Size = new Size(150, 20);
			this.m_pCreateDate.Location = new Point(115, 15);
			this.m_pCreateDate.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Type = new Label();
			this.mt_Type.Size = new Size(100, 20);
			this.mt_Type.Location = new Point(9, 35);
			this.mt_Type.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Type.Text = "Type:";
			this.m_pType = new Label();
			this.m_pType.Size = new Size(100, 20);
			this.m_pType.Location = new Point(115, 35);
			this.m_pType.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pType.Text = "Error";
			this.mt_Description = new Label();
			this.mt_Description.Size = new Size(100, 20);
			this.mt_Description.Location = new Point(9, 65);
			this.mt_Description.Text = "Desciption:";
			this.m_pText = new TextBox();
			this.m_pText.Size = new Size(475, 240);
			this.m_pText.Location = new Point(9, 85);
			this.m_pText.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pText.Multiline = true;
			this.m_pText.ReadOnly = true;
			this.m_pClose = new Button();
			this.m_pClose.Size = new Size(70, 20);
			this.m_pClose.Location = new Point(412, 340);
			this.m_pClose.Text = "Close";
			this.m_pClose.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pClose.Click += new EventHandler(this.m_pClose_Click);
			base.Controls.Add(this.m_pImage);
			base.Controls.Add(this.mt_CreateDate);
			base.Controls.Add(this.m_pCreateDate);
			base.Controls.Add(this.mt_Type);
			base.Controls.Add(this.m_pType);
			base.Controls.Add(this.mt_Description);
			base.Controls.Add(this.m_pText);
			base.Controls.Add(this.m_pClose);
		}

		private void m_pClose_Click(object sender, EventArgs e)
		{
			base.Close();
		}
	}
}

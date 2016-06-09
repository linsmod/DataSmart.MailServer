using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class NotificationForm : Control
	{
		private Panel m_pPanel;

		private PictureBox m_pIcon;

		private RichTextBox m_pText;

		public Image Icon
		{
			get
			{
				return this.m_pIcon.Image;
			}
			set
			{
				this.m_pIcon.Image = value;
			}
		}

		public override string Text
		{
			get
			{
				return this.m_pText.Text;
			}
			set
			{
				this.m_pText.Text = value;
			}
		}

		public NotificationForm()
		{
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			base.Size = new Size(100, 38);
			this.MinimumSize = new Size(100, 38);
			this.m_pPanel = new Panel();
			this.m_pPanel.Size = base.ClientSize;
			this.m_pPanel.Location = new Point(0, 0);
			this.m_pPanel.Dock = DockStyle.Fill;
			this.m_pPanel.BorderStyle = BorderStyle.FixedSingle;
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(36, 36);
			this.m_pIcon.Location = new Point(4, 1);
			this.m_pText = new RichTextBox();
			this.m_pText.Size = new Size(55, 36);
			this.m_pText.Location = new Point(45, 1);
			this.m_pText.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pText.ReadOnly = true;
			this.m_pText.ForeColor = Color.Gray;
			this.m_pText.ScrollBars = RichTextBoxScrollBars.Vertical;
			this.m_pText.BorderStyle = BorderStyle.None;
			this.m_pPanel.Controls.Add(this.m_pIcon);
			this.m_pPanel.Controls.Add(this.m_pText);
			base.Controls.Add(this.m_pPanel);
		}
	}
}

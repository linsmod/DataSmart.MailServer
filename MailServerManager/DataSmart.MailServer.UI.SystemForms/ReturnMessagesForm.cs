using DataSmart.MailServer.Management;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SystemForms
{
	public class ReturnMessagesForm : Form
	{
		private Label mt_MessageType;

		private ComboBox m_pMessageType;

		private Label mt_Subject;

		private TextBox m_pSubject;

		private WRichEditEx m_pText;

		private Button m_pHelp;

		private Button m_pSave;

		private VirtualServer m_pVirtualServer;

		private WComboBoxItem m_pCurrentMessageType;

		public ReturnMessagesForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.m_pMessageType.SelectedIndex = 0;
		}

		private void InitializeComponent()
		{
			base.Size = new Size(400, 400);
			this.mt_MessageType = new Label();
			this.mt_MessageType.Size = new Size(100, 20);
			this.mt_MessageType.Location = new Point(0, 20);
			this.mt_MessageType.TextAlign = ContentAlignment.MiddleRight;
			this.mt_MessageType.Text = "Message Type:";
			this.m_pMessageType = new ComboBox();
			this.m_pMessageType.Size = new Size(200, 20);
			this.m_pMessageType.Location = new Point(105, 20);
			this.m_pMessageType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pMessageType.Items.Add(new WComboBoxItem("Delayed delivery warning", "delayed_delivery_warning"));
			this.m_pMessageType.Items.Add(new WComboBoxItem("Undelivered notice", "undelivered"));
			this.m_pMessageType.SelectedIndexChanged += new EventHandler(this.m_pMessageType_SelectedIndexChanged);
			this.mt_Subject = new Label();
			this.mt_Subject.Size = new Size(100, 20);
			this.mt_Subject.Location = new Point(0, 65);
			this.mt_Subject.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Subject.Text = "Subject:";
			this.m_pSubject = new TextBox();
			this.m_pSubject.Size = new Size(280, 20);
			this.m_pSubject.Location = new Point(105, 65);
			this.m_pSubject.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pText = new WRichEditEx();
			this.m_pText.Size = new Size(375, 240);
			this.m_pText.Location = new Point(10, 90);
			this.m_pText.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pHelp = new Button();
			this.m_pHelp.Size = new Size(70, 20);
			this.m_pHelp.Location = new Point(10, 340);
			this.m_pHelp.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.m_pHelp.Text = "Help";
			this.m_pHelp.Click += new EventHandler(this.m_pHelp_Click);
			this.m_pSave = new Button();
			this.m_pSave.Size = new Size(70, 20);
			this.m_pSave.Location = new Point(315, 340);
			this.m_pSave.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pSave.Text = "Save";
			this.m_pSave.Click += new EventHandler(this.m_pSave_Click);
			base.Controls.Add(this.mt_MessageType);
			base.Controls.Add(this.m_pMessageType);
			base.Controls.Add(this.mt_Subject);
			base.Controls.Add(this.m_pSubject);
			base.Controls.Add(this.m_pText);
			base.Controls.Add(this.m_pHelp);
			base.Controls.Add(this.m_pSave);
		}

		private void m_pMessageType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pMessageType.SelectedItem == null)
			{
				return;
			}
			if (this.m_pCurrentMessageType != null)
			{
				if (this.m_pCurrentMessageType.Tag.ToString() == "delayed_delivery_warning")
				{
					this.m_pVirtualServer.SystemSettings.ReturnMessages.DelayedDeliveryWarning = new ServerReturnMessage(this.m_pSubject.Text, this.m_pText.Rtf);
				}
				else if (this.m_pCurrentMessageType.Tag.ToString() == "undelivered")
				{
					this.m_pVirtualServer.SystemSettings.ReturnMessages.Undelivered = new ServerReturnMessage(this.m_pSubject.Text, this.m_pText.Rtf);
				}
			}
			this.m_pCurrentMessageType = (WComboBoxItem)this.m_pMessageType.SelectedItem;
			if (this.m_pCurrentMessageType.Tag.ToString() == "delayed_delivery_warning")
			{
				this.m_pSubject.Text = this.m_pVirtualServer.SystemSettings.ReturnMessages.DelayedDeliveryWarning.Subject;
				this.m_pText.Rtf = this.m_pVirtualServer.SystemSettings.ReturnMessages.DelayedDeliveryWarning.BodyTextRtf;
				return;
			}
			if (this.m_pCurrentMessageType.Tag.ToString() == "undelivered")
			{
				this.m_pSubject.Text = this.m_pVirtualServer.SystemSettings.ReturnMessages.Undelivered.Subject;
				this.m_pText.Rtf = this.m_pVirtualServer.SystemSettings.ReturnMessages.Undelivered.BodyTextRtf;
			}
		}

		private void m_pHelp_Click(object sender, EventArgs e)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo("explorer", Application.StartupPath + "\\Help\\System.ReturnMessages.txt");
			Process.Start(startInfo);
		}

		private void m_pSave_Click(object sender, EventArgs e)
		{
			if (this.m_pCurrentMessageType.Tag.ToString() == "delayed_delivery_warning")
			{
				this.m_pVirtualServer.SystemSettings.ReturnMessages.DelayedDeliveryWarning = new ServerReturnMessage(this.m_pSubject.Text, this.m_pText.Rtf);
			}
			else if (this.m_pCurrentMessageType.Tag.ToString() == "undelivered")
			{
				this.m_pVirtualServer.SystemSettings.ReturnMessages.Undelivered = new ServerReturnMessage(this.m_pSubject.Text, this.m_pText.Rtf);
			}
			this.m_pVirtualServer.SystemSettings.Commit();
		}
	}
}

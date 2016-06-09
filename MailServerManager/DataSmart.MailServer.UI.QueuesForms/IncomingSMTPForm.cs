using DataSmart.MailServer.Management;
using System.NetworkToolkit.MIME;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.QueryForms
{
	public class IncomingSMTPForm : Form
	{
		private Button m_pGet;

		private ListView m_pQueue;

		private VirtualServer m_pVirtualServer;

		public IncomingSMTPForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			base.Size = new Size(470, 350);
			this.m_pGet = new Button();
			this.m_pGet.Size = new Size(70, 20);
			this.m_pGet.Location = new Point(385, 30);
			this.m_pGet.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pGet.Text = "Get";
			this.m_pGet.Click += new EventHandler(this.m_pGet_Click);
			this.m_pQueue = new ListView();
			this.m_pQueue.Size = new Size(445, 240);
			this.m_pQueue.Location = new Point(10, 60);
			this.m_pQueue.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pQueue.View = View.Details;
			this.m_pQueue.HideSelection = false;
			this.m_pQueue.FullRowSelect = true;
			this.m_pQueue.Columns.Add("Created", 115, HorizontalAlignment.Left);
			this.m_pQueue.Columns.Add("From", 100, HorizontalAlignment.Left);
			this.m_pQueue.Columns.Add("To", 100, HorizontalAlignment.Left);
			this.m_pQueue.Columns.Add("Subject", 150, HorizontalAlignment.Left);
			base.Controls.Add(this.m_pGet);
			base.Controls.Add(this.m_pQueue);
		}

		private void m_pGet_Click(object sender, EventArgs e)
		{
			this.m_pQueue.Items.Clear();
			this.m_pVirtualServer.Queues.SMTP.Refresh();
			foreach (QueueItem queueItem in this.m_pVirtualServer.Queues.SMTP)
			{
				MIME_h_Collection mIME_h_Collection = new MIME_h_Collection(new MIME_h_Provider());
				mIME_h_Collection.Parse(queueItem.Header);
				string text = "";
				if (mIME_h_Collection.GetFirst("From") != null)
				{
					text = mIME_h_Collection.GetFirst("From").ToString().Split(new char[]
					{
						':'
					}, 2)[1];
				}
				string text2 = "";
				if (mIME_h_Collection.GetFirst("To") != null)
				{
					text2 = mIME_h_Collection.GetFirst("To").ToString().Split(new char[]
					{
						':'
					}, 2)[1];
				}
				string text3 = "";
				if (mIME_h_Collection.GetFirst("Subject") != null)
				{
					text3 = mIME_h_Collection.GetFirst("Subject").ToString().Split(new char[]
					{
						':'
					}, 2)[1];
				}
				ListViewItem listViewItem = new ListViewItem();
				listViewItem.Text = queueItem.CreateTime.ToString();
				listViewItem.SubItems.Add(text);
				listViewItem.SubItems.Add(text2);
				listViewItem.SubItems.Add(text3);
				this.m_pQueue.Items.Add(listViewItem);
			}
		}
	}
}

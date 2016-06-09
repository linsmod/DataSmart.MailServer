using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class EventsForm : Form
	{
		private Button m_pClearAllEvents;

		private ImageList m_pEventsImages;

		private WListView m_pEvents;

		private Server m_pServer;

		public EventsForm(Server server, WFrame frame)
		{
			this.m_pServer = server;
			this.InitializeComponent();
			this.LoadEvents();
		}

		private void InitializeComponent()
		{
			base.Size = new Size(450, 300);
			this.m_pClearAllEvents = new Button();
			this.m_pClearAllEvents.Size = new Size(100, 20);
			this.m_pClearAllEvents.Location = new Point(9, 15);
			this.m_pClearAllEvents.Text = "Clear all Events";
			this.m_pClearAllEvents.Click += new EventHandler(this.m_pClearAllEvents_Click);
			this.m_pEventsImages = new ImageList();
			this.m_pEventsImages.Images.Add(ResManager.GetIcon("error.ico"));
			this.m_pEvents = new WListView();
			this.m_pEvents.Size = new Size(425, 210);
			this.m_pEvents.Location = new Point(9, 47);
			this.m_pEvents.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pEvents.View = View.Details;
			this.m_pEvents.FullRowSelect = true;
			this.m_pEvents.HideSelection = false;
			this.m_pEvents.SmallImageList = this.m_pEventsImages;
			this.m_pEvents.DoubleClick += new EventHandler(this.m_pEvents_DoubleClick);
			this.m_pEvents.Columns.Add("", 20, HorizontalAlignment.Left);
			this.m_pEvents.Columns.Add("Virtual Server", 120, HorizontalAlignment.Left);
			this.m_pEvents.Columns.Add("Date", 130, HorizontalAlignment.Left);
			this.m_pEvents.Columns.Add("Text", 200, HorizontalAlignment.Left);
			base.Controls.Add(this.m_pClearAllEvents);
			base.Controls.Add(this.m_pEvents);
		}

		private void m_pClearAllEvents_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Are you sure you want to delete all events", "Confirmation:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				this.m_pServer.Events.Clear();
				this.m_pEvents.Items.Clear();
			}
		}

		private void m_pEvents_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pEvents.SelectedItems.Count > 0)
			{
				Event @event = (Event)this.m_pEvents.SelectedItems[0].Tag;
				EventForm eventForm = new EventForm(@event.CreateDate, @event.Text);
				eventForm.ShowDialog(this);
			}
		}

		private void LoadEvents()
		{
			this.m_pServer.Events.Refresh();
			foreach (Event @event in this.m_pServer.Events)
			{
				ListViewItem listViewItem = new ListViewItem();
				listViewItem.ImageIndex = 0;
				listViewItem.SubItems.Add(@event.VirtualServer);
				listViewItem.SubItems.Add(@event.CreateDate.ToString());
				listViewItem.SubItems.Add(@event.Text.ToString().Split(new char[]
				{
					'\n'
				}, 2)[0]);
				listViewItem.Tag = @event;
				this.m_pEvents.Items.Add(listViewItem);
			}
		}
	}
}

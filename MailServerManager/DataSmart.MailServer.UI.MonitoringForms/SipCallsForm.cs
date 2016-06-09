using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.MonitoringForms
{
	public class SipCallsForm : Form
	{
		private ToolStrip m_pToolbar;

		private WListView m_pCalls;

		private Server m_pServer;

		public SipCallsForm(Server server, WFrame frame)
		{
			this.m_pServer = server;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadData();
		}

		private void InitializeComponent()
		{
			base.Size = new Size(472, 357);
			this.m_pToolbar = new ToolStrip();
			this.m_pToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pToolbar.BackColor = this.BackColor;
			this.m_pToolbar.Renderer = new ToolBarRendererEx();
			this.m_pToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pToolbar_ItemClicked);
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Enabled = false;
			toolStripButton.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton.Tag = "delete";
			toolStripButton.ToolTipText = "Delete";
			this.m_pToolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
			toolStripButton2.Tag = "refresh";
			toolStripButton2.ToolTipText = "Refresh";
			this.m_pToolbar.Items.Add(toolStripButton2);
			this.m_pCalls = new WListView();
			this.m_pCalls.Size = new Size(445, 265);
			this.m_pCalls.Location = new Point(9, 47);
			this.m_pCalls.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pCalls.View = View.Details;
			this.m_pCalls.FullRowSelect = true;
			this.m_pCalls.HideSelection = false;
			this.m_pCalls.Columns.Add("Caller", 180, HorizontalAlignment.Left);
			this.m_pCalls.Columns.Add("Callee", 180, HorizontalAlignment.Left);
			this.m_pCalls.Columns.Add("Start Time", 80, HorizontalAlignment.Left);
			this.m_pCalls.SelectedIndexChanged += new EventHandler(this.m_pCalls_SelectedIndexChanged);
			base.Controls.Add(this.m_pCalls);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "delete")
			{
				SipCall sipCall = (SipCall)this.m_pCalls.SelectedItems[0].Tag;
				if (MessageBox.Show(this, string.Concat(new string[]
				{
					"Are you sure you want to terminate call '",
					sipCall.Caller,
					"->",
					sipCall.Callee,
					"' ?"
				}), "Remove Registration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					sipCall.Terminate();
					this.m_pCalls.SelectedItems[0].Remove();
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "refresh")
			{
				this.LoadData();
			}
		}

		private void m_pCalls_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pCalls.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[0].Enabled = true;
				this.m_pToolbar.Items[0].Enabled = true;
				return;
			}
			this.m_pToolbar.Items[0].Enabled = false;
			this.m_pToolbar.Items[0].Enabled = false;
		}

		private void LoadData()
		{
			this.m_pCalls.Items.Clear();
			foreach (VirtualServer virtualServer in this.m_pServer.VirtualServers)
			{
				virtualServer.SipCalls.Refresh();
				foreach (SipCall sipCall in virtualServer.SipCalls)
				{
					ListViewItem listViewItem = new ListViewItem(sipCall.Caller);
					listViewItem.SubItems.Add(sipCall.Callee);
					listViewItem.SubItems.Add(sipCall.StartTime.ToString("HH:mm:ss"));
					listViewItem.Tag = sipCall;
					this.m_pCalls.Items.Add(listViewItem);
				}
			}
		}
	}
}

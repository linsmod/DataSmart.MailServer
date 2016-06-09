using DataSmart.MailServer.Management;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SystemForms
{
	public class FetchPop3ServiceForm : Form
	{
		private TabControl m_pTab;

		private Button m_pApply;

		private CheckBox m_pEnabled;

		private Label mt_FetchInterval;

		private NumericUpDown m_pFetchInterval;

		private Label mt_Seconds;

		private VirtualServer m_pVirtualServer;

		public FetchPop3ServiceForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.LoadData();
		}

		private void InitializeComponent()
		{
			this.m_pTab = new TabControl();
			this.m_pTab.Size = new Size(515, 520);
			this.m_pTab.Location = new Point(5, 0);
			this.m_pTab.TabPages.Add(new TabPage("General"));
			this.m_pApply = new Button();
			this.m_pApply.Size = new Size(70, 20);
			this.m_pApply.Location = new Point(450, 530);
			this.m_pApply.Text = "Apply";
			this.m_pApply.Click += new EventHandler(this.m_pApply_Click);
			this.m_pEnabled = new CheckBox();
			this.m_pEnabled.Size = new Size(70, 20);
			this.m_pEnabled.Location = new Point(105, 30);
			this.m_pEnabled.Text = "Enabled";
			this.mt_FetchInterval = new Label();
			this.mt_FetchInterval.Size = new Size(80, 13);
			this.mt_FetchInterval.Location = new Point(20, 63);
			this.mt_FetchInterval.Text = "Fetch Interval:";
			this.m_pFetchInterval = new NumericUpDown();
			this.m_pFetchInterval.Size = new Size(70, 20);
			this.m_pFetchInterval.Location = new Point(105, 60);
			this.m_pFetchInterval.Minimum = 30m;
			this.m_pFetchInterval.Maximum = 9999m;
			this.mt_Seconds = new Label();
			this.mt_Seconds.Size = new Size(30, 13);
			this.mt_Seconds.Location = new Point(180, 63);
			this.mt_Seconds.Text = "sec.";
			this.m_pTab.TabPages[0].Controls.Add(this.m_pEnabled);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_FetchInterval);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pFetchInterval);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Seconds);
			base.Controls.Add(this.m_pTab);
			base.Controls.Add(this.m_pApply);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (!base.Visible)
			{
				this.SaveData(true);
			}
		}

		private void m_pApply_Click(object sender, EventArgs e)
		{
			this.SaveData(false);
		}

		private void LoadData()
		{
			try
			{
				FetchMessages_Settings fetchMessages = this.m_pVirtualServer.SystemSettings.FetchMessages;
				this.m_pEnabled.Checked = fetchMessages.Enabled;
				this.m_pFetchInterval.Value = fetchMessages.FetchInterval;
			}
			catch (Exception x)
			{
				ErrorForm errorForm = new ErrorForm(x, new StackTrace());
				errorForm.ShowDialog(this);
			}
		}

		private void SaveData(bool confirmSave)
		{
			try
			{
				FetchMessages_Settings fetchMessages = this.m_pVirtualServer.SystemSettings.FetchMessages;
				fetchMessages.Enabled = this.m_pEnabled.Checked;
				fetchMessages.FetchInterval = (int)this.m_pFetchInterval.Value;
				if (this.m_pVirtualServer.SystemSettings.HasChanges && (!confirmSave || MessageBox.Show(this, "You have changes settings, do you want to save them ?", "Confirm:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
				{
					this.m_pVirtualServer.SystemSettings.Commit();
				}
			}
			catch (Exception x)
			{
				ErrorForm errorForm = new ErrorForm(x, new StackTrace());
				errorForm.ShowDialog(this);
			}
		}
	}
}

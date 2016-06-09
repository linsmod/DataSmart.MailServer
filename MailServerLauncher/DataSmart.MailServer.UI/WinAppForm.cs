using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class WinAppForm : Form
	{
		private Button m_pStart;

		private Button m_pStop;

		private Server m_pServer;

		public WinAppForm()
		{
			this.InitializeComponent();
			this.m_pServer = new Server();
		}

		private void InitializeComponent()
		{
			base.Size = new Size(200, 100);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.Text = "DataSmart Mail Server";
			base.FormClosed += new FormClosedEventHandler(this.wfrm_WinForm_FormClosed);
			this.m_pStart = new Button();
			this.m_pStart.Size = new Size(70, 20);
			this.m_pStart.Location = new Point(110, 20);
			this.m_pStart.Text = "Start";
			this.m_pStart.Click += new EventHandler(this.m_pStart_Click);
			this.m_pStop = new Button();
			this.m_pStop.Size = new Size(70, 20);
			this.m_pStop.Location = new Point(110, 50);
			this.m_pStop.Text = "Stop";
			this.m_pStop.Enabled = false;
			this.m_pStop.Click += new EventHandler(this.m_pStop_Click);
			base.Controls.Add(this.m_pStart);
			base.Controls.Add(this.m_pStop);
			base.Height = 200;
		}

		private void wfrm_WinForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.m_pServer.Stop();
		}

		private void m_pStart_Click(object sender, EventArgs e)
		{
			try
			{
				this.m_pServer.Start();
				this.m_pStart.Enabled = false;
				this.m_pStop.Enabled = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void m_pStop_Click(object sender, EventArgs e)
		{
			try
			{
				this.m_pServer.Stop();
				this.m_pStart.Enabled = true;
				this.m_pStop.Enabled = false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer
{
	public class BuildInitStringFormBase : Form
	{
		private Container components;

		public virtual string InitString
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public BuildInitStringFormBase()
		{
			this.InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			base.Size = new Size(300, 300);
			this.Text = "wfrm_BuildInitString_base";
		}
	}
}

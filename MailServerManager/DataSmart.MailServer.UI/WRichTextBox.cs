using System;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class WRichTextBox : RichTextBox
	{
		private bool m_SuspendPaint;

		public bool SuspendPaint
		{
			get
			{
				return this.m_SuspendPaint;
			}
			set
			{
				this.m_SuspendPaint = value;
				if (!value)
				{
					this.Refresh();
				}
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg != 15)
			{
				base.WndProc(ref m);
				return;
			}
			if (this.m_SuspendPaint)
			{
				m.Result = IntPtr.Zero;
				return;
			}
			base.WndProc(ref m);
		}
	}
}

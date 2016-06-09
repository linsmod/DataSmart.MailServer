using System;

namespace DataSmart.MailServer.UI
{
	public class WComboBoxItem
	{
		private string m_Text = "";

		private object m_Tag;

		public string Text
		{
			get
			{
				return this.m_Text;
			}
		}

		public object Tag
		{
			get
			{
				return this.m_Tag;
			}
			set
			{
				this.m_Tag = value;
			}
		}

		public WComboBoxItem(string text)
		{
			this.m_Text = text;
		}

		public WComboBoxItem(string text, object tag)
		{
			this.m_Text = text;
			this.m_Tag = tag;
		}

		public override string ToString()
		{
			return this.m_Text;
		}
	}
}

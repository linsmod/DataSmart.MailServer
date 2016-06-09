using System;

namespace DataSmart.MailServer.Management
{
	public class RouteAction_RouteToMailbox : RouteActionBase
	{
		private string m_Mailbox = "";

		public string Mailbox
		{
			get
			{
				return this.m_Mailbox;
			}
		}

		public RouteAction_RouteToMailbox(string mailbox) : base(RouteAction.RouteToMailbox)
		{
			this.m_Mailbox = mailbox;
		}

		internal RouteAction_RouteToMailbox(byte[] actionData) : base(RouteAction.RouteToMailbox)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_Mailbox = xmlTable.GetValue("Mailbox");
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("Mailbox", this.m_Mailbox);
			return xmlTable.ToByteData();
		}
	}
}

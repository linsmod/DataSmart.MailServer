using System;

namespace DataSmart.MailServer.Management
{
	public class RouteAction_RouteToEmail : RouteActionBase
	{
		private string m_Email = "";

		public string EmailAddress
		{
			get
			{
				return this.m_Email;
			}
		}

		public RouteAction_RouteToEmail(string emailAddress) : base(RouteAction.RouteToEmail)
		{
			this.m_Email = emailAddress;
		}

		internal RouteAction_RouteToEmail(byte[] actionData) : base(RouteAction.RouteToEmail)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_Email = xmlTable.GetValue("EmailAddress");
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("EmailAddress", this.m_Email);
			return xmlTable.ToByteData();
		}
	}
}

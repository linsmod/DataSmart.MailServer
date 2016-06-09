using System;

namespace DataSmart.MailServer.Management
{
	public class RouteAction_RouteToHost : RouteActionBase
	{
		private string m_Host = "";

		private int m_Port = 25;

		public string Host
		{
			get
			{
				return this.m_Host;
			}
		}

		public int Port
		{
			get
			{
				return this.m_Port;
			}
		}

		public RouteAction_RouteToHost(string host, int port) : base(RouteAction.RouteToHost)
		{
			this.m_Host = host;
			this.m_Port = port;
		}

		internal RouteAction_RouteToHost(byte[] actionData) : base(RouteAction.RouteToHost)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_Host = xmlTable.GetValue("Host");
			this.m_Port = Convert.ToInt32(xmlTable.GetValue("Port"));
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("Host", this.m_Host);
			xmlTable.Add("Port", this.m_Port.ToString());
			return xmlTable.ToByteData();
		}
	}
}

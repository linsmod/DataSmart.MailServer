using System;

namespace DataSmart.MailServer.Management
{
	public class RouteActionBase
	{
		private RouteAction m_ActionType = RouteAction.RouteToMailbox;

		public RouteAction ActionType
		{
			get
			{
				return this.m_ActionType;
			}
		}

		internal RouteActionBase(RouteAction actionType)
		{
			this.m_ActionType = actionType;
		}

		internal virtual byte[] Serialize()
		{
			return new byte[0];
		}
	}
}

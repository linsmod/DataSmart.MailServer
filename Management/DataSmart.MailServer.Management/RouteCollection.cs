using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class RouteCollection : IEnumerable
	{
		private VirtualServer m_pVirtualServer;

		private List<Route> m_pRoutes;

		public VirtualServer VirtualServer
		{
			get
			{
				return this.m_pVirtualServer;
			}
		}

		public int Count
		{
			get
			{
				return this.m_pRoutes.Count;
			}
		}

		public Route this[int index]
		{
			get
			{
				return this.m_pRoutes[index];
			}
		}

		public Route this[string routeID]
		{
			get
			{
				foreach (Route current in this.m_pRoutes)
				{
					if (current.ID.ToLower() == routeID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Route with specified ID '" + routeID + "' doesn't exist !");
			}
		}

		internal RouteCollection(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pRoutes = new List<Route>();
			this.Bind();
		}

		public Route Add(string description, string pattern, bool enabled, RouteActionBase action)
		{
			string text = Guid.NewGuid().ToString();
			long ticks = DateTime.Now.Ticks;
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"AddRoute ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(text),
				" ",
				ticks,
				" ",
				TextUtils.QuoteString(description),
				" ",
				TextUtils.QuoteString(pattern),
				" ",
				enabled,
				" ",
				(int)action.ActionType,
				" ",
				Convert.ToBase64String(action.Serialize())
			}));
			string text2 = this.m_pVirtualServer.Server.ReadLine();
			if (!text2.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text2);
			}
			Route route = new Route(this, text, ticks, description, pattern, enabled, action);
			this.m_pRoutes.Add(route);
			return route;
		}

		public void Remove(Route route)
		{
			Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("DeleteRoute " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(route.ID));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pRoutes.Remove(route);
		}

		public bool Contains(string routeID)
		{
			foreach (Route current in this.m_pRoutes)
			{
				if (current.ID.ToLower() == routeID.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		public bool ContainsPattern(string pattern)
		{
			foreach (Route current in this.m_pRoutes)
			{
				if (current.Pattern.ToLower() == pattern.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		public Route GetRouteByPattern(string pattern)
		{
			foreach (Route current in this.m_pRoutes)
			{
				if (current.Pattern.ToLower() == pattern.ToLower())
				{
					return current;
				}
			}
			throw new Exception("Route with specified pattern '" + pattern + "' doesn't exist !");
		}

		public void Refresh()
		{
			this.m_pRoutes.Clear();
			this.Bind();
		}

		private void Bind()
		{
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetRoutes " + this.m_pVirtualServer.VirtualServerID);
				string text = this.m_pVirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("Routing"))
				{
					foreach (DataRow dataRow in dataSet.Tables["Routing"].Rows)
					{
						RouteAction routeAction_enum = (RouteAction)Convert.ToInt32(dataRow["Action"]);
						RouteActionBase action = null;
						if (routeAction_enum == RouteAction.RouteToEmail)
						{
							action = new RouteAction_RouteToEmail(Convert.FromBase64String(dataRow["ActionData"].ToString()));
						}
						else if (routeAction_enum == RouteAction.RouteToHost)
						{
							action = new RouteAction_RouteToHost(Convert.FromBase64String(dataRow["ActionData"].ToString()));
						}
						else if (routeAction_enum == RouteAction.RouteToMailbox)
						{
							action = new RouteAction_RouteToMailbox(Convert.FromBase64String(dataRow["ActionData"].ToString()));
						}
						this.m_pRoutes.Add(new Route(this, dataRow["RouteID"].ToString(), Convert.ToInt64(dataRow["Cost"]), dataRow["Description"].ToString(), dataRow["Pattern"].ToString(), Convert.ToBoolean(dataRow["Enabled"].ToString()), action));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pRoutes.GetEnumerator();
		}
	}
}

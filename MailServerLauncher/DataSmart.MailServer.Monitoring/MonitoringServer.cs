using System.NetworkToolkit;
using System.NetworkToolkit.TCP;
using System;
using System.Net;
using System.Net.Sockets;

namespace DataSmart.MailServer.Monitoring
{
	public class MonitoringServer : TCP_Server<MonitoringServerSession>
	{
		private int m_MaxBadCommands = 30;

		private Server m_pServer;

		public int MaxBadCommands
		{
			get
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MaxBadCommands;
			}
			set
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				if (value < 0)
				{
					throw new ArgumentException("Property 'MaxBadCommands' value must be >= 0.");
				}
				this.m_MaxBadCommands = value;
			}
		}

		public Server MailServer
		{
			get
			{
				return this.m_pServer;
			}
		}

		public MonitoringServer(Server server)
		{
			this.m_pServer = server;
			if (Socket.OSSupportsIPv6)
			{
				base.Bindings = new IPBindInfo[]
				{
					new IPBindInfo(Dns.GetHostName(), BindInfoProtocol.TCP, IPAddress.Any, 5252),
					new IPBindInfo(Dns.GetHostName(), BindInfoProtocol.TCP, IPAddress.IPv6Any, 5252)
				};
			}
			else
			{
				base.Bindings = new IPBindInfo[]
				{
					new IPBindInfo(Dns.GetHostName(), BindInfoProtocol.TCP, IPAddress.Any, 5252)
				};
			}
			base.SessionIdleTimeout = 1800;
		}
	}
}

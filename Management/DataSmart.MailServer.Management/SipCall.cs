using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class SipCall
	{
		private SipCallCollection m_pOwner;

		private string m_CallID = "";

		private string m_Caller = "";

		private string m_Callee = "";

		private DateTime m_StartTime;

		public string CallID
		{
			get
			{
				return this.m_CallID;
			}
		}

		public string Caller
		{
			get
			{
				return this.m_Caller;
			}
		}

		public string Callee
		{
			get
			{
				return this.m_Callee;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.m_StartTime;
			}
		}

		internal SipCall(SipCallCollection owner, string callID, string caller, string callee, DateTime startTime)
		{
			this.m_pOwner = owner;
			this.m_CallID = callID;
			this.m_Caller = caller;
			this.m_Callee = callee;
			this.m_StartTime = startTime;
		}

		public void Terminate()
		{
			lock (this.m_pOwner.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pOwner.VirtualServer.Server.TCP_Client.TcpStream.WriteLine("TerminateSipCall " + TextUtils.QuoteString(this.m_pOwner.VirtualServer.VirtualServerID) + " " + TextUtils.QuoteString(this.CallID));
				string text = this.m_pOwner.VirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
			}
		}
	}
}

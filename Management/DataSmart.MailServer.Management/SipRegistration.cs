using System.NetworkToolkit;
using System.NetworkToolkit.SIP.Message;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class SipRegistration
	{
		private SipRegistrationCollection m_pOwner;

		private string m_UserName = "";

		private string m_AddressOfRecord = "";

		private SipRegistrationContact[] m_pContacts;

		public SipRegistrationCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		public string AddressOfRecord
		{
			get
			{
				return this.m_AddressOfRecord;
			}
		}

		public SipRegistrationContact[] Contacts
		{
			get
			{
				return this.m_pContacts;
			}
		}

		internal SipRegistration(SipRegistrationCollection owner, string userName, string addressOfRecord, SipRegistrationContact[] contacts)
		{
			this.m_pOwner = owner;
			this.m_UserName = userName;
			this.m_AddressOfRecord = addressOfRecord;
			this.m_pContacts = contacts;
		}

		public void Refresh()
		{
			lock (this.m_pOwner.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pOwner.VirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetSipRegistration " + TextUtils.QuoteString(this.m_pOwner.VirtualServer.VirtualServerID) + " " + TextUtils.QuoteString(this.m_AddressOfRecord));
				string text = this.m_pOwner.VirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pOwner.VirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("Contacts"))
				{
					List<SipRegistrationContact> list = new List<SipRegistrationContact>();
					foreach (DataRow dataRow in dataSet.Tables["Contacts"].Rows)
					{
						SIP_t_ContactParam sIP_t_ContactParam = new SIP_t_ContactParam();
						sIP_t_ContactParam.Parse(new System.NetworkToolkit.StringReader(dataRow["Value"].ToString()));
						list.Add(new SipRegistrationContact(sIP_t_ContactParam.Address.Uri.Value, sIP_t_ContactParam.Expires, sIP_t_ContactParam.QValue));
					}
					this.m_pContacts = list.ToArray();
				}
				else
				{
					this.m_pContacts = new SipRegistrationContact[0];
				}
			}
		}
	}
}

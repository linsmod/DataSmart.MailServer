using System.NetworkToolkit;
using System.NetworkToolkit.SIP.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class SipRegistrationCollection
	{
		private VirtualServer m_pOwner;

		private List<SipRegistration> m_pRegistrations;

		public VirtualServer VirtualServer
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public int Count
		{
			get
			{
				return this.m_pRegistrations.Count;
			}
		}

		public SipRegistration this[int index]
		{
			get
			{
				return this.m_pRegistrations[index];
			}
		}

		public SipRegistration this[string addressOfRecord]
		{
			get
			{
				foreach (SipRegistration current in this.m_pRegistrations)
				{
					if (current.AddressOfRecord.ToLower() == addressOfRecord.ToLower())
					{
						return current;
					}
				}
				throw new Exception("SipRegistration with registration '" + addressOfRecord + "' doesn't exist !");
			}
		}

		internal List<SipRegistration> List
		{
			get
			{
				return this.m_pRegistrations;
			}
		}

		internal SipRegistrationCollection(VirtualServer owner)
		{
			this.m_pOwner = owner;
			this.m_pRegistrations = new List<SipRegistration>();
			this.Bind();
		}

		public void Refresh()
		{
			lock (this.m_pOwner.Server.LockSynchronizer)
			{
				this.m_pRegistrations.Clear();
				this.Bind();
			}
		}

		public void Set(string addressOfRecord, string[] contacts)
		{
			lock (this.m_pOwner.Server.LockSynchronizer)
			{
				string text = "";
				for (int i = 0; i < contacts.Length; i++)
				{
					if (i == contacts.Length - 1)
					{
						text += contacts[i];
					}
					else
					{
						text = text + contacts[i] + "\t";
					}
				}
				this.m_pOwner.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
				{
					"SetSipRegistration ",
					TextUtils.QuoteString(this.m_pOwner.VirtualServerID),
					" ",
					TextUtils.QuoteString(addressOfRecord),
					" ",
					TextUtils.QuoteString(text)
				}));
				string text2 = this.m_pOwner.Server.ReadLine();
				if (!text2.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text2);
				}
				SipRegistration sipRegistration = new SipRegistration(this, "administrator", addressOfRecord, new SipRegistrationContact[0]);
				this.m_pRegistrations.Add(sipRegistration);
				sipRegistration.Refresh();
			}
		}

		public void Remove(SipRegistration registration)
		{
			lock (this.m_pOwner.Server.LockSynchronizer)
			{
				Guid.NewGuid().ToString();
				this.m_pOwner.Server.TCP_Client.TcpStream.WriteLine("DeleteSipRegistration " + TextUtils.QuoteString(this.m_pOwner.VirtualServerID) + " " + TextUtils.QuoteString(registration.AddressOfRecord));
				string text = this.m_pOwner.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				this.m_pRegistrations.Remove(registration);
			}
		}

		private void Bind()
		{
			lock (this.m_pOwner.Server.LockSynchronizer)
			{
				this.m_pOwner.Server.TCP_Client.TcpStream.WriteLine("GetSipRegistrations " + TextUtils.QuoteString(this.m_pOwner.VirtualServerID));
				string text = this.m_pOwner.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pOwner.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("SipRegistrations"))
				{
					foreach (DataRow dataRow in dataSet.Tables["SipRegistrations"].Rows)
					{
						List<SipRegistrationContact> list = new List<SipRegistrationContact>();
						string[] array = dataRow["Contacts"].ToString().Split(new char[]
						{
							'\t'
						});
						for (int i = 0; i < array.Length; i++)
						{
							string text2 = array[i];
							if (!string.IsNullOrEmpty(text2))
							{
								SIP_t_ContactParam sIP_t_ContactParam = new SIP_t_ContactParam();
								sIP_t_ContactParam.Parse(new System.NetworkToolkit.StringReader(text2));
								list.Add(new SipRegistrationContact(sIP_t_ContactParam.Address.Uri.Value, sIP_t_ContactParam.Expires, sIP_t_ContactParam.QValue));
							}
						}
						this.m_pRegistrations.Add(new SipRegistration(this, dataRow["UserName"].ToString(), dataRow["AddressOfRecord"].ToString(), list.ToArray()));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pRegistrations.GetEnumerator();
		}
	}
}

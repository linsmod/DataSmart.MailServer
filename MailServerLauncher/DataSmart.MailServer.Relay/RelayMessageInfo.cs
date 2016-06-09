using System.NetworkToolkit;
using System.NetworkToolkit.SMTP;
using System;
using System.Globalization;

namespace DataSmart.MailServer.Relay
{
	public class RelayMessageInfo
	{
		private string m_EnvelopeID;

		private string m_Sender = "";

		private string m_Recipient = "";

		private string m_OriginalRecipient;

		private SMTP_DSN_Notify m_DSN_Notify;

		private SMTP_DSN_Ret m_DSN_Ret;

		private DateTime m_Date;

		private bool m_DelayedDeliveryNotifySent;

		private HostEndPoint m_pHostEndPoint;

		public string EnvelopeID
		{
			get
			{
				return this.m_EnvelopeID;
			}
		}

		public string Sender
		{
			get
			{
				return this.m_Sender;
			}
		}

		public string Recipient
		{
			get
			{
				return this.m_Recipient;
			}
		}

		public string OriginalRecipient
		{
			get
			{
				return this.m_OriginalRecipient;
			}
		}

		public DateTime Date
		{
			get
			{
				return this.m_Date;
			}
		}

		public SMTP_DSN_Notify DSN_Notify
		{
			get
			{
				return this.m_DSN_Notify;
			}
		}

		public SMTP_DSN_Ret DSN_Ret
		{
			get
			{
				return this.m_DSN_Ret;
			}
		}

		public bool DelayedDeliveryNotifySent
		{
			get
			{
				return this.m_DelayedDeliveryNotifySent;
			}
			set
			{
				this.m_DelayedDeliveryNotifySent = value;
			}
		}

		public HostEndPoint HostEndPoint
		{
			get
			{
				return this.m_pHostEndPoint;
			}
		}

		public RelayMessageInfo(string envelopeID, string sender, string recipient, string originalRecipient, SMTP_DSN_Notify notify, SMTP_DSN_Ret ret, DateTime date, bool delayedDeliveryNotifySent, HostEndPoint hostEndPoint)
		{
			if (sender == null)
			{
				throw new ArgumentNullException("sender");
			}
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (recipient == "")
			{
				throw new ArgumentException("Argument 'recipient' value must be specified.");
			}
			this.m_EnvelopeID = envelopeID;
			this.m_Sender = sender;
			this.m_Recipient = recipient;
			this.m_OriginalRecipient = originalRecipient;
			this.m_DSN_Notify = notify;
			this.m_DSN_Ret = ret;
			this.m_Date = date;
			this.m_DelayedDeliveryNotifySent = delayedDeliveryNotifySent;
			this.m_pHostEndPoint = hostEndPoint;
		}

		public static RelayMessageInfo Parse(byte[] value)
		{
			RelayMessageInfo result;
			try
			{
				XmlTable xmlTable = new XmlTable("RelayMessageInfo");
				xmlTable.Parse(value);
				result = new RelayMessageInfo(string.IsNullOrEmpty(xmlTable.GetValue("EnvelopeID")) ? null : xmlTable.GetValue("EnvelopeID"), xmlTable.GetValue("Sender"), xmlTable.GetValue("Recipient"), string.IsNullOrEmpty(xmlTable.GetValue("OriginalRecipient")) ? null : xmlTable.GetValue("OriginalRecipient"), string.IsNullOrEmpty(xmlTable.GetValue("DSN-Notify")) ? SMTP_DSN_Notify.NotSpecified : ((SMTP_DSN_Notify)Enum.Parse(typeof(SMTP_DSN_Notify), xmlTable.GetValue("DSN-Notify"))), string.IsNullOrEmpty(xmlTable.GetValue("DSN-RET")) ? SMTP_DSN_Ret.NotSpecified : ((SMTP_DSN_Ret)Enum.Parse(typeof(SMTP_DSN_Ret), xmlTable.GetValue("DSN-RET"))), DateTime.ParseExact(xmlTable.GetValue("Date"), "r", DateTimeFormatInfo.InvariantInfo), Convert.ToBoolean(xmlTable.GetValue("DelayedDeliveryNotifySent")), (!string.IsNullOrEmpty(xmlTable.GetValue("HostEndPoint"))) ? HostEndPoint.Parse(xmlTable.GetValue("HostEndPoint"), 25) : null);
			}
			catch
			{
				throw new ArgumentException("Argument 'value' has invalid RelayMessageInfo value.");
			}
			return result;
		}

		public byte[] ToByte()
		{
			XmlTable xmlTable = new XmlTable("RelayMessageInfo");
			if (!string.IsNullOrEmpty(this.m_EnvelopeID))
			{
				xmlTable.Add("EnvelopeID", this.m_EnvelopeID);
			}
			xmlTable.Add("Sender", this.m_Sender);
			xmlTable.Add("Recipient", this.m_Recipient);
			if (!string.IsNullOrEmpty(this.m_OriginalRecipient))
			{
				xmlTable.Add("OriginalRecipient", this.m_OriginalRecipient);
			}
			if (this.m_DSN_Notify != SMTP_DSN_Notify.NotSpecified)
			{
				xmlTable.Add("DSN-Notify", this.m_DSN_Notify.ToString());
			}
			if (this.m_DSN_Ret != SMTP_DSN_Ret.NotSpecified)
			{
				xmlTable.Add("DSN-RET", this.m_DSN_Ret.ToString());
			}
			xmlTable.Add("Date", this.m_Date.ToString("r"));
			xmlTable.Add("DelayedDeliveryNotifySent", this.m_DelayedDeliveryNotifySent.ToString());
			if (this.m_pHostEndPoint != null)
			{
				xmlTable.Add("HostEndPoint", this.m_pHostEndPoint.ToString());
			}
			else
			{
				xmlTable.Add("HostEndPoint", "");
			}
			return xmlTable.ToByteData();
		}
	}
}

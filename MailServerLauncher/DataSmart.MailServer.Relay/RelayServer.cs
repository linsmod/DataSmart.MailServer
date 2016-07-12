using System.NetworkToolkit;
using System.NetworkToolkit.Mail;
using System.NetworkToolkit.MIME;
using System.NetworkToolkit.SMTP;
using System.NetworkToolkit.SMTP.Client;
using System.NetworkToolkit.SMTP.Relay;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace DataSmart.MailServer.Relay
{
	public class RelayServer : Relay_Server
	{
		private VirtualServer m_pVirtualServer;

		private int m_RelayInterval = 30;

		private int m_RelayRetryInterval = 180;

		private int m_DelayedDeliveryNotifyAfter = 5;

		private int m_UndeliveredAfter = 60;

		private ServerReturnMessage m_DelayedDeliveryMessage;

		private ServerReturnMessage m_UndeliveredMessage;

		private bool m_StoreUndelivered;

		public int RelayInterval
		{
			get
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RelayInterval;
			}
			set
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				if (this.m_RelayInterval != value)
				{
					this.m_RelayInterval = value;
				}
			}
		}

		public int RelayRetryInterval
		{
			get
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RelayRetryInterval;
			}
			set
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				if (this.m_RelayRetryInterval != value)
				{
					this.m_RelayRetryInterval = value;
				}
			}
		}

		public int DelayedDeliveryNotifyAfter
		{
			get
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_DelayedDeliveryNotifyAfter;
			}
			set
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				if (this.m_DelayedDeliveryNotifyAfter != value)
				{
					this.m_DelayedDeliveryNotifyAfter = value;
				}
			}
		}

		public int UndeliveredAfter
		{
			get
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_UndeliveredAfter;
			}
			set
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				if (this.m_UndeliveredAfter != value)
				{
					this.m_UndeliveredAfter = value;
				}
			}
		}

		public ServerReturnMessage DelayedDeliveryMessage
		{
			get
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_DelayedDeliveryMessage;
			}
			set
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				if (this.m_DelayedDeliveryMessage != value)
				{
					this.m_DelayedDeliveryMessage = value;
				}
			}
		}

		public ServerReturnMessage UndeliveredMessage
		{
			get
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_UndeliveredMessage;
			}
			set
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				if (this.m_UndeliveredMessage != value)
				{
					this.m_UndeliveredMessage = value;
				}
			}
		}

		public bool StoreUndeliveredMessages
		{
			get
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_StoreUndelivered;
			}
			set
			{
				if (base.IsDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				if (this.m_StoreUndelivered != value)
				{
					this.m_StoreUndelivered = value;
				}
			}
		}

		public RelayServer(VirtualServer virtualServer)
		{
			if (virtualServer == null)
			{
				throw new ArgumentNullException("virtualServer");
			}
			this.m_pVirtualServer = virtualServer;
			base.Queues.Add(new Relay_Queue("Relay"));
			base.Queues.Add(new Relay_Queue("Retry"));
		}

		public override void Start()
		{
			base.Start();
			Thread thread = new Thread(new ThreadStart(this.ProcessRelay));
            thread.Name = "RelayServer Relay Thread";
			thread.Start();

			Thread thread2 = new Thread(new ThreadStart(this.ProcessRelayRetry));
            thread2.Name = "RelayServer Relay Retry Thread";
			thread2.Start();
		}

		public void StoreRelayMessage(string id, string envelopeID, Stream message, HostEndPoint targetHost, string sender, string to, string originalRecipient, SMTP_DSN_Notify notify, SMTP_DSN_Ret ret)
		{
			this.StoreRelayMessage("Relay", id, envelopeID, DateTime.Now, message, targetHost, sender, to, originalRecipient, notify, ret);
		}

		private void StoreRelayMessage(string queueName, string id, string envelopeID, DateTime date, Stream message, HostEndPoint targetHost, string sender, string to, string originalRecipient, SMTP_DSN_Notify notify, SMTP_DSN_Ret ret)
		{
			if (queueName == null)
			{
				throw new ArgumentNullException("queueName");
			}
			if (queueName == "")
			{
				throw new ArgumentException("Argumnet 'queueName' value must be specified.");
			}
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id == "")
			{
				throw new ArgumentException("Argument 'id' value must be specified.");
			}
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (to == null)
			{
				throw new ArgumentNullException("to");
			}
			if (to == "")
			{
				throw new ArgumentException("Argument 'to' value must be specified.");
			}
			string text = this.m_pVirtualServer.MailStorePath + queueName;
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			using (FileStream fileStream = File.Create(PathHelper.PathFix(text + "\\" + id + ".eml")))
			{
				SCore.StreamCopy(message, fileStream);
				RelayMessageInfo relayMessageInfo = new RelayMessageInfo(envelopeID, sender, to, originalRecipient, notify, ret, date, false, targetHost);
				File.WriteAllBytes(PathHelper.PathFix(text + "\\" + id + ".info"), relayMessageInfo.ToByte());
			}
		}

		private void ProcessRelay()
		{
			DateTime dateTime = DateTime.MinValue;
			while (base.IsRunning)
			{
				try
				{
					if (dateTime.AddSeconds((double)this.m_RelayInterval) < DateTime.Now)
					{
						string text = this.m_pVirtualServer.MailStorePath + "Relay";
						if (Directory.Exists(text))
						{
							string[] files = Directory.GetFiles(text, "*.eml");
							string[] array = files;
							for (int i = 0; i < array.Length; i++)
							{
								string text2 = array[i];
								while (base.Queues[0].Count > 25)
								{
									if (!base.IsRunning)
									{
										return;
									}
									Thread.Sleep(100);
								}
								try
								{
									string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text2);
									FileStream message = File.Open(text2, FileMode.Open, FileAccess.ReadWrite, FileShare.Read | FileShare.Delete);
									if (File.Exists(PathHelper.PathFix(text + "\\" + fileNameWithoutExtension + ".info")))
									{
										RelayMessageInfo relayMessageInfo = RelayMessageInfo.Parse(File.ReadAllBytes(PathHelper.PathFix(text + "\\" + fileNameWithoutExtension + ".info")));
										base.Queues[0].QueueMessage((relayMessageInfo.HostEndPoint == null) ? null : new Relay_SmartHost(relayMessageInfo.HostEndPoint.Host, relayMessageInfo.HostEndPoint.Port), relayMessageInfo.Sender, relayMessageInfo.EnvelopeID, relayMessageInfo.DSN_Ret, relayMessageInfo.Recipient, relayMessageInfo.OriginalRecipient, relayMessageInfo.DSN_Notify, fileNameWithoutExtension, message, relayMessageInfo);
									}
									else
									{
										DataSmart.MailServer.Error.DumpError(this.m_pVirtualServer.Name, new Exception("Relay message '" + text2 + "' .info file is missing, deleting message."));
										File.Delete(text2);
									}
								}
								catch (IOException ex)
								{
									string arg_18A_0 = ex.Message;
								}
							}
						}
						dateTime = DateTime.Now;
						Thread.Sleep(1000);
					}
					else
					{
						Thread.Sleep(1000);
					}
				}
				catch (Exception x)
				{
					DataSmart.MailServer.Error.DumpError(this.m_pVirtualServer.Name, x);
				}
			}
		}

		private void ProcessRelayRetry()
		{
			DateTime dateTime = DateTime.MinValue;
			while (base.IsRunning)
			{
				try
				{
					if (dateTime.AddSeconds((double)this.m_RelayRetryInterval) < DateTime.Now)
					{
						string text = this.m_pVirtualServer.MailStorePath + "Retry";
						if (Directory.Exists(text))
						{
							string[] files = Directory.GetFiles(text, "*.eml");
							string[] array = files;
							for (int i = 0; i < array.Length; i++)
							{
								string text2 = array[i];
								while (base.Queues[1].Count > 25)
								{
									if (!base.IsRunning)
									{
										return;
									}
									Thread.Sleep(100);
								}
								try
								{
									string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text2);
									FileStream message = File.Open(text2, FileMode.Open, FileAccess.ReadWrite, FileShare.Read | FileShare.Delete);
									if (File.Exists(PathHelper.PathFix(text + "\\" + fileNameWithoutExtension + ".info")))
									{
										RelayMessageInfo relayMessageInfo = RelayMessageInfo.Parse(File.ReadAllBytes(PathHelper.PathFix(text + "\\" + fileNameWithoutExtension + ".info")));
										base.Queues[1].QueueMessage(relayMessageInfo.Sender, relayMessageInfo.Recipient, fileNameWithoutExtension, message, relayMessageInfo);
									}
									else
									{
										DataSmart.MailServer.Error.DumpError(this.m_pVirtualServer.Name, new Exception("Relay message '" + text2 + "' .info file is missing, deleting message."));
										File.Delete(text2);
									}
								}
								catch (IOException ex)
								{
									string arg_142_0 = ex.Message;
								}
							}
						}
						dateTime = DateTime.Now;
						Thread.Sleep(1000);
					}
					else
					{
						Thread.Sleep(1000);
					}
				}
				catch (Exception x)
				{
					DataSmart.MailServer.Error.DumpError(this.m_pVirtualServer.Name, x);
				}
			}
		}

		protected override void OnSessionCompleted(Relay_Session session, Exception exception)
		{
			base.OnSessionCompleted(session, exception);
			try
			{
				FileStream fileStream = (FileStream)session.MessageStream;
				RelayMessageInfo relayMessageInfo = (RelayMessageInfo)session.QueueTag;
				bool flag = false;
				if (exception == null)
				{
					flag = true;
					this.Send_DSN_Relayed(session);
				}
				else
				{
					bool flag2 = false;
					if (exception is SMTP_ClientException)
					{
						flag2 = ((SMTP_ClientException)exception).IsPermanentError;
					}
					if (flag2 || relayMessageInfo.Date.AddMinutes((double)this.m_UndeliveredAfter) < DateTime.Now)
					{
						this.Send_DSN_Failed(session, exception.Message);
						flag = true;
					}
					else if (session.Queue.Name.ToLower() == "relay")
					{
						session.MessageStream.Position = 0L;
						this.StoreRelayMessage("Retry", Path.GetFileNameWithoutExtension(fileStream.Name), relayMessageInfo.EnvelopeID, relayMessageInfo.Date, session.MessageStream, relayMessageInfo.HostEndPoint, relayMessageInfo.Sender, relayMessageInfo.Recipient, relayMessageInfo.OriginalRecipient, relayMessageInfo.DSN_Notify, relayMessageInfo.DSN_Ret);
						flag = true;
					}
					else if (!relayMessageInfo.DelayedDeliveryNotifySent && DateTime.Now > relayMessageInfo.Date.AddMinutes((double)this.DelayedDeliveryNotifyAfter))
					{
						this.Send_DSN_Delayed(session, exception.Message);
						relayMessageInfo.DelayedDeliveryNotifySent = true;
						File.WriteAllBytes(fileStream.Name.Replace(".eml", ".info"), relayMessageInfo.ToByte());
					}
				}
				if (flag)
				{
					File.Delete(fileStream.Name);
					File.Delete(fileStream.Name.Replace(".eml", ".info"));
				}
				fileStream.Dispose();
			}
			catch (Exception x)
			{
				DataSmart.MailServer.Error.DumpError(this.m_pVirtualServer.Name, x);
			}
		}

		protected override void OnError(Exception x)
		{
			base.OnError(x);
			DataSmart.MailServer.Error.DumpError(this.m_pVirtualServer.Name, x);
		}

		private void Send_DSN_Failed(Relay_Session session, string error)
		{
			try
			{
				if (!string.IsNullOrEmpty(session.From))
				{
					RelayMessageInfo relayMessageInfo = (RelayMessageInfo)session.QueueTag;
					if (relayMessageInfo.DSN_Notify == SMTP_DSN_Notify.NotSpecified || (relayMessageInfo.DSN_Notify & SMTP_DSN_Notify.Failure) != SMTP_DSN_Notify.NotSpecified)
					{
						session.MessageStream.Position = 0L;
						Mail_Message mail_Message = Mail_Message.ParseFromStream(session.MessageStream);
						RelayVariablesManager relayVariablesManager = new RelayVariablesManager(this, session, error, mail_Message);
						ServerReturnMessage serverReturnMessage = this.UndeliveredMessage;
						if (serverReturnMessage == null)
						{
							string bodyTextRft = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fswiss\\fprq2\\fcharset0 Verdana;}{\\f1\\fswiss\\fprq2\\fcharset186 Verdana;}{\\f2\\fnil\\fcharset0 Verdana;}{\\f3\\fnil\\fcharset186 Verdana;}{\\f4\\fswiss\\fcharset0 Arial;}{\\f5\\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}}\r\n{\\colortbl ;\\red0\\green64\\blue128;\\red255\\green0\\blue0;\\red0\\green128\\blue0;}\r\n\\viewkind4\\uc1\\pard\\f0\\fs20 Your message t\\lang1061\\f1 o \\cf1\\lang1033\\f2 <#relay.to>\\cf0\\f0 , dated \\b\\fs16 <#message.header[\"Date:\"]>\\b0\\fs20 , could not be delivered.\\par\r\n\\par\r\nRecipient \\i <#relay.to>'\\i0 s Server (\\cf1 <#relay.session_hostname>\\cf0 ) returned the following response: \\cf2 <#relay.error>\\cf0\\par\r\n\\par\r\n\\par\r\n\\b * Server will not attempt to deliver this message anymore\\b0 .\\par\r\n\\par\r\n--------\\par\r\n\\par\r\nYour original message is attached to this e-mail (\\b data.eml\\b0 )\\par\r\n\\par\r\n\\fs16 The tracking number for this message is \\cf3 <#relay.session_messageid>\\cf0\\fs20\\par\r\n\\lang1061\\f5\\par\r\n\\lang1033\\f2\\par\r\n}\r\n";
							serverReturnMessage = new ServerReturnMessage("Undelivered notice: <#message.header[\"Subject:\"]>", bodyTextRft);
						}
						string rtfText = relayVariablesManager.Process(serverReturnMessage.BodyTextRtf);
						Mail_Message mail_Message2 = DeliveryStatusNotification.CreateDsnMessage(session.From, relayVariablesManager.Process(serverReturnMessage.Subject), rtfText, relayMessageInfo.EnvelopeID, relayMessageInfo.Date, null, (session.IsConnected && string.IsNullOrEmpty(session.LocalHostName)) ? session.LocalEndPoint.Address.ToString() : session.LocalHostName, relayMessageInfo.OriginalRecipient, session.To, "failed", error, session.RemoteHostName, DateTime.Now, DateTime.MinValue, (relayMessageInfo.DSN_Ret == SMTP_DSN_Ret.NotSpecified) ? SMTP_DSN_Ret.FullMessage : relayMessageInfo.DSN_Ret, mail_Message);
						using (MemoryStream memoryStream = new MemoryStream())
						{
							mail_Message2.ToStream(memoryStream, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8);
							this.m_pVirtualServer.ProcessAndStoreMessage("", new string[]
							{
								session.From
							}, memoryStream, null);
						}
						mail_Message.Dispose();
						mail_Message2.Dispose();
					}
				}
			}
			catch (Exception x)
			{
				DataSmart.MailServer.Error.DumpError(this.m_pVirtualServer.Name, x);
			}
		}

		private void Send_DSN_Delayed(Relay_Session session, string error)
		{
			try
			{
				if (!string.IsNullOrEmpty(session.From))
				{
					RelayMessageInfo relayMessageInfo = (RelayMessageInfo)session.QueueTag;
					if (relayMessageInfo.DSN_Notify == SMTP_DSN_Notify.NotSpecified || (relayMessageInfo.DSN_Notify & SMTP_DSN_Notify.Delay) != SMTP_DSN_Notify.NotSpecified)
					{
						session.MessageStream.Position = 0L;
						Mail_Message mail_Message = Mail_Message.ParseFromStream(session.MessageStream);
						RelayVariablesManager relayVariablesManager = new RelayVariablesManager(this, session, error, mail_Message);
						ServerReturnMessage serverReturnMessage = this.DelayedDeliveryMessage;
						if (serverReturnMessage == null)
						{
							string bodyTextRft = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Verdana;}{\\f1\\fnil\\fcharset186 Verdana;}{\\f2\\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}{\\f3\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n{\\colortbl ;\\red0\\green64\\blue128;\\red255\\green0\\blue0;\\red0\\green128\\blue0;}\r\n\\viewkind4\\uc1\\pard\\f0\\fs20 This e-mail is generated by the Server(\\cf1 <#relay.hostname>\\cf0 )  to notify you, \\par\r\n\\lang1061\\f1 that \\lang1033\\f0 your message to \\cf1 <#relay.to>\\cf0  dated \\b\\fs16 <#message.header[\"Date:\"]>\\b0  \\fs20 could not be sent at the first attempt.\\par\r\n\\par\r\nRecipient \\i <#relay.to>'\\i0 s Server (\\cf1 <#relay.session_hostname>\\cf0 ) returned the following response: \\cf2 <#relay.error>\\cf0\\par\r\n\\par\r\n\\par\r\nPlease note Server will attempt to deliver this message for \\b <#relay.undelivered_after>\\b0  hours.\\par\r\n\\par\r\n--------\\par\r\n\\par\r\nYour original message is attached to this e-mail (\\b data.eml\\b0 )\\par\r\n\\par\r\n\\fs16 The tracking number for this message is \\cf3 <#relay.session_messageid>\\cf0\\fs20\\par\r\n\\lang1061\\f2\\par\r\n\\pard\\lang1033\\f3\\fs17\\par\r\n}\r\n";
							serverReturnMessage = new ServerReturnMessage("Delayed delivery notice: <#message.header[\"Subject:\"]>", bodyTextRft);
						}
						string rtfText = relayVariablesManager.Process(serverReturnMessage.BodyTextRtf);
						Mail_Message mail_Message2 = DeliveryStatusNotification.CreateDsnMessage(session.From, relayVariablesManager.Process(serverReturnMessage.Subject), rtfText, relayMessageInfo.EnvelopeID, relayMessageInfo.Date, null, (session.IsConnected && string.IsNullOrEmpty(session.LocalHostName)) ? session.LocalEndPoint.Address.ToString() : session.LocalHostName, relayMessageInfo.OriginalRecipient, session.To, "delayed", error, session.RemoteHostName, DateTime.Now, relayMessageInfo.Date.AddMinutes((double)this.UndeliveredAfter), (relayMessageInfo.DSN_Ret == SMTP_DSN_Ret.NotSpecified) ? SMTP_DSN_Ret.FullMessage : relayMessageInfo.DSN_Ret, mail_Message);
						using (MemoryStream memoryStream = new MemoryStream())
						{
							mail_Message2.ToStream(memoryStream, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8);
							this.m_pVirtualServer.ProcessAndStoreMessage("", new string[]
							{
								session.From
							}, memoryStream, null);
						}
						mail_Message.Dispose();
						mail_Message2.Dispose();
					}
				}
			}
			catch (Exception x)
			{
				DataSmart.MailServer.Error.DumpError(this.m_pVirtualServer.Name, x);
			}
		}

		private void Send_DSN_Relayed(Relay_Session session)
		{
			if (session == null)
			{
				return;
			}
			try
			{
				if (!string.IsNullOrEmpty(session.From))
				{
					RelayMessageInfo relayMessageInfo = (RelayMessageInfo)session.QueueTag;
					if ((relayMessageInfo.DSN_Notify & SMTP_DSN_Notify.Success) != SMTP_DSN_Notify.NotSpecified)
					{
						session.MessageStream.Position = 0L;
						Mail_Message mail_Message = Mail_Message.ParseFromStream(session.MessageStream);
						RelayVariablesManager relayVariablesManager = new RelayVariablesManager(this, session, "", mail_Message);
						ServerReturnMessage serverReturnMessage = null;
						if (serverReturnMessage == null)
						{
							string bodyTextRft = "{\\rtf1\\ansi\\ansicpg1257\\deff0\\deflang1061{\\fonttbl{\\f0\\froman\\fcharset0 Times New Roman;}{\\f1\froman\\fcharset186{\\*\\fname Times New Roman;}Times New Roman Baltic;}{\\f2\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}}\r\n{\\colortbl ;\\red0\\green128\\blue0;\\red128\\green128\\blue128;}\r\n{\\*\\generator Msftedit 5.41.21.2508;}\\viewkind4\\uc1\\pard\\sb100\\sa100\\lang1033\\f0\\fs24\\par\r\nYour message WAS SUCCESSFULLY RELAYED to:\\line\\lang1061\\f1\\tab\\cf1\\lang1033\\b\\f0 <" + session.To + ">\\line\\cf0\\b0 and you explicitly requested a delivery status notification on success.\\par\\par\r\n\\cf2 Your original message\\lang1061\\f1 /header\\lang1033\\f0  is attached to this e-mail\\lang1061\\f1 .\\lang1033\\f0\\par\\r\\n\\cf0\\line\\par\r\n\\pard\\lang1061\\f2\\fs20\\par\r\n}\r\n";
							serverReturnMessage = new ServerReturnMessage("DSN SUCCESSFULLY RELAYED: <#message.header[\"Subject:\"]>", bodyTextRft);
						}
						string rtfText = relayVariablesManager.Process(serverReturnMessage.BodyTextRtf);
						Mail_Message mail_Message2 = DeliveryStatusNotification.CreateDsnMessage(session.From, relayVariablesManager.Process(serverReturnMessage.Subject), rtfText, relayMessageInfo.EnvelopeID, relayMessageInfo.Date, null, (session.IsConnected && string.IsNullOrEmpty(session.LocalHostName)) ? session.LocalEndPoint.Address.ToString() : session.LocalHostName, relayMessageInfo.OriginalRecipient, session.To, "relayed", "200 OK", session.RemoteHostName, DateTime.MinValue, DateTime.MinValue, (relayMessageInfo.DSN_Ret == SMTP_DSN_Ret.NotSpecified) ? SMTP_DSN_Ret.Headers : relayMessageInfo.DSN_Ret, mail_Message);
						using (MemoryStream memoryStream = new MemoryStream())
						{
							mail_Message2.ToStream(memoryStream, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8);
							this.m_pVirtualServer.ProcessAndStoreMessage("", new string[]
							{
								session.From
							}, memoryStream, null);
						}
						mail_Message.Dispose();
						mail_Message2.Dispose();
					}
				}
			}
			catch (Exception x)
			{
				DataSmart.MailServer.Error.DumpError(this.m_pVirtualServer.Name, x);
			}
		}
	}
}

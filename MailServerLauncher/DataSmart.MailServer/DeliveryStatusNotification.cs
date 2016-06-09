using System.NetworkToolkit.Mail;
using System.NetworkToolkit.MIME;
using System.NetworkToolkit.SMTP;
using System;
using System.IO;
using System.Text;

namespace DataSmart.MailServer
{
	public class DeliveryStatusNotification
	{
		public static Mail_Message CreateDsnMessage(string to, string subject, string rtfText, string envelopeID, DateTime arrivalDate, string receivedFromMTA, string reportingMTA, string originalRecipient, string finalRecipient, string action, string statusCode_text, string remoteMTA, DateTime lastAttempt, DateTime retryUntil, SMTP_DSN_Ret ret, Mail_Message message)
		{
			rtfText = rtfText.Replace("\r\n", "\n").Replace("\n", "\r\n");
			Mail_Message mail_Message = new Mail_Message();
			mail_Message.MimeVersion = "1.0";
			mail_Message.Date = DateTime.Now;
			mail_Message.From = new Mail_t_MailboxList();
			mail_Message.From.Add(new Mail_t_Mailbox("Mail Delivery Subsystem", "postmaster@local"));
			mail_Message.To = new Mail_t_AddressList();
			mail_Message.To.Add(new Mail_t_Mailbox(null, to));
			mail_Message.Subject = subject;
			MIME_h_ContentType mIME_h_ContentType = new MIME_h_ContentType(MIME_MediaTypes.Multipart.report);
			mIME_h_ContentType.Parameters["report-type"] = "delivery-status";
			mIME_h_ContentType.Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.');
			MIME_b_MultipartReport mIME_b_MultipartReport = new MIME_b_MultipartReport(mIME_h_ContentType);
			mail_Message.Body = mIME_b_MultipartReport;
			MIME_Entity mIME_Entity = new MIME_Entity();
			MIME_b_MultipartAlternative mIME_b_MultipartAlternative = new MIME_b_MultipartAlternative(new MIME_h_ContentType(MIME_MediaTypes.Multipart.alternative)
			{
				Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.')
			});
			mIME_Entity.Body = mIME_b_MultipartAlternative;
			mIME_b_MultipartReport.BodyParts.Add(mIME_Entity);
			MIME_Entity mIME_Entity2 = new MIME_Entity();
			MIME_b_Text mIME_b_Text = new MIME_b_Text(MIME_MediaTypes.Text.plain);
			mIME_Entity2.Body = mIME_b_Text;
			mIME_b_Text.SetText(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, SCore.RtfToText(rtfText));
			mIME_b_MultipartAlternative.BodyParts.Add(mIME_Entity2);
			MIME_Entity mIME_Entity3 = new MIME_Entity();
			MIME_b_Text mIME_b_Text2 = new MIME_b_Text(MIME_MediaTypes.Text.html);
			mIME_Entity3.Body = mIME_b_Text2;
			mIME_b_Text2.SetText(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, SCore.RtfToHtml(rtfText));
			mIME_b_MultipartAlternative.BodyParts.Add(mIME_Entity3);
			MIME_Entity mIME_Entity4 = new MIME_Entity();
			MIME_b_Message mIME_b_Message = new MIME_b_Message(MIME_MediaTypes.Message.delivery_status);
			mIME_Entity4.Body = mIME_b_Message;
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(envelopeID))
			{
				stringBuilder.Append("Original-Envelope-Id: " + envelopeID + "\r\n");
			}
			stringBuilder.Append("Arrival-Date: " + MIME_Utils.DateTimeToRfc2822(arrivalDate) + "\r\n");
			if (!string.IsNullOrEmpty(receivedFromMTA))
			{
				stringBuilder.Append("Received-From-MTA: dns; " + receivedFromMTA + "\r\n");
			}
			stringBuilder.Append("Reporting-MTA: dns; " + reportingMTA + "\r\n");
			stringBuilder.Append("\r\n");
			if (!string.IsNullOrEmpty(originalRecipient))
			{
				stringBuilder.Append("Original-Recipient: " + originalRecipient + "\r\n");
			}
			stringBuilder.Append("Final-Recipient: rfc822;" + finalRecipient + "\r\n");
			stringBuilder.Append("Action: " + action + "\r\n");
			stringBuilder.Append("Status: " + statusCode_text.Substring(0, 1) + ".0.0\r\n");
			if (!string.IsNullOrEmpty(statusCode_text))
			{
				stringBuilder.Append("Diagnostic-Code: smtp; " + statusCode_text + "\r\n");
			}
			if (!string.IsNullOrEmpty(remoteMTA))
			{
				stringBuilder.Append("Remote-MTA: dns; " + remoteMTA + "\r\n");
			}
			if (lastAttempt != DateTime.MinValue)
			{
				stringBuilder.Append("Last-Attempt-Date: " + MIME_Utils.DateTimeToRfc2822(lastAttempt) + "\r\n");
			}
			if (retryUntil != DateTime.MinValue)
			{
				stringBuilder.Append("Will-Retry-Until: " + MIME_Utils.DateTimeToRfc2822(retryUntil) + "\r\n");
			}
			stringBuilder.Append("\r\n");
			mIME_b_Message.SetData(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())), MIME_TransferEncodings.EightBit);
			mIME_b_MultipartReport.BodyParts.Add(mIME_Entity4);
			if (message != null)
			{
				MIME_Entity mIME_Entity5 = new MIME_Entity();
				MIME_b_MessageRfc822 mIME_b_MessageRfc = new MIME_b_MessageRfc822();
				mIME_Entity5.Body = mIME_b_MessageRfc;
				if (ret == SMTP_DSN_Ret.FullMessage)
				{
					mIME_b_MessageRfc.Message = message;
				}
				else
				{
					MemoryStream memoryStream = new MemoryStream();
					message.Header.ToStream(memoryStream, null, null);
					memoryStream.Position = 0L;
					mIME_b_MessageRfc.Message = Mail_Message.ParseFromStream(memoryStream);
				}
				mIME_b_MultipartReport.BodyParts.Add(mIME_Entity5);
			}
			return mail_Message;
		}
	}
}

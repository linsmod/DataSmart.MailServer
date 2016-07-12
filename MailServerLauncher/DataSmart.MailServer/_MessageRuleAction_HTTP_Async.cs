using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace DataSmart.MailServer
{
	internal class _MessageRuleAction_HTTP_Async
	{
		private string m_Url = "";

		private Stream m_pMessage;

		public _MessageRuleAction_HTTP_Async(string url, Stream message)
		{
			this.m_Url = url;
			this.m_pMessage = message;
			Thread thread = new Thread(new ThreadStart(this.Post));
            thread.Name = "Message Rule Action HTTP Thread";
            thread.Start();
		}

		private void Post()
		{
			try
			{
				WebClient webClient = new WebClient();
				webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=---------------------8c808e3aebd9294");
				string text = "-----------------------8c808e3aebd9294\r\n";
				text += "Content-Disposition: form-data; name=\"file\"; filename=\"mail.eml\"\r\n";
				text += "Content-Type: application/octet-stream\r\n";
				text += "\r\n";
				MemoryStream memoryStream = new MemoryStream();
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				memoryStream.Write(bytes, 0, bytes.Length);
				SCore.StreamCopy(this.m_pMessage, memoryStream);
				bytes = Encoding.UTF8.GetBytes("\r\n-----------------------8c808e3aebd9294--\r\n");
				memoryStream.Write(bytes, 0, bytes.Length);
				webClient.UploadData(this.m_Url, memoryStream.ToArray());
			}
			catch (Exception x)
			{
				Error.DumpError(x);
			}
		}
	}
}

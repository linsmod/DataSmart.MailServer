using System.NetworkToolkit;
using System.NetworkToolkit.IMAP;
using System;
using System.IO;

namespace DataSmart.MailServer
{
	internal class _InternalHeader
	{
		private FileStream m_pFile;

		private IMAP_MessageFlags m_MessageFlags = IMAP_MessageFlags.Recent;

		private string m_Envelope = "";

		private string m_Body = "";

		public IMAP_MessageFlags MessageFlags
		{
			get
			{
				return this.m_MessageFlags;
			}
		}

		public string Envelope
		{
			get
			{
				return this.m_Envelope;
			}
		}

		public string Body
		{
			get
			{
				return this.m_Body;
			}
		}

		public _InternalHeader(FileStream fs)
		{
			this.m_pFile = fs;
			StreamLineReader streamLineReader = new StreamLineReader(fs);
			string text = streamLineReader.ReadLineString();
			if (text != null && text.ToLower() == "<internalheader>")
			{
				text = streamLineReader.ReadLineString();
				while (text.ToLower() != "</internalheader>")
				{
					if (text.ToLower().StartsWith("#-messageflags:"))
					{
						this.m_MessageFlags = (IMAP_MessageFlags)Enum.Parse(typeof(IMAP_MessageFlags), text.Substring(15).Trim());
					}
					else if (text.ToLower().StartsWith("#-envelope:"))
					{
						this.m_Envelope = text.Substring(11).Trim();
					}
					else if (text.ToLower().StartsWith("#-body:"))
					{
						this.m_Body = text.Substring(7).Trim();
					}
					text = streamLineReader.ReadLineString();
				}
				if (fs.CanWrite)
				{
					byte[] array = new byte[fs.Length - fs.Position];
					fs.Read(array, 0, array.Length);
					fs.Position = 0L;
					fs.Write(array, 0, array.Length);
					fs.SetLength((long)array.Length);
					fs.Position = 0L;
					return;
				}
			}
			else
			{
				fs.Position = 0L;
			}
		}
	}
}

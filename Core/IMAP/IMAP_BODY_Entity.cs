using System.NetworkToolkit.MIME;
using System;
using System.Collections.Generic;

namespace System.NetworkToolkit.IMAP
{
	public class IMAP_BODY_Entity
	{
		private IMAP_BODY_Entity m_pParentEntity;

		private List<IMAP_BODY_Entity> m_pChildEntities;

		private MIME_h_ContentType m_pContentType;

		private string m_ContentID;

		private string m_ContentDescription;

		private string m_ContentEncoding = MIME_TransferEncodings.SevenBit;

		private int m_ContentSize;

		private IMAP_Envelope m_pEnvelope;

		private int m_ContentLines;

		public IMAP_BODY_Entity ParentEntity
		{
			get
			{
				return this.m_pParentEntity;
			}
		}

		public IMAP_BODY_Entity[] ChildEntities
		{
			get
			{
				return this.m_pChildEntities.ToArray();
			}
		}

		public MIME_h_ContentType ContentType
		{
			get
			{
				return this.m_pContentType;
			}
		}

		public string ContentID
		{
			get
			{
				return this.m_ContentID;
			}
		}

		public string ContentDescription
		{
			get
			{
				return this.m_ContentDescription;
			}
		}

		public string ContentTransferEncoding
		{
			get
			{
				return this.m_ContentEncoding;
			}
		}

		public int ContentSize
		{
			get
			{
				if (string.Equals(this.ContentType.Type, "multipart", StringComparison.InvariantCultureIgnoreCase))
				{
					throw new Exception("NOTE: ContentSize property is available only for non-multipart contentype !");
				}
				return this.m_ContentSize;
			}
		}

		public int ContentLines
		{
			get
			{
				if (!string.Equals(this.ContentType.Type, "text", StringComparison.InvariantCultureIgnoreCase))
				{
					throw new Exception("NOTE: ContentLines property is available only for text/xxx content type !");
				}
				return this.m_ContentSize;
			}
		}

		internal IMAP_BODY_Entity()
		{
			this.m_pChildEntities = new List<IMAP_BODY_Entity>();
		}

		internal void Parse(string text)
		{
			StringReader stringReader = new StringReader(text);
			stringReader.ReadToFirstChar();
			if (stringReader.StartsWith("("))
			{
				while (stringReader.StartsWith("("))
				{
					IMAP_BODY_Entity iMAP_BODY_Entity = new IMAP_BODY_Entity();
					iMAP_BODY_Entity.Parse(stringReader.ReadParenthesized());
					iMAP_BODY_Entity.m_pParentEntity = this;
					this.m_pChildEntities.Add(iMAP_BODY_Entity);
					stringReader.ReadToFirstChar();
				}
				string str = stringReader.ReadWord();
				this.m_pContentType = new MIME_h_ContentType("multipart/" + str);
				return;
			}
			string text2 = stringReader.ReadWord();
			string text3 = stringReader.ReadWord();
			if (text2.ToUpper() != "NIL" && text3.ToUpper() != "NIL")
			{
				this.m_pContentType = new MIME_h_ContentType(text2 + "/" + text3);
			}
			stringReader.ReadToFirstChar();
			if (stringReader.StartsWith("("))
			{
				string source = stringReader.ReadParenthesized();
				StringReader stringReader2 = new StringReader(source);
				while (stringReader2.Available > 0L)
				{
					string name = stringReader2.ReadWord();
					string value = MIME_Encoding_EncodedWord.DecodeS(stringReader2.ReadWord());
					this.m_pContentType.Parameters[name] = value;
				}
			}
			else
			{
				stringReader.ReadWord();
			}
			string text4 = stringReader.ReadWord();
			if (text4.ToUpper() != "NIL")
			{
				this.m_ContentID = text4;
			}
			string text5 = stringReader.ReadWord();
			if (text5.ToUpper() != "NIL")
			{
				this.m_ContentDescription = text5;
			}
			string text6 = stringReader.ReadWord();
			if (text6.ToUpper() != "NIL")
			{
				this.m_ContentEncoding = text6;
			}
			string text7 = stringReader.ReadWord();
			if (text7.ToUpper() != "NIL")
			{
				this.m_ContentSize = Convert.ToInt32(text7);
			}
			if (string.Equals(this.ContentType.TypeWithSubype, MIME_MediaTypes.Message.rfc822, StringComparison.InvariantCultureIgnoreCase))
			{
				stringReader.ReadToFirstChar();
				if (stringReader.StartsWith("("))
				{
					stringReader.ReadParenthesized();
				}
				else
				{
					stringReader.ReadWord();
				}
			}
			if (text2.ToLower() == "text")
			{
				string text8 = stringReader.ReadWord();
				if (text8.ToUpper() != "NIL")
				{
					this.m_ContentLines = Convert.ToInt32(text8);
				}
			}
		}
	}
}

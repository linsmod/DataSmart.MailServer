using System.NetworkToolkit.Mail;
using System.NetworkToolkit.MIME;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.NetworkToolkit.IMAP
{
	public class IMAP_BODY
	{
		private IMAP_BODY_Entity m_pMainEntity;

		public IMAP_BODY_Entity MainEntity
		{
			get
			{
				return this.m_pMainEntity;
			}
		}

		public IMAP_BODY_Entity[] Entities
		{
			get
			{
				List<IMAP_BODY_Entity> list = new List<IMAP_BODY_Entity>();
				list.Add(this.m_pMainEntity);
				this.GetEntities(this.m_pMainEntity.ChildEntities, list);
				return list.ToArray();
			}
		}

		public IMAP_BODY_Entity[] Attachmnets
		{
			get
			{
				List<IMAP_BODY_Entity> list = new List<IMAP_BODY_Entity>();
				IMAP_BODY_Entity[] entities = this.Entities;
				IMAP_BODY_Entity[] array = entities;
				for (int i = 0; i < array.Length; i++)
				{
					IMAP_BODY_Entity iMAP_BODY_Entity = array[i];
					if (iMAP_BODY_Entity.ContentType != null)
					{
						foreach (MIME_h_Parameter mIME_h_Parameter in iMAP_BODY_Entity.ContentType.Parameters)
						{
							if (mIME_h_Parameter.Name.ToLower() == "name")
							{
								list.Add(iMAP_BODY_Entity);
								break;
							}
						}
					}
				}
				return list.ToArray();
			}
		}

		public IMAP_BODY()
		{
			this.m_pMainEntity = new IMAP_BODY_Entity();
		}

		public static string ConstructBodyStructure(Mail_Message message, bool bodystructure)
		{
			if (bodystructure)
			{
				return "BODYSTRUCTURE " + IMAP_BODY.ConstructParts(message, bodystructure);
			}
			return "BODY " + IMAP_BODY.ConstructParts(message, bodystructure);
		}

		private static string ConstructParts(MIME_Entity entity, bool bodystructure)
		{
			MIME_Encoding_EncodedWord mIME_Encoding_EncodedWord = new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8);
			mIME_Encoding_EncodedWord.Split = false;
			StringBuilder stringBuilder = new StringBuilder();
			if (entity.Body is MIME_b_Multipart)
			{
				stringBuilder.Append("(");
				foreach (MIME_Entity entity2 in ((MIME_b_Multipart)entity.Body).BodyParts)
				{
					stringBuilder.Append(IMAP_BODY.ConstructParts(entity2, bodystructure));
				}
				if (entity.ContentType != null && entity.ContentType.SubType != null)
				{
					stringBuilder.Append(" \"" + entity.ContentType.SubType + "\"");
				}
				else
				{
					stringBuilder.Append(" NIL");
				}
				stringBuilder.Append(")");
			}
			else
			{
				stringBuilder.Append("(");
				if (entity.ContentType != null && entity.ContentType.Type != null)
				{
					stringBuilder.Append("\"" + entity.ContentType.Type + "\"");
				}
				else
				{
					stringBuilder.Append("NIL");
				}
				if (entity.ContentType != null && entity.ContentType.SubType != null)
				{
					stringBuilder.Append(" \"" + entity.ContentType.SubType + "\"");
				}
				else
				{
					stringBuilder.Append(" NIL");
				}
				if (entity.ContentType != null)
				{
					if (entity.ContentType.Parameters.Count > 0)
					{
						stringBuilder.Append(" (");
						bool flag = true;
						foreach (MIME_h_Parameter mIME_h_Parameter in entity.ContentType.Parameters)
						{
							if (flag)
							{
								flag = false;
							}
							else
							{
								stringBuilder.Append(" ");
							}
							stringBuilder.Append(string.Concat(new string[]
							{
								"\"",
								mIME_h_Parameter.Name,
								"\" \"",
								mIME_Encoding_EncodedWord.Encode(mIME_h_Parameter.Value),
								"\""
							}));
						}
						stringBuilder.Append(")");
					}
					else
					{
						stringBuilder.Append(" NIL");
					}
				}
				else
				{
					stringBuilder.Append(" NIL");
				}
				string contentID = entity.ContentID;
				if (contentID != null)
				{
					stringBuilder.Append(" \"" + mIME_Encoding_EncodedWord.Encode(contentID) + "\"");
				}
				else
				{
					stringBuilder.Append(" NIL");
				}
				string contentDescription = entity.ContentDescription;
				if (contentDescription != null)
				{
					stringBuilder.Append(" \"" + mIME_Encoding_EncodedWord.Encode(contentDescription) + "\"");
				}
				else
				{
					stringBuilder.Append(" NIL");
				}
				if (entity.ContentTransferEncoding != null)
				{
					stringBuilder.Append(" \"" + mIME_Encoding_EncodedWord.Encode(entity.ContentTransferEncoding) + "\"");
				}
				else
				{
					stringBuilder.Append(" \"7bit\"");
				}
				if (entity.Body is MIME_b_SinglepartBase)
				{
					stringBuilder.Append(" " + ((MIME_b_SinglepartBase)entity.Body).EncodedData.Length.ToString());
				}
				else
				{
					stringBuilder.Append(" 0");
				}
				if (entity.Body is MIME_b_MessageRfc822)
				{
					stringBuilder.Append(" " + IMAP_Envelope.ConstructEnvelope(((MIME_b_MessageRfc822)entity.Body).Message));
				}
				if (entity.Body is MIME_b_Text)
				{
					long num = 0L;
					StreamLineReader streamLineReader = new StreamLineReader(new MemoryStream(((MIME_b_SinglepartBase)entity.Body).EncodedData));
					for (byte[] array = streamLineReader.ReadLine(); array != null; array = streamLineReader.ReadLine())
					{
						num += 1L;
					}
					stringBuilder.Append(" " + num.ToString());
				}
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}

		public void Parse(string bodyStructureString)
		{
			this.m_pMainEntity = new IMAP_BODY_Entity();
			this.m_pMainEntity.Parse(bodyStructureString);
		}

		private void GetEntities(IMAP_BODY_Entity[] entities, List<IMAP_BODY_Entity> allEntries)
		{
			if (entities != null)
			{
				for (int i = 0; i < entities.Length; i++)
				{
					IMAP_BODY_Entity iMAP_BODY_Entity = entities[i];
					allEntries.Add(iMAP_BODY_Entity);
					if (iMAP_BODY_Entity.ChildEntities.Length > 0)
					{
						this.GetEntities(iMAP_BODY_Entity.ChildEntities, allEntries);
					}
				}
			}
		}
	}
}

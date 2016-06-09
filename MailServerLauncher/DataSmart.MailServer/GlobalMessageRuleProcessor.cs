using System.NetworkToolkit;
using System.NetworkToolkit.IO;
using System.NetworkToolkit.Mail;
using System.NetworkToolkit.MIME;
using System.NetworkToolkit.SMTP;
using System.NetworkToolkit.SMTP.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DataSmart.MailServer
{
	public class GlobalMessageRuleProcessor
	{
		private enum PossibleClauseItem
		{
			AND = 2,
			OR = 4,
			NOT = 8,
			Parenthesizes = 16,
			Matcher = 32
		}

		public static void CheckMatchExpressionSyntax(string matchExpression)
		{
			GlobalMessageRuleProcessor globalMessageRuleProcessor = new GlobalMessageRuleProcessor();
			globalMessageRuleProcessor.Match(true, new System.NetworkToolkit.StringReader(matchExpression), null, null, null, null, 0);
		}

		public bool Match(string matchExpression, string mailFrom, string[] rcptTo, SMTP_Session smtpSession, Mail_Message mime, int messageSize)
		{
			System.NetworkToolkit.StringReader r = new System.NetworkToolkit.StringReader(matchExpression);
			return this.Match(false, r, mailFrom, rcptTo, smtpSession, mime, messageSize);
		}

		private bool Match(bool syntaxCheckOnly, System.NetworkToolkit.StringReader r, string mailFrom, string[] rcptTo, SMTP_Session smtpSession, Mail_Message mime, int messageSize)
		{
			GlobalMessageRuleProcessor.PossibleClauseItem possibleClauseItem = (GlobalMessageRuleProcessor.PossibleClauseItem)56;
			bool flag = false;
			r.ReadToFirstChar();
			if (r.Available == 0L)
			{
				throw new Exception("Invalid syntax: '" + this.ClauseItemsToString(possibleClauseItem) + "' expected !");
			}
			while (r.Available > 0L)
			{
				r.ReadToFirstChar();
				if (syntaxCheckOnly)
				{
					flag = true;
				}
				if (r.StartsWith("("))
				{
					flag = this.Match(syntaxCheckOnly, new System.NetworkToolkit.StringReader(r.ReadParenthesized()), mailFrom, rcptTo, smtpSession, mime, messageSize);
					possibleClauseItem = (GlobalMessageRuleProcessor.PossibleClauseItem)56;
				}
				else if (r.StartsWith("and", false))
				{
					if ((possibleClauseItem & GlobalMessageRuleProcessor.PossibleClauseItem.AND) == (GlobalMessageRuleProcessor.PossibleClauseItem)0)
					{
						throw new Exception("Invalid syntax: '" + this.ClauseItemsToString(possibleClauseItem) + "' expected !");
					}
					if (!flag)
					{
						return false;
					}
					r.ReadWord();
					r.ReadToFirstChar();
					flag = this.Match(syntaxCheckOnly, r, mailFrom, rcptTo, smtpSession, mime, messageSize);
					possibleClauseItem = (GlobalMessageRuleProcessor.PossibleClauseItem)56;
				}
				else if (r.StartsWith("or", false))
				{
					if ((possibleClauseItem & GlobalMessageRuleProcessor.PossibleClauseItem.OR) == (GlobalMessageRuleProcessor.PossibleClauseItem)0)
					{
						throw new Exception("Invalid syntax: '" + this.ClauseItemsToString(possibleClauseItem) + "' expected !");
					}
					r.ReadWord();
					r.ReadToFirstChar();
					if (flag)
					{
						this.Match(syntaxCheckOnly, r, mailFrom, rcptTo, smtpSession, mime, messageSize);
					}
					else
					{
						flag = this.Match(syntaxCheckOnly, r, mailFrom, rcptTo, smtpSession, mime, messageSize);
					}
					possibleClauseItem = (GlobalMessageRuleProcessor.PossibleClauseItem)56;
				}
				else if (r.StartsWith("not", false))
				{
					if ((possibleClauseItem & GlobalMessageRuleProcessor.PossibleClauseItem.NOT) == (GlobalMessageRuleProcessor.PossibleClauseItem)0)
					{
						throw new Exception("Invalid syntax: '" + this.ClauseItemsToString(possibleClauseItem) + "' expected !");
					}
					r.ReadWord();
					r.ReadToFirstChar();
					flag = !this.Match(syntaxCheckOnly, r, mailFrom, rcptTo, smtpSession, mime, messageSize);
					possibleClauseItem = (GlobalMessageRuleProcessor.PossibleClauseItem)48;
				}
				else
				{
					if ((possibleClauseItem & GlobalMessageRuleProcessor.PossibleClauseItem.Matcher) == (GlobalMessageRuleProcessor.PossibleClauseItem)0)
					{
						throw new Exception(string.Concat(new string[]
						{
							"Invalid syntax: '",
							this.ClauseItemsToString(possibleClauseItem),
							"' expected ! \r\n\r\n Near: '",
							r.OriginalString.Substring(0, r.Position),
							"'"
						}));
					}
					string text = r.ReadWord();
					if (text == null)
					{
						throw new Exception("Invalid syntax: matcher is missing !");
					}
					text = text.ToLower();
					string[] array = new string[0];
					if (text == "smtp.mail_from")
					{
						if (!syntaxCheckOnly)
						{
							array = new string[]
							{
								mailFrom
							};
						}
					}
					else if (text == "smtp.rcpt_to")
					{
						if (!syntaxCheckOnly)
						{
							array = rcptTo;
						}
					}
					else if (text == "smtp.ehlo")
					{
						if (!syntaxCheckOnly)
						{
							array = new string[]
							{
								smtpSession.EhloHost
							};
						}
					}
					else if (text == "smtp.authenticated")
					{
						if (!syntaxCheckOnly && smtpSession != null)
						{
							array = new string[]
							{
								smtpSession.IsAuthenticated.ToString()
							};
						}
					}
					else if (text == "smtp.user")
					{
						if (!syntaxCheckOnly && smtpSession != null && smtpSession.AuthenticatedUserIdentity != null)
						{
							array = new string[]
							{
								smtpSession.AuthenticatedUserIdentity.Name
							};
						}
					}
					else if (text == "smtp.remote_ip")
					{
						if (!syntaxCheckOnly && smtpSession != null)
						{
							array = new string[]
							{
								smtpSession.RemoteEndPoint.Address.ToString()
							};
						}
					}
					else if (text == "message.size")
					{
						if (!syntaxCheckOnly)
						{
							array = new string[]
							{
								messageSize.ToString()
							};
						}
					}
					else if (text == "message.header")
					{
						string text2 = r.ReadWord();
						if (text2 == null)
						{
							throw new Exception("Match source MainHeaderField HeaderFieldName is missing ! Syntax:{MainHeaderField <SP> \"HeaderFieldName:\"}");
						}
						if (!syntaxCheckOnly && mime.Header.Contains(text2))
						{
							MIME_h[] array2 = mime.Header[text2];
							array = new string[array2.Length];
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = array2[i].ValueToString();
							}
						}
					}
					else if (text == "message.all_headers")
					{
						string text3 = r.ReadWord();
						if (text3 == null)
						{
							throw new Exception("Match source MainHeaderField HeaderFieldName is missing ! Syntax:{MainHeaderField <SP> \"HeaderFieldName:\"}");
						}
						if (!syntaxCheckOnly)
						{
							List<string> list = new List<string>();
							MIME_Entity[] allEntities = mime.AllEntities;
							for (int j = 0; j < allEntities.Length; j++)
							{
								MIME_Entity mIME_Entity = allEntities[j];
								if (mIME_Entity.Header.Contains(text3))
								{
									MIME_h[] array3 = mIME_Entity.Header[text3];
									for (int k = 0; k < array3.Length; k++)
									{
										list.Add(array3[k].ValueToString());
									}
								}
							}
							array = list.ToArray();
						}
					}
					else if (text == "message.body_text")
					{
						if (!syntaxCheckOnly)
						{
							array = new string[]
							{
								mime.BodyText
							};
						}
					}
					else if (text == "message.body_html")
					{
						if (!syntaxCheckOnly)
						{
							array = new string[]
							{
								mime.BodyHtmlText
							};
						}
					}
					else if (text == "message.content_md5")
					{
						if (!syntaxCheckOnly)
						{
							List<string> list2 = new List<string>();
							MIME_Entity[] allEntities2 = mime.AllEntities;
							for (int l = 0; l < allEntities2.Length; l++)
							{
								MIME_Entity mIME_Entity2 = allEntities2[l];
								try
								{
									if (mIME_Entity2.Body is MIME_b_SinglepartBase)
									{
										byte[] data = ((MIME_b_SinglepartBase)mIME_Entity2.Body).Data;
										if (data != null)
										{
											MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
											list2.Add(Encoding.UTF8.GetString(mD5CryptoServiceProvider.ComputeHash(data)));
										}
									}
								}
								catch
								{
								}
							}
							array = list2.ToArray();
						}
					}
					else if (text == "sys.date_time")
					{
						if (!syntaxCheckOnly)
						{
							array = new string[]
							{
								DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
							};
						}
					}
					else if (text == "sys.date")
					{
						if (!syntaxCheckOnly)
						{
							array = new string[]
							{
								DateTime.Today.ToString("dd.MM.yyyy")
							};
						}
					}
					else if (text == "sys.time")
					{
						if (!syntaxCheckOnly)
						{
							array = new string[]
							{
								DateTime.Now.ToString("HH:mm:ss")
							};
						}
					}
					else if (text == "sys.day_of_week")
					{
						if (!syntaxCheckOnly)
						{
							array = new string[]
							{
								DateTime.Today.DayOfWeek.ToString()
							};
						}
					}
					else
					{
						if (!(text == "sys.day_of_year"))
						{
							throw new Exception("Unknown match source '" + text + "' !");
						}
						if (!syntaxCheckOnly)
						{
							array = new string[]
							{
								DateTime.Today.ToString("M")
							};
						}
					}
					flag = false;
					text = r.ReadWord(true, new char[]
					{
						' '
					}, true);
					if (text == null)
					{
						throw new Exception("Invalid syntax: operator is missing ! \r\n\r\n Near: '" + r.OriginalString.Substring(0, r.Position) + "'");
					}
					text = text.ToLower();
					if (text == "*")
					{
						string text4 = r.ReadWord();
						if (text4 == null)
						{
							throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
						}
						text4 = text4.ToLower();
						if (!syntaxCheckOnly)
						{
							string[] array4 = array;
							for (int m = 0; m < array4.Length; m++)
							{
								string text5 = array4[m];
								if (SCore.IsAstericMatch(text4, text5.ToLower()))
								{
									flag = true;
									break;
								}
							}
						}
					}
					else if (text == "!*")
					{
						string text6 = r.ReadWord();
						if (text6 == null)
						{
							throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
						}
						text6 = text6.ToLower();
						if (!syntaxCheckOnly)
						{
							string[] array4 = array;
							for (int j = 0; j < array4.Length; j++)
							{
								string text7 = array4[j];
								if (SCore.IsAstericMatch(text6, text7.ToLower()))
								{
									flag = false;
									break;
								}
							}
						}
					}
					else if (text == "==")
					{
						string text8 = r.ReadWord();
						if (text8 == null)
						{
							throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
						}
						text8 = text8.ToLower();
						if (!syntaxCheckOnly)
						{
							string[] array4 = array;
							for (int j = 0; j < array4.Length; j++)
							{
								string text9 = array4[j];
								if (text8 == text9.ToLower())
								{
									flag = true;
									break;
								}
							}
						}
					}
					else if (text == "!=")
					{
						string text10 = r.ReadWord();
						if (text10 == null)
						{
							throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
						}
						text10 = text10.ToLower();
						if (!syntaxCheckOnly)
						{
							string[] array4 = array;
							for (int j = 0; j < array4.Length; j++)
							{
								string text11 = array4[j];
								if (text10 == text11.ToLower())
								{
									flag = false;
									break;
								}
								flag = true;
							}
						}
					}
					else if (text == ">=")
					{
						string text12 = r.ReadWord();
						if (text12 == null)
						{
							throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
						}
						text12 = text12.ToLower();
						if (!syntaxCheckOnly)
						{
							string[] array4 = array;
							for (int j = 0; j < array4.Length; j++)
							{
								string text13 = array4[j];
								if (text13.ToLower().CompareTo(text12) >= 0)
								{
									flag = true;
									break;
								}
							}
						}
					}
					else if (text == "<=")
					{
						string text14 = r.ReadWord();
						if (text14 == null)
						{
							throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
						}
						text14 = text14.ToLower();
						if (!syntaxCheckOnly)
						{
							string[] array4 = array;
							for (int j = 0; j < array4.Length; j++)
							{
								string text15 = array4[j];
								if (text15.ToLower().CompareTo(text14) <= 0)
								{
									flag = true;
									break;
								}
							}
						}
					}
					else if (text == ">")
					{
						string text16 = r.ReadWord();
						if (text16 == null)
						{
							throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
						}
						text16 = text16.ToLower();
						if (!syntaxCheckOnly)
						{
							string[] array4 = array;
							for (int j = 0; j < array4.Length; j++)
							{
								string text17 = array4[j];
								if (text17.ToLower().CompareTo(text16) > 0)
								{
									flag = true;
									break;
								}
							}
						}
					}
					else if (text == "<")
					{
						string text18 = r.ReadWord();
						if (text18 == null)
						{
							throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
						}
						text18 = text18.ToLower();
						if (!syntaxCheckOnly)
						{
							string[] array4 = array;
							for (int j = 0; j < array4.Length; j++)
							{
								string text19 = array4[j];
								if (text19.ToLower().CompareTo(text18) < 0)
								{
									flag = true;
									break;
								}
							}
						}
					}
					else
					{
						if (!(text == "regex"))
						{
							throw new Exception("Unknown keword '" + text + "' !");
						}
						string text20 = r.ReadWord();
						if (text20 == null)
						{
							throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
						}
						text20 = text20.ToLower();
						if (!syntaxCheckOnly)
						{
							string[] array4 = array;
							for (int j = 0; j < array4.Length; j++)
							{
								string text21 = array4[j];
								if (Regex.IsMatch(text20, text21.ToLower()))
								{
									flag = true;
									break;
								}
							}
						}
					}
					possibleClauseItem = (GlobalMessageRuleProcessor.PossibleClauseItem)6;
				}
			}
			return flag;
		}

		public GlobalMessageRuleActionResult DoActions(DataView dvActions, VirtualServer server, Stream message, string sender, string[] to)
		{
			bool deleteMessage = false;
			string storeFolder = null;
			string errorText = null;
			foreach (DataRowView dataRowView in dvActions)
			{
				GlobalMessageRuleActionType globalMessageRuleAction_enum = (GlobalMessageRuleActionType)dataRowView["ActionType"];
				byte[] data = (byte[])dataRowView["ActionData"];
				message.Position = 0L;
				if (globalMessageRuleAction_enum == (GlobalMessageRuleActionType)1)
				{
					XmlTable xmlTable = new XmlTable("ActionData");
					xmlTable.Parse(data);
					string value = xmlTable.GetValue("From");
					string value2 = xmlTable.GetValue("Message");
					MIME_h_Collection mIME_h_Collection = new MIME_h_Collection(new MIME_h_Provider());
					mIME_h_Collection.Parse(new SmartStream(message, false));
					if (!mIME_h_Collection.Contains("X-LS-MailServer-AutoResponse"))
					{
						Mail_Message mail_Message = Mail_Message.ParseFromByte(Encoding.UTF8.GetBytes(value2));
						mail_Message.Header.Add(new MIME_h_Unstructured("X-LS-MailServer-AutoResponse", ""));
						mail_Message.Date = DateTime.Now;
						if (mail_Message.To == null || mail_Message.To.Count == 0)
						{
							if (mail_Message.To == null)
							{
								mail_Message.To = new Mail_t_AddressList
								{
									new Mail_t_Mailbox(null, sender)
								};
							}
							else
							{
								mail_Message.To.Add(new Mail_t_Mailbox(null, sender));
							}
						}
						if (mail_Message.Subject != null && mIME_h_Collection.Contains("Subject"))
						{
							mail_Message.Subject = mail_Message.Subject.Replace("#SUBJECT", mIME_h_Collection.GetFirst("Subject").ValueToString().Trim());
						}
						if (!string.IsNullOrEmpty(sender))
						{
							server.ProcessAndStoreMessage(value, new string[]
							{
								sender
							}, new MemoryStream(mail_Message.ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8)), null);
						}
					}
				}
				else if (globalMessageRuleAction_enum ==  GlobalMessageRuleActionType.DeleteMessage)
				{
					XmlTable xmlTable2 = new XmlTable("ActionData");
					xmlTable2.Parse(data);
					deleteMessage = true;
				}
				else if (globalMessageRuleAction_enum == GlobalMessageRuleActionType.ExecuteProgram)
				{
					XmlTable xmlTable3 = new XmlTable("ActionData");
					xmlTable3.Parse(data);
					Process.Start(new ProcessStartInfo
					{
						FileName = xmlTable3.GetValue("Program"),
						Arguments = xmlTable3.GetValue("Arguments"),
						CreateNoWindow = true
					});
				}
				else if (globalMessageRuleAction_enum ==  GlobalMessageRuleActionType.ForwardToEmail)
				{
					XmlTable xmlTable4 = new XmlTable("ActionData");
					xmlTable4.Parse(data);
					MIME_h_Collection mIME_h_Collection2 = new MIME_h_Collection(new MIME_h_Provider());
					mIME_h_Collection2.Parse(new SmartStream(message, false));
					bool flag = false;
					if (mIME_h_Collection2.Contains("X-LS-MailServer-ForwardedTo"))
					{
						MIME_h[] array = mIME_h_Collection2["X-LS-MailServer-ForwardedTo"];
						for (int i = 0; i < array.Length; i++)
						{
							MIME_h mIME_h = array[i];
							if (mIME_h.ValueToString().Trim() == xmlTable4.GetValue("Email"))
							{
								flag = true;
								break;
							}
						}
					}
					message.Position = 0L;
					if (!flag)
					{
						MemoryStream memoryStream = new MemoryStream();
						byte[] bytes = Encoding.UTF8.GetBytes("X-LS-MailServer-ForwardedTo: " + xmlTable4.GetValue("Email") + "\r\n");
						memoryStream.Write(bytes, 0, bytes.Length);
						SCore.StreamCopy(message, memoryStream);
						server.ProcessAndStoreMessage(sender, new string[]
						{
							xmlTable4.GetValue("Email")
						}, memoryStream, null);
					}
				}
				else if (globalMessageRuleAction_enum ==  GlobalMessageRuleActionType.ForwardToHost)
				{
					XmlTable xmlTable5 = new XmlTable("ActionData");
					xmlTable5.Parse(data);
					for (int j = 0; j < to.Length; j++)
					{
						string to2 = to[j];
						message.Position = 0L;
						server.RelayServer.StoreRelayMessage(Guid.NewGuid().ToString(), null, message, HostEndPoint.Parse(xmlTable5.GetValue("Host") + ":" + xmlTable5.GetValue("Port")), sender, to2, null, SMTP_DSN_Notify.NotSpecified, SMTP_DSN_Ret.NotSpecified);
					}
					message.Position = 0L;
				}
				else
				{
					if (globalMessageRuleAction_enum ==  GlobalMessageRuleActionType.StoreToDiskFolder)
					{
						XmlTable xmlTable6 = new XmlTable("ActionData");
						xmlTable6.Parse(data);
						string text = xmlTable6.GetValue("Folder");
						if (!text.EndsWith("\\"))
						{
							text += "\\";
						}
						if (!Directory.Exists(text))
						{
							continue;
						}
						using (FileStream fileStream = File.Create(string.Concat(new string[]
						{
							text,
							DateTime.Now.ToString("ddMMyyyyHHmmss"),
							"_",
							Guid.NewGuid().ToString().Replace('-', '_').Substring(0, 8),
							".eml"
						})))
						{
							SCore.StreamCopy(message, fileStream);
							continue;
						}
					}
					if (globalMessageRuleAction_enum ==  GlobalMessageRuleActionType.StoreToIMAPFolder)
					{
						XmlTable xmlTable7 = new XmlTable("ActionData");
						xmlTable7.Parse(data);
						storeFolder = xmlTable7.GetValue("Folder");
					}
					else if (globalMessageRuleAction_enum == (GlobalMessageRuleActionType)8)
					{
						XmlTable xmlTable8 = new XmlTable("ActionData");
						xmlTable8.Parse(data);
						Mail_Message mail_Message2 = Mail_Message.ParseFromStream(message);
						mail_Message2.Header.Add(new MIME_h_Unstructured(xmlTable8.GetValue("HeaderFieldName"), xmlTable8.GetValue("HeaderFieldValue")));
						message.SetLength(0L);
						mail_Message2.ToStream(message, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8);
					}
					else if (globalMessageRuleAction_enum == (GlobalMessageRuleActionType)9)
					{
						XmlTable xmlTable9 = new XmlTable("ActionData");
						xmlTable9.Parse(data);
						Mail_Message mail_Message3 = Mail_Message.ParseFromStream(message);
						mail_Message3.Header.RemoveAll(xmlTable9.GetValue("HeaderFieldName"));
						message.SetLength(0L);
						mail_Message3.ToStream(message, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8);
					}
					else if (globalMessageRuleAction_enum == (GlobalMessageRuleActionType)10)
					{
						XmlTable xmlTable10 = new XmlTable("ActionData");
						xmlTable10.Parse(data);
						errorText = xmlTable10.GetValue("ErrorText");
					}
					else if (globalMessageRuleAction_enum ==(GlobalMessageRuleActionType) 11)
					{
						XmlTable xmlTable11 = new XmlTable("ActionData");
						xmlTable11.Parse(data);
						new _MessageRuleAction_FTP_AsyncSend(xmlTable11.GetValue("Server"), Convert.ToInt32(xmlTable11.GetValue("Port")), xmlTable11.GetValue("User"), xmlTable11.GetValue("Password"), xmlTable11.GetValue("Folder"), message, DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Guid.NewGuid().ToString().Replace('-', '_').Substring(0, 8) + ".eml");
					}
					else if (globalMessageRuleAction_enum ==(GlobalMessageRuleActionType) 12)
					{
						XmlTable xmlTable12 = new XmlTable("ActionData");
						xmlTable12.Parse(data);
						Mail_Message mail_Message4 = Mail_Message.ParseFromStream(message);
						if (!mail_Message4.Header.Contains("Newsgroups:"))
						{
							mail_Message4.Header.Add(new MIME_h_Unstructured("Newsgroups:", xmlTable12.GetValue("Newsgroup")));
						}
						new _MessageRuleAction_NNTP_Async(xmlTable12.GetValue("Server"), Convert.ToInt32(xmlTable12.GetValue("Port")), xmlTable12.GetValue("Newsgroup"), new MemoryStream(mail_Message4.ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8)));
					}
					else if (globalMessageRuleAction_enum ==(GlobalMessageRuleActionType) 13)
					{
						XmlTable xmlTable13 = new XmlTable("ActionData");
						xmlTable13.Parse(data);
						new _MessageRuleAction_HTTP_Async(xmlTable13.GetValue("URL"), message);
					}
				}
			}
			return new GlobalMessageRuleActionResult(deleteMessage, storeFolder, errorText);
		}

		private string ClauseItemsToString(GlobalMessageRuleProcessor.PossibleClauseItem clauseItems)
		{
			string text = "";
			if ((clauseItems & GlobalMessageRuleProcessor.PossibleClauseItem.AND) != (GlobalMessageRuleProcessor.PossibleClauseItem)0)
			{
				text += "AND,";
			}
			if ((clauseItems & GlobalMessageRuleProcessor.PossibleClauseItem.Matcher) != (GlobalMessageRuleProcessor.PossibleClauseItem)0)
			{
				text += "Matcher,";
			}
			if ((clauseItems & GlobalMessageRuleProcessor.PossibleClauseItem.NOT) != (GlobalMessageRuleProcessor.PossibleClauseItem)0)
			{
				text += "NOT,";
			}
			if ((clauseItems & GlobalMessageRuleProcessor.PossibleClauseItem.OR) != (GlobalMessageRuleProcessor.PossibleClauseItem)0)
			{
				text += "OR,";
			}
			if ((clauseItems & GlobalMessageRuleProcessor.PossibleClauseItem.Parenthesizes) != (GlobalMessageRuleProcessor.PossibleClauseItem)0)
			{
				text += "Parenthesizes,";
			}
			if (text.EndsWith(","))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text.Trim();
		}
	}
}

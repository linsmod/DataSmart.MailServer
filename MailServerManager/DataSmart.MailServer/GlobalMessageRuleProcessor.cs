using System.NetworkToolkit;
using System.NetworkToolkit.Mail;
using System.NetworkToolkit.MIME;
using System.NetworkToolkit.SMTP.Server;
using System;
using System.Collections.Generic;
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
			globalMessageRuleProcessor.Match(true, new StringReader(matchExpression), null, null, null, null, 0);
		}

		public bool Match(string matchExpression, string mailFrom, string[] rcptTo, SMTP_Session smtpSession, Mail_Message mime, int messageSize)
		{
			StringReader r = new StringReader(matchExpression);
			return this.Match(false, r, mailFrom, rcptTo, smtpSession, mime, messageSize);
		}

		private bool Match(bool syntaxCheckOnly, StringReader r, string mailFrom, string[] rcptTo, SMTP_Session smtpSession, Mail_Message mime, int messageSize)
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
					flag = this.Match(syntaxCheckOnly, new StringReader(r.ReadParenthesized()), mailFrom, rcptTo, smtpSession, mime, messageSize);
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

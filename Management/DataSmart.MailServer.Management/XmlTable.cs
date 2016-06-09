using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace DataSmart.MailServer.Management
{
	public class XmlTable
	{
		private string m_TableName = "";

		private Hashtable m_pValues;

		public string TableName
		{
			get
			{
				return this.m_TableName;
			}
			set
			{
				this.m_TableName = value;
			}
		}

		public XmlTable(string tableName)
		{
			if (tableName == null || tableName == "")
			{
				throw new Exception("Table name can't be empty !");
			}
			this.m_TableName = tableName;
			this.m_pValues = new Hashtable();
		}

		public void Add(string name, string value)
		{
			if (this.m_pValues.ContainsKey(name))
			{
				throw new Exception("Specified name '" + name + "' already exists !");
			}
			this.m_pValues.Add(name, value);
		}

		public string GetValue(string name)
		{
			if (!this.m_pValues.ContainsKey(name))
			{
				throw new Exception("Specified name '" + name + "' doesn't exists !");
			}
			return this.m_pValues[name].ToString();
		}

		public void Parse(byte[] data)
		{
			this.m_pValues.Clear();
			MemoryStream input = new MemoryStream(data);
			XmlTextReader xmlTextReader = new XmlTextReader(input);
			xmlTextReader.Read();
			this.m_TableName = xmlTextReader.LocalName;
			while (xmlTextReader.Read())
			{
				if (xmlTextReader.NodeType == XmlNodeType.Element)
				{
					this.Add(xmlTextReader.LocalName, xmlTextReader.ReadElementString());
				}
			}
		}

		public string ToStringData()
		{
			return Encoding.UTF8.GetString(this.ToByteData());
		}

		public byte[] ToByteData()
		{
			MemoryStream memoryStream = new MemoryStream();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
			xmlTextWriter.WriteStartElement(this.m_TableName);
			xmlTextWriter.WriteRaw("\r\n");
			foreach (DictionaryEntry dictionaryEntry in this.m_pValues)
			{
				xmlTextWriter.WriteRaw("\t");
				xmlTextWriter.WriteStartElement(dictionaryEntry.Key.ToString());
				xmlTextWriter.WriteValue(dictionaryEntry.Value.ToString());
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteRaw("\r\n");
			}
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.Flush();
			return memoryStream.ToArray();
		}
	}
}

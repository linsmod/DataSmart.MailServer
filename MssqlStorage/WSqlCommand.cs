using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace DataSmart.MailServer.MssqlStorage
{
	/// <summary>
	/// Summary description for WSqlCommand.
	/// </summary>
	internal class WSqlCommand : IDisposable
	{
		private SqlCommand m_SqlCmd  = null;
		private string     m_connStr = "";
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="connectionString">Connection string.</param>
		/// <param name="commandText">Command text.</param>
		public WSqlCommand(string connectionString,string commandText)
		{
			m_connStr = connectionString;
			
			m_SqlCmd = new SqlCommand(commandText);
			m_SqlCmd.CommandType    = CommandType.StoredProcedure;
			m_SqlCmd.CommandTimeout = 180;
		}

		#region function Dispose

		public void Dispose()
		{
			if(m_SqlCmd != null){
				m_SqlCmd.Dispose();
			}
		}

		#endregion


		#region function AddParameter

		/// <summary>
		/// Adds parameter to Sql Command.
		/// </summary>
		/// <param name="name">Parameter name.</param>
		/// <param name="dbType">Parameter datatype.</param>
		/// <param name="value">Parameter value.</param>
		public void AddParameter(string name,SqlDbType dbType,object value)
		{		
			SqlDbType dbTyp = dbType;
			object val = value;
			
			if(dbType == SqlDbType.UniqueIdentifier){
				dbTyp = SqlDbType.NVarChar;
				string guid = val.ToString();
				if(guid.Length < 1){
					return;
				}
			}

			m_SqlCmd.Parameters.Add(name,dbTyp).Value = val;
		}

		#endregion

		#region fucntion Execute

		/// <summary>
		/// Executes command.
		/// </summary>
		/// <returns></returns>
		public DataSet Execute()
		{
			DataSet dsRetVal = null;

			using(SqlConnection con = new SqlConnection(m_connStr)){
				con.Open();
				m_SqlCmd.Connection = con;
				
				dsRetVal = new DataSet();
				SqlDataAdapter adapter = new SqlDataAdapter(m_SqlCmd);
				adapter.Fill(dsRetVal);

				adapter.Dispose();
			}

			return dsRetVal;
		}

		#endregion


		#region Properties Implementaion

		/// <summary>
		/// Gets or sets command timeout time.
		/// </summary>
		public int CommandTimeout
		{
			get{ return m_SqlCmd.CommandTimeout; }

			set{ m_SqlCmd.CommandTimeout = value; }
		}

		/// <summary>
		/// Gets or sets command type.
		/// </summary>
		public CommandType CommandType
		{
			get{ return m_SqlCmd.CommandType; }

			set{ m_SqlCmd.CommandType = value; }
		}

		#endregion

	}
}

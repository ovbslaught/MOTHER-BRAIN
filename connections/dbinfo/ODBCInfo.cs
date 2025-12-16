using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data.Common;
using runnerDotNet;

namespace runnerDotNet
{
	public class ODBCInfo : DBInfo
	{
		public ODBCInfo(Connection connObj) : base(connObj)
		{ }
		
		public override XVar db_gettablelist()
		{
			XVar ret = XVar.Array();
			
			var dbcon = ((System.Data.Odbc.OdbcConnection)conn.connect().Value);
			try
			{
				dbcon.Open();
				
				System.Data.DataTable schemaTable = dbcon.GetSchema( "Tables" );

				foreach (System.Data.DataRow row in schemaTable.Rows)
				{
					String schema = row["TABLE_SCHEM"].ToString(),
						table = row["TABLE_NAME"].ToString(),
						type = row["TABLE_TYPE"].ToString();
					if( schema != "" )
					{
						table = schema + "." + table;
					}
					if (type == "TABLE" || type == "VIEW")
						ret.Add(table);
				}
			}
			finally
			{
				dbcon.Close();
			}
			return ret;
		}
	}
}
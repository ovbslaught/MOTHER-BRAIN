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
	public class OLEDBInfo : DBInfo
	{
		public OLEDBInfo(Connection connObj) : base(connObj)
		{ }
		
		public override XVar db_gettablelist()
		{
			XVar ret = XVar.Array();
			
			var dbcon = ((System.Data.OleDb.OleDbConnection)conn.connect().Value);
			try
			{
				dbcon.Open();
				System.Data.DataTable schemaTable = dbcon.GetOleDbSchemaTable(
					System.Data.OleDb.OleDbSchemaGuid.Tables,
					new object[] { null, null, null, "TABLE" });

				foreach (System.Data.DataRow row in schemaTable.Rows)
				{
					if (row["TABLE_TYPE"].ToString() == "TABLE" || row["TABLE_TYPE"].ToString() == "VIEW")
						ret.Add(row["TABLE_NAME"].ToString());
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
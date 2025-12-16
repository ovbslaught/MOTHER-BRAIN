using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data.Common;
using runnerDotNet;

/**
 * The base class containing methods 
 * extracting the database info
 */
namespace runnerDotNet
{
	public class DBInfo : XClass
	{
		protected Connection conn;
		
		public DBInfo(Connection connObj)
		{
			conn = connObj;
		}
		
		public virtual XVar db_gettablelist()
		{
			XVar ret = XVar.Array();
			return ret;
		}
			
		public virtual XVar db_getfieldslist(XVar strSQL)
		{
			XVar res = XVar.Array();
			RunnerDBReader rs = conn.query(strSQL).getQueryHandle() as RunnerDBReader;
			for (int i = 0; i < rs.FieldCount; i++)
			{
				res[i] = new XVar("fieldname", rs.GetName(i), "type", rs.GetFieldType(i), "not_null", 0);
			}
			rs.Connection.Close();
			return res;
		}
	}
}
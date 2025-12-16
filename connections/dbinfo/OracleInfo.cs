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
	public class OracleInfo : DBInfo
	{
		public OracleInfo(Connection connObj) : base(connObj)
		{ }
		
		public override XVar db_gettablelist()
		{
			XVar ret = XVar.Array();
			XVar strSQL = @"select owner||'.'||table_name as name,'TABLE' as type from all_tables where owner not like '%SYS%'
				 union all
				 select owner||'.'||view_name as name,'VIEW' from all_views where owner not like '%SYS%'";
			var rs = conn.query(strSQL);
			XVar data;
			while (data = rs.fetchNumeric())
			{
				ret.Add(data[0]);
			}
			return ret;
		}
	}
}
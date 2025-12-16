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
	public class PostgreInfo : DBInfo
	{
		public PostgreInfo(Connection connObj) : base(connObj)
		{ }
		
		public override XVar db_gettablelist()
		{
			XVar ret = XVar.Array();
			XVar strSQL = @"select schemaname||'.'||tablename as name from pg_tables where schemaname not in ('pg_catalog','information_schema')
						 union all
						 select schemaname||'.'||viewname as name from pg_views where schemaname not in ('pg_catalog','information_schema')";
			var rs = conn.query(strSQL);
			XVar data;
			while (data = rs.fetchAssoc())
				ret.Add(data["name"]);
			
			return ret;
		}
	}
}
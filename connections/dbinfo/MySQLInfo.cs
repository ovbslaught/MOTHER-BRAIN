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
	public class MySQLInfo : DBInfo
	{
		public MySQLInfo(Connection connObj) : base(connObj)
		{ }
		
		public override XVar db_gettablelist()
		{
			dynamic ret = XVar.Array();
			var strSQL = "select DATABASE() as dbname";
			var rs = conn.query(strSQL);
			var data = rs.fetchAssoc();
			if(data == false)
				return ret;

			var dbname = data["dbname"];
			var query = conn.query("SELECT VERSION() as mysql_version");
			data = query.fetchAssoc();
			var server_info = 0;
			if(data != false)
				server_info = data["mysql_version"];
			if(server_info >= 5)
			{
				strSQL = MVCFunctions.Concat("SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA = '", dbname, "'");
				rs = conn.query(strSQL);
				while(data = rs.fetchAssoc())
					ret.Add(data["TABLE_NAME"]);

				strSQL = MVCFunctions.Concat("SELECT TABLE_NAME FROM information_schema.VIEWS WHERE TABLE_SCHEMA = '", dbname, "'");
				rs = conn.query(strSQL);
				while(data = rs.fetchAssoc())
					if(!MVCFunctions.in_array(data["TABLE_NAME"], ret))
						ret.Add(data["TABLE_NAME"]);
				MVCFunctions.sort(ref ret);
			}
			else
			{
				strSQL = "SHOW tables";
				rs = conn.query(strSQL);
				while(data = CommonFunctions.db_fetch_numarray(rs))
					ret.Add(data[0]);
			}

			return ret;
		}
		
	}
}
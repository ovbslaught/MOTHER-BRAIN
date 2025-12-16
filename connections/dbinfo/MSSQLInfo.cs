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
	public class MSSQLInfo : DBInfo
	{
		public MSSQLInfo(Connection connObj) : base(connObj)
		{ }
		public override XVar db_gettablelist()
		{
			XVar ret = XVar.Array();
			XVar strSQL = "sp_tables";
			var rs = conn.query(strSQL);
			XVar data;
			while (data = rs.fetchAssoc())
			{
				if (data["TABLE_OWNER"].ToUpper() != "SYS" && data["TABLE_OWNER"].ToUpper() != "INFORMATION_SCHEMA")
				{
					ret.Add(MVCFunctions.Concat(data["TABLE_OWNER"], ".", data["TABLE_NAME"]));
				}
			}
			return ret;
		}
	}
}
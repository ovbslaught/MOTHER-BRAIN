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
	public class SQLite3Connection : Connection
	{
		public SQLite3Connection(XVar parameters) : base(parameters)
		{ }
		
		protected override string GetLastInsertedIdSql()
		{
			return "select last_insert_rowid()";
		}
		
		protected override DbCommand GetCommand()
		{
			return new System.Data.SQLite.SQLiteCommand();
		}
		
		protected override ConnectionsPool GetConnectionsPool(string connStr)
		{
			return new SQLiteConnectionPool(connStr);
		}
		
		protected override DBFunctions GetDbFunctions( XVar extraParams)
		{
			return new SQLite3Functions( extraParams );
		}
		
		protected override DBInfo GetDbInfo()
		{
			return new SQLite3Info(this);
		}
	}
}
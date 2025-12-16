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
	public class OLEDBConnection : Connection
	{
		public OLEDBConnection(XVar parameters) : base(parameters)
		{ }
		
		
		protected override ConnectionsPool GetConnectionsPool(string connStr)
		{
			return new OLEDBConnectionPool(connStr);
		}
		
		protected override DbCommand GetCommand()
		{
			return new System.Data.OleDb.OleDbCommand();
		}
		
	}
}
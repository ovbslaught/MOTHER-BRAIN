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
	public class ConnectionManager : ConnectionManager_Base
	{
		XVar _connectionsData;
		public ConnectionManager() {
			GlobalVars.ConnectionStrings = XVar.Clone(XVar.Array());
			for(int i = 0; i < System.Configuration.ConfigurationManager.ConnectionStrings.Count; i++)
				GlobalVars.ConnectionStrings[System.Configuration.ConfigurationManager.ConnectionStrings[i].Name] = System.Configuration.ConfigurationManager.ConnectionStrings[i].ConnectionString;
		}

		protected override XVar getConnection(dynamic connId)
		{
			if(connId == "")
				return getDefault();

			if( !_connectionsData ) {
				_setConnectionsData();
			}
			XVar data = _connectionsData[connId];

			switch(data["connStringType"].ToString())
			{
				case "msaccess":
				case "odbc":
				case "odbcdsn":
				case "custom":
				case "file":
				case "db2":
				{
					string firstClause = GlobalVars.ConnectionStrings[connId].ToString().Substring(0, 9).ToUpper();
					if(  firstClause == "PROVIDER=" )
						return new OLEDBConnection(data);
					else
						return new ODBCConnection(data);
				}
				case "sqlite":
					return new SQLite3Connection(data);
				default:
					return null;
			}
		}

		/**
		 * Set the data representing the project's
		 * db connection properties
		 */
		protected XVar _setConnectionsData()
		{
			// content of this function can be modified on demo account
			// variable names data and connectionsData are important

			var connectionsData = XVar.Array();
			XVar data;
			connectionsData["conn"] = new XVar( "dbType", "6",
"connStringType", "sqlite",
"connId", "conn",
"connName", "api.md",
"leftWrap", "",
"rightWrap", "",
"EncryptInfo", new XVar(  ) );
			_connectionsData = connectionsData;
			return null;
		}

		public virtual XVar getDefaultConnId()
		{
			return XVar.Clone(ProjectSettings.getProjectValue(new XVar("defaultConnID")));
		}

	}
}
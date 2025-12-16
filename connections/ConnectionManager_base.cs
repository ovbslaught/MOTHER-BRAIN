using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using runnerDotNet;
namespace runnerDotNet
{
	public partial class ConnectionManager_Base : XClass
	{
		protected dynamic cache = XVar.Array();
		public ConnectionManager_Base()
		{
		}
		public virtual XVar getTableConnId(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic projectTables = XVar.Array(), tableObj = XVar.Array();
			projectTables = ProjectSettings.getProjectTables();
			tableObj = XVar.Clone(projectTables[table]);
			if(XVar.Pack(tableObj))
			{
				return tableObj["connId"];
			}
			return this.getDefaultConnId();
		}
		public virtual XVar byTable(dynamic _param_tName)
		{
			#region pass-by-value parameters
			dynamic tName = XVar.Clone(_param_tName);
			#endregion

			return this.byId((XVar)(this.getTableConnId((XVar)(tName))));
		}
		public virtual XVar byName(dynamic _param_connName)
		{
			#region pass-by-value parameters
			dynamic connName = XVar.Clone(_param_connName);
			#endregion

			dynamic connId = null;
			connId = XVar.Clone(this.getIdByName((XVar)(connName)));
			if(XVar.Pack(!(XVar)(connId)))
			{
				return this.getDefault();
			}
			return this.byId((XVar)(connId));
		}
		protected virtual XVar getIdByName(dynamic _param_connName)
		{
			#region pass-by-value parameters
			dynamic connName = XVar.Clone(_param_connName);
			#endregion

			return CommonFunctions.runnerConnectionIdByName((XVar)(connName));
		}
		public virtual XVar byId(dynamic _param_connId)
		{
			#region pass-by-value parameters
			dynamic connId = XVar.Clone(_param_connId);
			#endregion

			if(XVar.Pack(!(XVar)(connId)))
			{
				connId = XVar.Clone(this.getDefaultConnId());
			}
			if(XVar.Pack(!(XVar)(this.cache.KeyExists(connId))))
			{
				dynamic conn = null;
				conn = XVar.Clone(this.getConnection((XVar)(connId)));
				if((XVar)(!(XVar)(conn))  && (XVar)(GlobalVars.restApis))
				{
					conn = XVar.Clone(GlobalVars.restApis.getConnection((XVar)(connId)));
				}
				if(XVar.Pack(!(XVar)(conn)))
				{
					conn = XVar.Clone(this.getConnection((XVar)(this.getDefaultConnId())));
				}
				this.cache.InitAndSetArrayItem(conn, connId);
			}
			return this.cache[connId];
		}
		public virtual XVar getDefault()
		{
			return this.byId((XVar)(this.getDefaultConnId()));
		}
		public virtual XVar getDefaultConnId()
		{
			return ProjectSettings.getProjectValue(new XVar("defaultConnID"));
		}
		public virtual XVar getForLogin()
		{
			return this.byId((XVar)(this.getLoginConnId()));
		}
		public virtual XVar getLoginConnId()
		{
			dynamic db = XVar.Array();
			db = Security.dbProvider();
			if(XVar.Pack(db))
			{
				return db["table"]["connId"];
			}
			return "";
		}
		public virtual XVar getForAudit()
		{
			return this.byId((XVar)(ProjectSettings.getSecurityValue(new XVar("auditAndLocking"), new XVar("loggingTable"), new XVar("connId"))));
		}
		public virtual XVar getForLocking()
		{
			return this.byId((XVar)(ProjectSettings.getSecurityValue(new XVar("auditAndLocking"), new XVar("lockingTable"), new XVar("connId"))));
		}
		public virtual XVar getForUserGroups()
		{
			return this.byId((XVar)(this.getUserGroupsConnId()));
		}
		public virtual XVar getUserGroupsConnId()
		{
			return ProjectSettings.getSecurityValue(new XVar("dpTableConnId"));
		}
		public virtual XVar getForSavedSearches()
		{
			return this.byId((XVar)(this.getSavedSearchesConnId()));
		}
		public virtual XVar getSavedSearchesConnId()
		{
			return ProjectSettings.getProjectValue(new XVar("settingsTable"), new XVar("connId"));
		}
		public virtual XVar getForWebReports()
		{
			return this.byId((XVar)(this.getSavedSearchesConnId()));
		}
		public virtual XVar getWebReportsConnId()
		{
			return ProjectSettings.getProjectValue(new XVar("wrConnectionID"));
		}
		protected virtual XVar getConnection(dynamic _param_connId)
		{
			#region pass-by-value parameters
			dynamic connId = XVar.Clone(_param_connId);
			#endregion

			return false;
		}
		public virtual XVar checkTablesSubqueriesSupport(dynamic _param_dataSourceTName1, dynamic _param_dataSourceTName2)
		{
			#region pass-by-value parameters
			dynamic dataSourceTName1 = XVar.Clone(_param_dataSourceTName1);
			dynamic dataSourceTName2 = XVar.Clone(_param_dataSourceTName2);
			#endregion

			dynamic connId1 = null, connId2 = null, connInfo = XVar.Array();
			connId1 = XVar.Clone(this.getTableConnId((XVar)(dataSourceTName1)));
			connId2 = XVar.Clone(this.getTableConnId((XVar)(dataSourceTName2)));
			if(connId1 != connId2)
			{
				return false;
			}
			connInfo = XVar.Clone(MVCFunctions.runnerGetConnectionInfo((XVar)(connId1)));
			if((XVar)(connInfo["dbType"] == Constants.nDATABASE_Access)  && (XVar)(dataSourceTName1 == dataSourceTName2))
			{
				return false;
			}
			return true;
		}
		public virtual XVar CloseConnections()
		{
			foreach (KeyValuePair<XVar, dynamic> connection in this.cache.GetEnumerator())
			{
				if(XVar.Pack(connection.Value))
				{
					connection.Value.close();
				}
			}

			return null;
		}
	}
}

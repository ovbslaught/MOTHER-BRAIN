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
	public partial class DataSourceWebSQL : DataSource
	{
		protected dynamic report;
		protected static bool skipDataSourceWebSQLCtor = false;
		public DataSourceWebSQL(dynamic _param_connection, dynamic report)
			:base((XVar)"sql")
		{
			if(skipDataSourceWebSQLCtor)
			{
				skipDataSourceWebSQLCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.connection = XVar.Clone(connection);
			this.report = report;
		}
		protected virtual XVar buildSQL(dynamic _param_dc, dynamic _param_addOrder, dynamic _param_replaceFieldList = null)
		{
			#region default values
			if(_param_replaceFieldList as Object == null) _param_replaceFieldList = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic addOrder = XVar.Clone(_param_addOrder);
			dynamic replaceFieldList = XVar.Clone(_param_replaceFieldList);
			#endregion

			dynamic sql = null;
			if(XVar.Pack(!(XVar)(dc._cache.KeyExists("sql"))))
			{
				if(XVar.Pack(!(XVar)(CommonFunctions.IsStoredProcedure((XVar)(this.report["sql"])))))
				{
					dynamic sql_query = null;
					sql_query = XVar.Clone(this.report["sql"]);
					if(this.connection.dbType == Constants.nDATABASE_MSSQLServer)
					{
						dynamic pos = null;
						pos = XVar.Clone(MVCFunctions.strrpos((XVar)(MVCFunctions.strtoupper((XVar)(sql_query))), new XVar("ORDER BY")));
						if(XVar.Pack(pos))
						{
							sql_query = XVar.Clone(MVCFunctions.substr((XVar)(sql_query), new XVar(0), (XVar)(pos)));
						}
					}
					dc._cache.InitAndSetArrayItem(MVCFunctions.Concat("select * from (", sql_query, ") ", this.connection.addFieldWrappers((XVar)(CommonFunctions.generateAlias())), this.report["where"]), "sql");
				}
				else
				{
					dc._cache.InitAndSetArrayItem(this.report["sql"], "sql");
				}
				dc._cache.InitAndSetArrayItem(MVCFunctions.Concat(this.report["sql"], this.report["where"], this.report["group_by"]), "sql");
			}
			sql = XVar.Clone(dc._cache["sql"]);
			return sql;
		}
		protected override XVar getListImpl(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic sql = null;
			sql = XVar.Clone(this.buildSQL((XVar)(dc), new XVar(true)));
			return this.connection.limitedQuery((XVar)(sql), (XVar)(dc.startRecord), (XVar)(dc.reccount), new XVar(true));
		}
	}
}

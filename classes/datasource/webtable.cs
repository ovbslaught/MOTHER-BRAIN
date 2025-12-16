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
	public partial class DataSourceWebTable : DataSourceDbTable
	{
		protected dynamic report;
		protected static bool skipDataSourceWebTableCtor = false;
		public DataSourceWebTable(dynamic _param_name, dynamic _param_connection, dynamic report, dynamic tableInfo)
			:base((XVar)_param_name, (XVar)_param_connection, (XVar)tableInfo)
		{
			if(skipDataSourceWebTableCtor)
			{
				skipDataSourceWebTableCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.tableInfo = tableInfo;
			this.report = report;
		}
		public override XVar getFieldType(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic fieldInfo = XVar.Array();
			fieldInfo = XVar.Clone(CommonFunctions.getArrayElementNC((XVar)(this.tableInfo["fields"]), (XVar)(field)));
			if(XVar.Pack(!(XVar)(fieldInfo)))
			{
				return null;
			}
			return fieldInfo["type"];
		}
		protected override XVar getColumnCount()
		{
			return MVCFunctions.count(this.tableInfo["fields"]);
		}
		public override XVar getColumnList()
		{
			return MVCFunctions.array_keys((XVar)(this.tableInfo["fields"]));
		}
		protected override XVar buildSQL(dynamic _param_dc, dynamic _param_addOrder, dynamic _param_replaceFieldList = null)
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
				dc._cache.InitAndSetArrayItem(MVCFunctions.Concat(this.report["sql"], this.report["where"], this.report["group_by"]), "sql");
			}
			sql = XVar.Clone(dc._cache["sql"]);
			if(XVar.Pack(addOrder))
			{
				sql = MVCFunctions.Concat(sql, this.getOrderClause((XVar)(dc)));
			}
			return sql;
		}
		protected override XVar getOrderClause(dynamic _param_dc, dynamic _param_forceColumnNames = null)
		{
			#region default values
			if(_param_forceColumnNames as Object == null) _param_forceColumnNames = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic forceColumnNames = XVar.Clone(_param_forceColumnNames);
			#endregion

			if(XVar.Pack(dc._cache.KeyExists("order")))
			{
				return dc._cache["order"];
			}
			dc._cache.InitAndSetArrayItem(this.report["order_by"], "order");
			return dc._cache["order"];
		}
	}
}

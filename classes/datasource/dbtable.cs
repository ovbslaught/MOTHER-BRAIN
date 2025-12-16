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
	public partial class DataSourceDbTable : DataSourceTable
	{
		protected dynamic tableInfo;
		protected static bool skipDataSourceDbTableCtor = false;
		public DataSourceDbTable(dynamic _param_name, dynamic _param_connection, dynamic tableInfo)
			:base((XVar)_param_name, (XVar)_param_connection)
		{
			if(skipDataSourceDbTableCtor)
			{
				skipDataSourceDbTableCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.tableInfo = tableInfo;
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
		protected override XVar getAutoincField()
		{
			return Connection.getAutoincField((XVar)(this.tableInfo));
		}
		protected override XVar getColumnCount()
		{
			return MVCFunctions.count(this.tableInfo["fields"]);
		}
		public override XVar getColumnList()
		{
			return MVCFunctions.array_keys((XVar)(this.tableInfo["fields"]));
		}
		protected override XVar getSQLComponents()
		{
			return new XVar("head", "SELECT * ", "from", MVCFunctions.Concat("FROM ", this.connection.addTableWrappers((XVar)(this.tableInfo["fullName"])), " "), "where", "", "groupby", "", "having", "");
		}
		public override XVar deleteSingle(dynamic _param_dc, dynamic _param_requireKeys = null)
		{
			#region default values
			if(_param_requireKeys as Object == null) _param_requireKeys = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic requireKeys = XVar.Clone(_param_requireKeys);
			#endregion

			dynamic sql = null;
			if((XVar)(!(XVar)(dc.keys))  && (XVar)(requireKeys))
			{
				return true;
			}
			sql = XVar.Clone(MVCFunctions.Concat(" DELETE FROM ", this.connection.addTableWrappers((XVar)(this.tableInfo["fullName"])), " WHERE ", this.getWhereClause((XVar)(dc))));
			if(XVar.Pack(this.connection.exec((XVar)(sql))))
			{
				return true;
			}
			this.setError((XVar)(this.connection.lastError()));
			return false;
		}
		public override XVar insertSingle(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic autoincField = null, data = XVar.Array(), fields = XVar.Array(), ret = null, sql = null, values = XVar.Array();
			this.prepareInsertValues((XVar)(dc));
			fields = XVar.Clone(XVar.Array());
			values = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> sv in dc._cache["sqlValues"].GetEnumerator())
			{
				fields.InitAndSetArrayItem(sv.Key, null);
				values.InitAndSetArrayItem(sv.Value, null);
			}
			sql = XVar.Clone(MVCFunctions.Concat("INSERT INTO ", this.connection.addTableWrappers((XVar)(this.tableInfo["fullName"])), "(", MVCFunctions.implode(new XVar(", "), (XVar)(fields)), ")", " VALUES ", "(", MVCFunctions.implode(new XVar(", "), (XVar)(values)), ")"));
			autoincField = XVar.Clone(this.getAutoincField());
			ret = XVar.Clone(this.connection.execWithBlobProcessing((XVar)(sql), (XVar)(dc._cache["blobs"]), (XVar)(dc._cache["blobTypes"]), (XVar)(autoincField)));
			if(XVar.Pack(!(XVar)(ret)))
			{
				this.setError((XVar)(this.connection.lastError()));
				return false;
			}
			data = XVar.Clone(dc.values);
			if((XVar)(autoincField)  && (XVar)(!(XVar)(data.KeyExists(autoincField))))
			{
				data.InitAndSetArrayItem(this.connection.getInsertedId(), autoincField);
			}
			return data;
		}
		public override XVar updateSingle(dynamic _param_dc, dynamic _param_requireKeys = null)
		{
			#region default values
			if(_param_requireKeys as Object == null) _param_requireKeys = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic requireKeys = XVar.Clone(_param_requireKeys);
			#endregion

			dynamic fieldList = XVar.Array(), ret = null, sql = null;
			if((XVar)(!(XVar)(MVCFunctions.count(dc.values)))  || (XVar)((XVar)(!(XVar)(MVCFunctions.count(dc.keys)))  && (XVar)(requireKeys)))
			{
				return true;
			}
			this.prepareInsertValues((XVar)(dc));
			fieldList = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> sv in dc._cache["sqlValues"].GetEnumerator())
			{
				fieldList.InitAndSetArrayItem(MVCFunctions.Concat(sv.Key, "=", sv.Value), null);
			}
			sql = XVar.Clone(MVCFunctions.Concat("UPDATE ", this.connection.addTableWrappers((XVar)(this.tableInfo["fullName"])), " SET ", MVCFunctions.implode(new XVar(",\n "), (XVar)(fieldList)), " WHERE ", this.getWhereClause((XVar)(dc))));
			ret = XVar.Clone(this.connection.execWithBlobProcessing((XVar)(sql), (XVar)(dc._cache["blobs"]), (XVar)(dc._cache["blobTypes"])));
			if(XVar.Pack(ret))
			{
				return true;
			}
			this.setError((XVar)(this.connection.lastError()));
			return false;
		}
	}
}

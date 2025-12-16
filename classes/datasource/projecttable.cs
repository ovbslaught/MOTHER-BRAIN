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
	public partial class DataSourceProjectTable : DataSourceTable
	{
		protected dynamic query;
		protected dynamic cipherer;
		protected static bool skipDataSourceProjectTableCtor = false;
		public DataSourceProjectTable(dynamic _param_name, dynamic _param_pSet_packed, dynamic _param_connection)
			:base((XVar)_param_name, (XVar)_param_connection)
		{
			if(skipDataSourceProjectTableCtor)
			{
				skipDataSourceProjectTableCtor = false;
				return;
			}
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.query = XVar.Clone(pSet.getSQLQuery());
			this.pSet = XVar.UnPackProjectSettings(pSet);
			this.cipherer = XVar.Clone(new RunnerCipherer((XVar)(this._name)));
		}
		protected override XVar getAutoincField()
		{
			foreach (KeyValuePair<XVar, dynamic> keyField in this.getKeyFields().GetEnumerator())
			{
				if(XVar.Pack(this.pSet.isAutoincField((XVar)(keyField.Value))))
				{
					return keyField.Value;
				}
			}
			return null;
		}
		protected override XVar getGroupByFieldList()
		{
			return this.pSet.getListOfFieldsByExprType(new XVar(true));
		}
		protected override XVar getSQLComponents()
		{
			return this.query.getSqlComponents((XVar)(this.connection), (XVar)(this.cipherer));
		}
		protected override XVar isAggregateField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.pSet.isAggregateField((XVar)(field));
		}
		protected override XVar valueNeedsEncrypted(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return (XVar)(this.connection.isEncryptionByPHPEnabled())  && (XVar)(this.cipherer.isFieldEncrypted((XVar)(field)));
		}
		protected override XVar fieldExpression(dynamic _param_field, dynamic _param_modifier = null)
		{
			#region default values
			if(_param_modifier as Object == null) _param_modifier = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic modifier = XVar.Clone(_param_modifier);
			#endregion

			dynamic fieldExpr = null;
			fieldExpr = XVar.Clone(RunnerPage._getFieldSQL((XVar)(field), (XVar)(this.connection), (XVar)(this.pSet)));
			if((XVar)(this.cipherer.isFieldEncrypted((XVar)(field)))  && (XVar)(!(XVar)(this.connection.isEncryptionByPHPEnabled())))
			{
				fieldExpr = XVar.Clone(this.cipherer.GetEncryptedFieldName((XVar)(fieldExpr), (XVar)(field)));
			}
			return this.applyFieldModifier((XVar)(fieldExpr), (XVar)(this.getFieldType((XVar)(field))), (XVar)(modifier));
		}
		protected override XVar dbTableName()
		{
			return this.pSet.getOriginalTableName();
		}
		public override XVar insertSingle(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic autoincField = null, execResult = null, fields = XVar.Array(), sql = null, table = null, values = XVar.Array();
			this.prepareInsertValues((XVar)(dc));
			table = XVar.Clone(this.dbTableName());
			fields = XVar.Clone(XVar.Array());
			values = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> sv in dc._cache["sqlValues"].GetEnumerator())
			{
				fields.InitAndSetArrayItem(sv.Key, null);
				values.InitAndSetArrayItem(sv.Value, null);
			}
			sql = XVar.Clone(MVCFunctions.Concat("INSERT INTO ", this.connection.addTableWrappers((XVar)(table)), "(", MVCFunctions.implode(new XVar(", "), (XVar)(fields)), ")", " VALUES ", "(", MVCFunctions.implode(new XVar(", "), (XVar)(values)), ")"));
			autoincField = XVar.Clone(this.getAutoincField());
			if((XVar)(dc.identiyInsertOff)  && (XVar)(this.connection.dbType == Constants.nDATABASE_MSSQLServer))
			{
				dynamic sqls = XVar.Array();
				sqls = XVar.Clone(XVar.Array());
				sqls.InitAndSetArrayItem(MVCFunctions.Concat("SET IDENTITY_INSERT ", this.connection.addTableWrappers((XVar)(table)), " ON"), null);
				sqls.InitAndSetArrayItem(sql, null);
				sqls.InitAndSetArrayItem(MVCFunctions.Concat("SET IDENTITY_INSERT ", this.connection.addTableWrappers((XVar)(table)), " OFF"), null);
				execResult = XVar.Clone(this.connection.execMultiple((XVar)(sqls)));
			}
			else
			{
				execResult = XVar.Clone(this.connection.execSilentWithBlobProcessing((XVar)(sql), (XVar)(dc._cache["blobs"]), (XVar)(dc._cache["blobTypes"]), (XVar)(autoincField)));
			}
			if(XVar.Pack(execResult))
			{
				dynamic data = XVar.Array();
				data = XVar.Clone(dc.values);
				if((XVar)(autoincField != null)  && (XVar)(!(XVar)(data.KeyExists(autoincField))))
				{
					data.InitAndSetArrayItem(this.connection.getInsertedId(), autoincField);
				}
				return data;
			}
			this.setError((XVar)(this.connection.lastError()));
			return false;
		}
		public override XVar updateMany(dynamic _param_keys, dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			dynamic values = XVar.Clone(_param_values);
			#endregion


			return null;
		}
		protected override XVar getUpdateFieldSQL(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic fname = null, strField = null;
			strField = XVar.Clone(this.pSet.getStrField((XVar)(field)));
			if(strField != XVar.Pack(""))
			{
				return this.connection.addFieldWrappers((XVar)(strField));
			}
			fname = XVar.Clone(this.pSet.getFullFieldName((XVar)(field)));
			if(fname == XVar.Pack(""))
			{
				return this.connection.addFieldWrappers((XVar)(field));
			}
			if(XVar.Pack(!(XVar)(this.pSet.isSQLExpression((XVar)(field)))))
			{
				return MVCFunctions.Concat(this.connection.addTableWrappers((XVar)(this.pSet.getStrOriginalTableName())), ".", this.connection.addFieldWrappers((XVar)(fname)));
			}
			return fname;
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

			dynamic fieldList = XVar.Array(), sql = null, table = null, where = null;
			if((XVar)((XVar)(!(XVar)(MVCFunctions.count(dc.values)))  && (XVar)(!(XVar)(MVCFunctions.count(dc.advValues))))  || (XVar)((XVar)(!(XVar)(MVCFunctions.count(dc.keys)))  && (XVar)(requireKeys)))
			{
				return true;
			}
			this.prepareInsertValues((XVar)(dc));
			table = XVar.Clone(this.dbTableName());
			where = XVar.Clone(this.getWhereClause((XVar)(dc)));
			fieldList = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> sv in dc._cache["sqlValues"].GetEnumerator())
			{
				fieldList.InitAndSetArrayItem(MVCFunctions.Concat(sv.Key, "=", sv.Value), null);
			}
			sql = XVar.Clone(MVCFunctions.Concat("UPDATE ", this.connection.addTableWrappers((XVar)(table)), " SET ", MVCFunctions.implode(new XVar(",\n "), (XVar)(fieldList)), " WHERE ", where));
			if(XVar.Pack(this.connection.execSilentWithBlobProcessing((XVar)(sql), (XVar)(dc._cache["blobs"]), (XVar)(dc._cache["blobTypes"]))))
			{
				return true;
			}
			this.setError((XVar)(this.connection.lastError()));
			return false;
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

			dynamic sql = null, table = null, where = null;
			if((XVar)(!(XVar)(MVCFunctions.count(dc.keys)))  && (XVar)(requireKeys))
			{
				return true;
			}
			table = XVar.Clone(this.dbTableName());
			where = XVar.Clone(this.getWhereClause((XVar)(dc)));
			sql = XVar.Clone(MVCFunctions.Concat(" DELETE FROM ", this.connection.addTableWrappers((XVar)(table)), " WHERE ", where));
			if(XVar.Pack(this.connection.exec((XVar)(sql))))
			{
				return true;
			}
			this.setError((XVar)(this.connection.lastError()));
			return false;
		}
		public override XVar lastAutoincValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Pack(this.pSet.isAutoincField((XVar)(field))))
			{
				return this.connection.getInsertedId();
			}
			return "";
		}
		protected override XVar getColumnCount()
		{
			return this.pSet.getFieldCount();
		}
		public override XVar getColumnList()
		{
			return this.pSet.getFieldsList();
		}
		protected override XVar encryptField(dynamic _param_field, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return this.cipherer.EncryptField((XVar)(field), (XVar)(value));
		}
		public override XVar decryptRecord(dynamic data)
		{
			return this.cipherer.DecryptFetchedArray((XVar)(data));
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

			if(XVar.Pack(dc.order))
			{
				return base.getOrderClause((XVar)(dc), (XVar)(forceColumnNames));
			}
			return this.pSet.getStrOrderBy();
		}
		protected override XVar insertNull(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.pSet.insertNull((XVar)(field));
		}
		public override XVar prepareInsertSQLValue(dynamic _param_field, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return this.cipherer.AddDBQuotes((XVar)(field), (XVar)(value));
		}
	}
}

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
	public partial class DsOperand : XClass
	{
		public dynamic var_type;
		public dynamic value;
		public dynamic modifier;
		public dynamic tochar;
		public dynamic joinData = XVar.Pack(null);
		public dynamic likeWrapper = XVar.Pack(null);
		public DsOperand(dynamic _param_type, dynamic _param_value, dynamic _param_modifier = null, dynamic _param_joinData = null, dynamic _param_likeWrapper = null, dynamic _param_tochar = null)
		{
			#region default values
			if(_param_modifier as Object == null) _param_modifier = new XVar(0);
			if(_param_joinData as Object == null) _param_joinData = new XVar();
			if(_param_likeWrapper as Object == null) _param_likeWrapper = new XVar();
			if(_param_tochar as Object == null) _param_tochar = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			dynamic value = XVar.Clone(_param_value);
			dynamic modifier = XVar.Clone(_param_modifier);
			dynamic joinData = XVar.Clone(_param_joinData);
			dynamic likeWrapper = XVar.Clone(_param_likeWrapper);
			dynamic tochar = XVar.Clone(_param_tochar);
			#endregion

			this.var_type = XVar.Clone(var_type);
			this.value = XVar.Clone(value);
			this.modifier = XVar.Clone(modifier);
			this.joinData = new XVar(null);
			this.likeWrapper = XVar.Clone(likeWrapper);
			this.tochar = XVar.Clone(tochar);
		}
	}
	public partial class DsCondition : XClass
	{
		public dynamic operands;
		public dynamic operation;
		public dynamic caseInsensitive;
		public DsCondition(dynamic _param_operands, dynamic _param_operation, dynamic _param_caseInsensitive = null)
		{
			#region default values
			if(_param_caseInsensitive as Object == null) _param_caseInsensitive = new XVar(Constants.dsCASE_DEFAULT);
			#endregion

			#region pass-by-value parameters
			dynamic operands = XVar.Clone(_param_operands);
			dynamic operation = XVar.Clone(_param_operation);
			dynamic caseInsensitive = XVar.Clone(_param_caseInsensitive);
			#endregion

			this.operands = XVar.Clone(operands);
			this.operation = XVar.Clone(operation);
			this.caseInsensitive = XVar.Clone(caseInsensitive);
		}
		public virtual XVar findFieldValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic fieldFound = null;
			fieldFound = new XVar(false);
			foreach (KeyValuePair<XVar, dynamic> op in this.operands.GetEnumerator())
			{
				if((XVar)(op.Value.var_type == Constants.dsotCONDITION)  && (XVar)(op.Value.value))
				{
					dynamic condition = null, ret = null;
					condition = XVar.Clone(op.Value.value);
					ret = XVar.Clone(condition.findFieldValue((XVar)(field)));
					if(!XVar.Equals(XVar.Pack(ret), XVar.Pack(null)))
					{
						return ret;
					}
				}
				if(op.Value.var_type == Constants.dsotFIELD)
				{
					fieldFound = XVar.Clone(op.Value.value == field);
				}
				if((XVar)(fieldFound)  && (XVar)(op.Value.var_type == Constants.dsotCONST))
				{
					return op.Value.value;
				}
			}
			return null;
		}
	}
	public partial class DsCommand : XClass
	{
		public dynamic filter;
		public dynamic keys = XVar.Array();
		public dynamic extraColumns = XVar.Array();
		public dynamic startRecord = XVar.Pack(0);
		public dynamic reccount = XVar.Pack(-1);
		public dynamic order = XVar.Array();
		public dynamic totals = XVar.Array();
		public dynamic values = XVar.Array();
		public dynamic advValues = XVar.Array();
		public dynamic identiyInsertOff = XVar.Pack(false);
		public dynamic skipAggregated = XVar.Pack(false);
		public dynamic _cache = XVar.Array();
		public virtual XVar allRecords()
		{
			return (XVar)(this.startRecord == 0)  && (XVar)(this.reccount == -1);
		}
		public virtual XVar clearCache()
		{
			this._cache = XVar.Clone(XVar.Array());

			return null;
		}
		public virtual XVar compareRecords(dynamic _param_a, dynamic _param_b)
		{
			#region pass-by-value parameters
			dynamic a = XVar.Clone(_param_a);
			dynamic b = XVar.Clone(_param_b);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> o in this.order.GetEnumerator())
			{
				dynamic field = null, orderMod = null;
				field = XVar.Clone(o.Value["column"]);
				if(XVar.Pack(!(XVar)(field)))
				{
					continue;
				}
				if(a[field] == b[field])
				{
					continue;
				}
				orderMod = XVar.Clone((XVar.Pack(o.Value["dir"] == "ASC") ? XVar.Pack(1) : XVar.Pack(-1)));
				return orderMod * ((XVar.Pack(b[field] < a[field]) ? XVar.Pack(1) : XVar.Pack(-1)));
			}
			return 0;
		}
		public virtual XVar updateTotalOrder()
		{
			dynamic i = null, t = XVar.Array();
			if(XVar.Pack(!(XVar)(this.totals)))
			{
				return null;
			}
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.totals); ++(i))
			{
				t = this.totals[i];
				foreach (KeyValuePair<XVar, dynamic> o in this.order.GetEnumerator())
				{
					if(o.Value["column"] == t["field"])
					{
						t.InitAndSetArrayItem(o.Value["dir"], "direction");
						break;
					}
				}
			}

			return null;
		}
		public virtual XVar findExtraColumn(dynamic _param_alias)
		{
			#region pass-by-value parameters
			dynamic alias = XVar.Clone(_param_alias);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> ec in this.extraColumns.GetEnumerator())
			{
				if(ec.Value.alias == alias)
				{
					return ec.Value;
				}
			}
			return false;
		}
		public virtual XVar getExtraColumnIndex(dynamic _param_alias)
		{
			#region pass-by-value parameters
			dynamic alias = XVar.Clone(_param_alias);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> ec in this.extraColumns.GetEnumerator())
			{
				if(ec.Value.alias == alias)
				{
					return ec.Key;
				}
			}
			return false;
		}
		public virtual XVar findFieldFilterValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Pack(this.keys.KeyExists(field)))
			{
				return this.keys[field];
			}
			if(XVar.Pack(!(XVar)(this.filter)))
			{
				return null;
			}
			return this.filter.findFieldValue((XVar)(field));
		}
		public virtual XVar invertOrder()
		{
			dynamic order = XVar.Array();
			order = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> o in this.order.GetEnumerator())
			{
				dynamic newO = XVar.Array();
				newO = XVar.Clone(o.Value);
				newO.InitAndSetArrayItem((XVar.Pack(newO["dir"] == "DESC") ? XVar.Pack("ASC") : XVar.Pack("DESC")), "dir");
				order.InitAndSetArrayItem(newO, null);
			}
			this.order = XVar.Clone(order);

			return null;
		}
	}
	public partial class DataCondition : XClass
	{
		public static XVar FieldEquals(dynamic _param_fieldname, dynamic _param_value, dynamic _param_fieldModifier = null, dynamic _param_caseInsensitive = null)
		{
			#region default values
			if(_param_fieldModifier as Object == null) _param_fieldModifier = new XVar(0);
			if(_param_caseInsensitive as Object == null) _param_caseInsensitive = new XVar(Constants.dsCASE_DEFAULT);
			#endregion

			#region pass-by-value parameters
			dynamic fieldname = XVar.Clone(_param_fieldname);
			dynamic value = XVar.Clone(_param_value);
			dynamic fieldModifier = XVar.Clone(_param_fieldModifier);
			dynamic caseInsensitive = XVar.Clone(_param_caseInsensitive);
			#endregion

			return new DsCondition((XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotFIELD), (XVar)(fieldname), (XVar)(fieldModifier)), 1, new DsOperand(new XVar(Constants.dsotCONST), (XVar)(value)))), new XVar(Constants.dsopEQUAL), (XVar)(caseInsensitive));
		}
		public static XVar FieldsEqual(dynamic _param_fields, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic fields = XVar.Clone(_param_fields);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic cond = XVar.Array();
			cond = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in fields.GetEnumerator())
			{
				cond.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(f.Value), (XVar)(data[f.Value])), null);
			}
			return DataCondition._And((XVar)(cond));
		}
		public static XVar FieldBetween(dynamic _param_fieldname, dynamic _param_value1, dynamic _param_value2, dynamic _param_caseInsensitive)
		{
			#region pass-by-value parameters
			dynamic fieldname = XVar.Clone(_param_fieldname);
			dynamic value1 = XVar.Clone(_param_value1);
			dynamic value2 = XVar.Clone(_param_value2);
			dynamic caseInsensitive = XVar.Clone(_param_caseInsensitive);
			#endregion

			return new DsCondition((XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotFIELD), (XVar)(fieldname)), 1, new DsOperand(new XVar(Constants.dsotCONST), (XVar)(value1)), 2, new DsOperand(new XVar(Constants.dsotCONST), (XVar)(value2)))), new XVar(Constants.dsopBETWEEN), (XVar)(caseInsensitive));
		}
		public static XVar FieldIs(dynamic _param_fieldname, dynamic _param_operation, dynamic _param_value, dynamic _param_caseInsensitive = null, dynamic _param_modifier = null, dynamic _param_likeWrapper = null, dynamic _param_tochar = null)
		{
			#region default values
			if(_param_caseInsensitive as Object == null) _param_caseInsensitive = new XVar(Constants.dsCASE_DEFAULT);
			if(_param_modifier as Object == null) _param_modifier = new XVar(0);
			if(_param_likeWrapper as Object == null) _param_likeWrapper = new XVar();
			if(_param_tochar as Object == null) _param_tochar = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic fieldname = XVar.Clone(_param_fieldname);
			dynamic operation = XVar.Clone(_param_operation);
			dynamic value = XVar.Clone(_param_value);
			dynamic caseInsensitive = XVar.Clone(_param_caseInsensitive);
			dynamic modifier = XVar.Clone(_param_modifier);
			dynamic likeWrapper = XVar.Clone(_param_likeWrapper);
			dynamic tochar = XVar.Clone(_param_tochar);
			#endregion

			return new DsCondition((XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotFIELD), (XVar)(fieldname), (XVar)(modifier), new XVar(null), new XVar(null), (XVar)(tochar)), 1, new DsOperand(new XVar(Constants.dsotCONST), (XVar)(value), new XVar(0), new XVar(null), (XVar)(likeWrapper)))), (XVar)(operation), (XVar)(caseInsensitive));
		}
		public static XVar FieldIsNull(dynamic _param_fieldname)
		{
			#region pass-by-value parameters
			dynamic fieldname = XVar.Clone(_param_fieldname);
			#endregion

			return new DsCondition((XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotFIELD), (XVar)(fieldname)))), new XVar(Constants.dsopNULL));
		}
		public static XVar FieldIsEmpty(dynamic _param_fieldname)
		{
			#region pass-by-value parameters
			dynamic fieldname = XVar.Clone(_param_fieldname);
			#endregion

			return new DsCondition((XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotFIELD), (XVar)(fieldname)))), new XVar(Constants.dsopEMPTY));
		}
		public static XVar FieldHasList(dynamic _param_fieldname, dynamic _param_operation, dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic fieldname = XVar.Clone(_param_fieldname);
			dynamic operation = XVar.Clone(_param_operation);
			dynamic values = XVar.Clone(_param_values);
			#endregion

			return new DsCondition((XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotFIELD), (XVar)(fieldname)), 1, new DsOperand(new XVar(Constants.dsotLIST), (XVar)(values)))), (XVar)(operation), new XVar(false));
		}
		public static XVar FieldInList(dynamic _param_fieldname, dynamic _param_values, dynamic _param_caseInsensitive = null)
		{
			#region default values
			if(_param_caseInsensitive as Object == null) _param_caseInsensitive = new XVar(Constants.dsCASE_DEFAULT);
			#endregion

			#region pass-by-value parameters
			dynamic fieldname = XVar.Clone(_param_fieldname);
			dynamic values = XVar.Clone(_param_values);
			dynamic caseInsensitive = XVar.Clone(_param_caseInsensitive);
			#endregion

			return new DsCondition((XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotFIELD), (XVar)(fieldname)), 1, new DsOperand(new XVar(Constants.dsotLIST), (XVar)(values)))), new XVar(Constants.dsopIN), (XVar)(caseInsensitive));
		}
		public static XVar SQLIs(dynamic _param_expr, dynamic _param_operation, dynamic _param_value, dynamic _param_caseInsensitive = null)
		{
			#region default values
			if(_param_caseInsensitive as Object == null) _param_caseInsensitive = new XVar(Constants.dsCASE_DEFAULT);
			#endregion

			#region pass-by-value parameters
			dynamic expr = XVar.Clone(_param_expr);
			dynamic operation = XVar.Clone(_param_operation);
			dynamic value = XVar.Clone(_param_value);
			dynamic caseInsensitive = XVar.Clone(_param_caseInsensitive);
			#endregion

			return new DsCondition((XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotSQL), (XVar)(expr)), 1, new DsOperand(new XVar(Constants.dsotCONST), (XVar)(value)))), (XVar)(operation), (XVar)(caseInsensitive));
		}
		public static XVar CaseFieldOrNull(dynamic _param_condition, dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic condition = XVar.Clone(_param_condition);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return new DsCaseExpression((XVar)(new XVar(0, condition)), (XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotFIELD), (XVar)(field), new XVar(0)))), (XVar)(new DsOperand(new XVar(Constants.dsotNULL), new XVar(0))));
		}
		public static XVar CaseConstOrNull(dynamic _param_condition, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic condition = XVar.Clone(_param_condition);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return new DsCaseExpression((XVar)(new XVar(0, condition)), (XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotCONST), (XVar)(value), new XVar(0)))), (XVar)(new DsOperand(new XVar(Constants.dsotNULL), new XVar(0))));
		}
		public static XVar _False()
		{
			return new DsCondition((XVar)(XVar.Array()), new XVar(Constants.dsopFALSE));
		}
		public static XVar _And(dynamic _param_conditions)
		{
			#region pass-by-value parameters
			dynamic conditions = XVar.Clone(_param_conditions);
			#endregion

			dynamic ret = null;
			ret = XVar.Clone(new DsCondition((XVar)(XVar.Array()), new XVar(Constants.dsopAND)));
			foreach (KeyValuePair<XVar, dynamic> c in conditions.GetEnumerator())
			{
				ret.operands.InitAndSetArrayItem(new DsOperand(new XVar(Constants.dsotCONDITION), (XVar)(c.Value)), null);
			}
			return ret;
		}
		public static XVar _Or(dynamic _param_conditions)
		{
			#region pass-by-value parameters
			dynamic conditions = XVar.Clone(_param_conditions);
			#endregion

			dynamic ret = null;
			ret = XVar.Clone(new DsCondition((XVar)(XVar.Array()), new XVar(Constants.dsopOR)));
			foreach (KeyValuePair<XVar, dynamic> c in conditions.GetEnumerator())
			{
				ret.operands.InitAndSetArrayItem(new DsOperand(new XVar(Constants.dsotCONDITION), (XVar)(c.Value)), null);
			}
			return ret;
		}
		public static XVar _Not(dynamic _param_condition)
		{
			#region pass-by-value parameters
			dynamic condition = XVar.Clone(_param_condition);
			#endregion

			return new DsCondition((XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotCONDITION), (XVar)(condition)))), new XVar(Constants.dsopNOT));
		}
		public static XVar SQLCondition(dynamic _param_sql)
		{
			#region pass-by-value parameters
			dynamic sql = XVar.Clone(_param_sql);
			#endregion

			if(XVar.Pack(!(XVar)(sql)))
			{
				return null;
			}
			return new DsCondition((XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotCONST), (XVar)(sql)))), new XVar(Constants.dsopSQL));
		}
	}
	public partial class DsFieldData : XClass
	{
		public dynamic sql;
		public dynamic field;
		public dynamic joinData;
		public dynamic modifier;
		public dynamic alias;
		public DsFieldData(dynamic _param_sql, dynamic _param_alias, dynamic _param_field, dynamic _param_modifier = null, dynamic _param_joinData = null)
		{
			#region default values
			if(_param_modifier as Object == null) _param_modifier = new XVar(0);
			if(_param_joinData as Object == null) _param_joinData = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic sql = XVar.Clone(_param_sql);
			dynamic alias = XVar.Clone(_param_alias);
			dynamic field = XVar.Clone(_param_field);
			dynamic modifier = XVar.Clone(_param_modifier);
			dynamic joinData = XVar.Clone(_param_joinData);
			#endregion

			this.sql = XVar.Clone(sql);
			this.alias = XVar.Clone(alias);
			this.field = XVar.Clone(field);
			this.modifier = XVar.Clone(modifier);
			this.joinData = XVar.Clone(joinData);
		}
	}
	public partial class DsCaseExpression : XClass
	{
		public dynamic conditions = XVar.Array();
		public dynamic values = XVar.Array();
		public dynamic defValue;
		public DsCaseExpression(dynamic _param_conditions, dynamic _param_values, dynamic _param_defValue)
		{
			#region pass-by-value parameters
			dynamic conditions = XVar.Clone(_param_conditions);
			dynamic values = XVar.Clone(_param_values);
			dynamic defValue = XVar.Clone(_param_defValue);
			#endregion

			this.conditions = XVar.Clone(conditions);
			this.values = XVar.Clone(values);
			this.defValue = XVar.Clone(defValue);
		}
	}
	public partial class DsJoinData : XClass
	{
		public dynamic dataSource;
		public dynamic linkField;
		public dynamic displayField;
		public dynamic displayExpression;
		public dynamic longList;
		public dynamic displayAlias;
	}
}

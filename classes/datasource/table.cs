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
	public partial class DataSourceTable : DataSource
	{
		protected static bool skipDataSourceTableCtor = false;
		public DataSourceTable(dynamic _param_name, dynamic _param_connection)
			:base((XVar)_param_name)
		{
			if(skipDataSourceTableCtor)
			{
				skipDataSourceTableCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.connection = XVar.Clone(connection);
		}
		public override XVar prepareSQL(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic order = null, sql = null;
			this.buildWhereHaving((XVar)(dc));
			order = XVar.Clone(this.getOrderClause((XVar)(dc)));
			sql = XVar.Clone(this.buildSQL((XVar)(dc), new XVar(false)));
			return new XVar("sql", sql, "where", dc._cache["where"], "order", order);
		}
		public override XVar overrideSQL(dynamic _param_dc, dynamic _param_sql)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic sql = XVar.Clone(_param_sql);
			#endregion

			dc._cache.InitAndSetArrayItem(true, "overriden");
			dc._cache.InitAndSetArrayItem(sql, "sql");

			return null;
		}
		public override XVar overrideWhere(dynamic _param_dc, dynamic _param_where, dynamic _param_having = null)
		{
			#region default values
			if(_param_having as Object == null) _param_having = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic where = XVar.Clone(_param_where);
			dynamic having = XVar.Clone(_param_having);
			#endregion

			dc._cache.Remove("sql");
			dc._cache.InitAndSetArrayItem(true, "overriden");
			dc._cache.InitAndSetArrayItem(where, "where");
			if(XVar.Pack(dc._cache["having"]))
			{
				dc._cache.InitAndSetArrayItem(where, "having");
			}

			return null;
		}
		public override XVar overrideOrder(dynamic _param_dc, dynamic _param_orderby)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic orderby = XVar.Clone(_param_orderby);
			#endregion

			dc._cache.InitAndSetArrayItem(orderby, "order");

			return null;
		}
		protected virtual XVar getAutoincField()
		{
			return null;
		}
		protected virtual XVar buildWhereHaving(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic context = null, filter = null, filters = XVar.Array();
			if(XVar.Pack(dc._cache.KeyExists("where")))
			{
				return null;
			}
			filter = XVar.Clone(this.addKeysToFilter((XVar)(dc)));
			filters = XVar.Clone(this.splitFilterWhereHaving((XVar)(dc), (XVar)(filter)));
			context = XVar.Clone(new DsFilterBuildContext());
			dc._cache.InitAndSetArrayItem(this.conditionToSQL((XVar)(filters["where"]), (XVar)(context)), "where");
			dc._cache.InitAndSetArrayItem(this.conditionToSQL((XVar)(filters["having"]), (XVar)(context)), "having");
			dc._cache.InitAndSetArrayItem(context, "context");

			return null;
		}
		protected virtual XVar getWhereClause(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			this.buildWhereHaving((XVar)(dc));
			return dc._cache["where"];
		}
		protected virtual XVar getHavingClause(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			this.buildWhereHaving((XVar)(dc));
			return dc._cache["having"];
		}
		protected virtual XVar getOrderClause(dynamic _param_dc, dynamic _param_forceColumnNames = null)
		{
			#region default values
			if(_param_forceColumnNames as Object == null) _param_forceColumnNames = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic forceColumnNames = XVar.Clone(_param_forceColumnNames);
			#endregion

			dynamic columns = XVar.Array(), orderString = null, orderby = XVar.Array();
			if(XVar.Pack(dc._cache.KeyExists("order")))
			{
				return dc._cache["order"];
			}
			columns = new XVar(null);
			orderby = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> of in dc.order.GetEnumerator())
			{
				if(XVar.Pack(of.Value["index"]))
				{
					if(XVar.Pack(!(XVar)(forceColumnNames)))
					{
						orderby.InitAndSetArrayItem(MVCFunctions.Concat(of.Value["index"], " ", of.Value["dir"]), null);
					}
					else
					{
						dynamic column = null;
						if(XVar.Pack(!(XVar)(columns)))
						{
							columns = XVar.Clone(this.getColumnList());
						}
						column = XVar.Clone(columns[(int)of.Value["index"] - 1]);
						if(XVar.Pack(column))
						{
							orderby.InitAndSetArrayItem(MVCFunctions.Concat(this.wrap((XVar)(column)), " ", of.Value["dir"]), null);
						}
					}
				}
				else
				{
					if(XVar.Pack(of.Value["column"]))
					{
						dynamic extraColumn = null;
						extraColumn = XVar.Clone(dc.findExtraColumn((XVar)(of.Value["column"])));
						if(XVar.Pack(extraColumn))
						{
							if(this.connection.dbType == Constants.nDATABASE_Access)
							{
								dynamic columnCount = null, ecIdx = null;
								ecIdx = XVar.Clone(dc.getExtraColumnIndex((XVar)(of.Value["column"])));
								columnCount = XVar.Clone(this.getColumnCount());
								orderby.InitAndSetArrayItem(MVCFunctions.Concat((columnCount + ecIdx) + 1, " ", of.Value["dir"]), null);
							}
							else
							{
								orderby.InitAndSetArrayItem(MVCFunctions.Concat(this.connection.addFieldWrappers((XVar)(of.Value["column"])), " ", of.Value["dir"]), null);
							}
						}
						else
						{
							orderby.InitAndSetArrayItem(MVCFunctions.Concat(this.fieldExpression((XVar)(of.Value["column"])), " ", of.Value["dir"]), null);
						}
					}
					else
					{
						if(XVar.Pack(of.Value["expr"]))
						{
							orderby.InitAndSetArrayItem(MVCFunctions.Concat(of.Value["expr"], " ", of.Value["dir"]), null);
						}
						else
						{
							continue;
						}
					}
				}
			}
			orderString = XVar.Clone((XVar.Pack(orderby) ? XVar.Pack(MVCFunctions.Concat("ORDER BY ", MVCFunctions.implode(new XVar(", "), (XVar)(orderby)))) : XVar.Pack("")));
			dc._cache.InitAndSetArrayItem(orderString, "orderby");
			return dc._cache["orderby"];
		}
		protected virtual XVar getGroupByFieldList()
		{
			return XVar.Array();
		}
		protected virtual XVar splitFilterWhereHaving(dynamic _param_dc, dynamic _param_filter)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic filter = XVar.Clone(_param_filter);
			#endregion

			dynamic grouppedFields = null, operands = XVar.Array(), ret = XVar.Array();
			ret = XVar.Clone(new XVar("where", DataCondition._And((XVar)(XVar.Array())), "having", DataCondition._And((XVar)(XVar.Array()))));
			grouppedFields = XVar.Clone(this.getGroupByFieldList());
			if(XVar.Pack(!(XVar)(grouppedFields)))
			{
				ret.InitAndSetArrayItem(filter, "where");
				return ret;
			}
			this.flattenANDs((XVar)(filter));
			if(!XVar.Equals(XVar.Pack(filter.operation), XVar.Pack(Constants.dsopAND)))
			{
				operands = XVar.Clone(new XVar(0, new DsOperand(new XVar(Constants.dsotCONDITION), (XVar)(filter))));
			}
			else
			{
				operands = filter.operands;
			}
			foreach (KeyValuePair<XVar, dynamic> oper in operands.GetEnumerator())
			{
				if(XVar.Pack(this.findField((XVar)(oper.Value.value), (XVar)(grouppedFields))))
				{
					ret["having"].operands.InitAndSetArrayItem(oper.Value, null);
				}
				else
				{
					ret["where"].operands.InitAndSetArrayItem(oper.Value, null);
				}
			}
			return ret;
		}
		protected virtual XVar findField(dynamic _param_condition, dynamic _param_fields)
		{
			#region pass-by-value parameters
			dynamic condition = XVar.Clone(_param_condition);
			dynamic fields = XVar.Clone(_param_fields);
			#endregion

			if(XVar.Pack(!(XVar)(condition)))
			{
				return false;
			}
			foreach (KeyValuePair<XVar, dynamic> op in condition.operands.GetEnumerator())
			{
				if(XVar.Equals(XVar.Pack(op.Value.var_type), XVar.Pack(Constants.dsotFIELD)))
				{
					if(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(op.Value.value), (XVar)(fields))), XVar.Pack(false)))
					{
						return true;
					}
				}
				else
				{
					if(XVar.Equals(XVar.Pack(op.Value.var_type), XVar.Pack(Constants.dsotCONDITION)))
					{
						if(XVar.Pack(this.findField((XVar)(op.Value.value), (XVar)(fields))))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		protected virtual XVar conditionToSQL(dynamic _param_dCondition, dynamic _param_context)
		{
			#region pass-by-value parameters
			dynamic dCondition = XVar.Clone(_param_dCondition);
			dynamic context = XVar.Clone(_param_context);
			#endregion

			dynamic op = null, sql = null;
			if(XVar.Pack(!(XVar)(dCondition)))
			{
				return "";
			}
			op = XVar.Clone(dCondition.operation);
			if((XVar)(op == Constants.dsopAND)  || (XVar)(op == Constants.dsopOR))
			{
				dynamic sqlConditions = XVar.Array();
				sqlConditions = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> o in dCondition.operands.GetEnumerator())
				{
					sql = XVar.Clone(MVCFunctions.trim((XVar)(this.conditionToSQL((XVar)(o.Value.value), (XVar)(context)))));
					if(!XVar.Equals(XVar.Pack(sql), XVar.Pack("")))
					{
						sqlConditions.InitAndSetArrayItem(MVCFunctions.Concat("( ", sql, " )"), null);
					}
				}
				return MVCFunctions.implode((XVar)((XVar.Pack(op == Constants.dsopAND) ? XVar.Pack(" and ") : XVar.Pack(" or "))), (XVar)(sqlConditions));
			}
			else
			{
				if(op == Constants.dsopSQL)
				{
					return MVCFunctions.trim((XVar)(dCondition.operands[0].value));
				}
				else
				{
					if(op == Constants.dsopNOT)
					{
						sql = XVar.Clone(MVCFunctions.trim((XVar)(this.conditionToSQL((XVar)(dCondition.operands[0].value), (XVar)(context)))));
						if(XVar.Pack(sql))
						{
							return MVCFunctions.Concat("NOT ( ", sql, " )");
						}
						return "";
					}
					else
					{
						if(XVar.Pack(base.basicFieldCondition((XVar)(op))))
						{
							return this.basicFieldConditionSQL((XVar)(dCondition), (XVar)(context));
						}
						else
						{
							if(op == Constants.dsopFALSE)
							{
								return this.sqlExpressionFalse();
							}
						}
					}
				}
			}
			return "";
		}
		protected virtual XVar valueNeedsEncrypted(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return false;
		}
		protected virtual XVar basicFieldConditionSQL(dynamic _param_dCondition, dynamic _param_context)
		{
			#region pass-by-value parameters
			dynamic dCondition = XVar.Clone(_param_dCondition);
			dynamic context = XVar.Clone(_param_context);
			#endregion

			dynamic caseInsensitive = null, encryptValue = null, field = null, fieldExpr = null, fieldType = null, joinData = null, modifier = null, op = null, operandType = null;
			operandType = XVar.Clone(dCondition.operands[0].var_type);
			field = XVar.Clone(dCondition.operands[0].value);
			modifier = XVar.Clone(dCondition.operands[0].modifier);
			fieldType = XVar.Clone(this.getFieldType((XVar)(field)));
			caseInsensitive = XVar.Clone(dCondition.caseInsensitive);
			if((XVar)(CommonFunctions.IsTextType((XVar)(fieldType)))  && (XVar)(this.connection.dbType == Constants.nDATABASE_MSSQLServer))
			{
				caseInsensitive = new XVar(Constants.dsCASE_DEFAULT);
			}
			encryptValue = XVar.Clone(this.valueNeedsEncrypted((XVar)(field)));
			if(operandType == Constants.dsotFIELD)
			{
				fieldExpr = XVar.Clone(this.fieldExpression((XVar)(field), (XVar)(modifier)));
			}
			else
			{
				if(operandType == Constants.dsotSQL)
				{
					fieldExpr = XVar.Clone(field);
					fieldType = new XVar(0);
				}
			}
			if(XVar.Pack(context.useSubquery))
			{
				fieldExpr = XVar.Clone(this.connection.addFieldWrappers((XVar)(field)));
				encryptValue = new XVar(false);
			}
			joinData = XVar.Clone(dCondition.operands[0].joinData);
			if(XVar.Pack(joinData))
			{
				if(XVar.Pack(joinData.dataSource.tableBased()))
				{
					return this.createJoinedSearchClause((XVar)(field), (XVar)(fieldExpr), (XVar)(dCondition));
				}
				return "";
			}
			if((XVar)(dCondition.operands[0].tochar)  && (XVar)(!(XVar)(CommonFunctions.IsCharType((XVar)(fieldType)))))
			{
				fieldExpr = XVar.Clone(this.connection.field2char((XVar)(fieldExpr), (XVar)(fieldType)));
				fieldType = new XVar(200);
			}
			if((XVar)(CommonFunctions.IsDateFieldType((XVar)(fieldType)))  && (XVar)(XVar.Pack(0) < modifier))
			{
				fieldType = new XVar(3);
			}
			if(XVar.Pack(encryptValue))
			{
				dynamic encryptedEmptyString = null, i = null, values = XVar.Array();
				values = XVar.Clone(XVar.Array());
				i = new XVar(1);
				for(;i < MVCFunctions.count(dCondition.operands); ++(i))
				{
					values.InitAndSetArrayItem(this.encryptField((XVar)(field), (XVar)(dCondition.operands[i].value)), null);
				}
				if((XVar)(dCondition.operation == Constants.dsopBETWEEN)  && (XVar)(MVCFunctions.count(values) == 2))
				{
					return MVCFunctions.Concat(fieldExpr, " = ", values[0], " or ", fieldExpr, " = ", values[1]);
				}
				if(dCondition.operation == Constants.dsopNULL)
				{
					encryptedEmptyString = XVar.Clone(this.encryptField((XVar)(field), (XVar)(dCondition.operands[i].value)));
					return MVCFunctions.Concat(fieldExpr, " is null or ", fieldExpr, " = ''");
				}
				if(dCondition.operation == Constants.dsopEMPTY)
				{
					encryptedEmptyString = XVar.Clone(this.encryptField((XVar)(field), (XVar)(dCondition.operands[i].value)));
					return MVCFunctions.Concat(fieldExpr, " is null or ", fieldExpr, " = '' or ", fieldExpr, " = ", this.connection.prepareString((XVar)(encryptedEmptyString)));
				}
				return MVCFunctions.Concat(fieldExpr, " = ", this.connection.prepareString((XVar)(values[0])));
			}
			op = XVar.Clone(dCondition.operation);
			if(XVar.Pack(!(XVar)(this.checkBasicOpOperands((XVar)(op), (XVar)(fieldType), (XVar)(dCondition.operands)))))
			{
				return this.sqlExpressionFalse();
			}
			if(op == Constants.dsopEQUAL)
			{
				return this.sqlExpressionEquals((XVar)(fieldType), (XVar)(fieldExpr), (XVar)(dCondition.operands[1].value), (XVar)(caseInsensitive));
			}
			else
			{
				if((XVar)(op == Constants.dsopMORE)  || (XVar)(op == Constants.dsopLESS))
				{
					return this.sqlExpressionMore((XVar)(fieldType), (XVar)(fieldExpr), (XVar)(dCondition.operands[1].value), (XVar)(caseInsensitive), (XVar)(op == Constants.dsopMORE));
				}
				else
				{
					if(op == Constants.dsopEMPTY)
					{
						return this.sqlExpressionEmpty((XVar)(fieldType), (XVar)(fieldExpr));
					}
					else
					{
						if(op == Constants.dsopNULL)
						{
							return this.sqlExpressionNull((XVar)(fieldExpr));
						}
						else
						{
							if((XVar)(op == Constants.dsopSTART)  || (XVar)(op == Constants.dsopCONTAIN))
							{
								return this.sqlExpressionLike((XVar)(fieldType), (XVar)(fieldExpr), (XVar)(dCondition.operands[1].value), (XVar)(caseInsensitive), (XVar)(op == Constants.dsopSTART), (XVar)(dCondition.operands[1].likeWrapper));
							}
							else
							{
								if(op == Constants.dsopBETWEEN)
								{
									return this.sqlExpressionBetween((XVar)(fieldType), (XVar)(fieldExpr), (XVar)(dCondition.operands[1].value), (XVar)(dCondition.operands[2].value), (XVar)(caseInsensitive));
								}
								else
								{
									if((XVar)(op == Constants.dsopSOME_IN_LIST)  || (XVar)(op == Constants.dsopALL_IN_LIST))
									{
										return this.sqlExpressionInList((XVar)(fieldType), (XVar)(fieldExpr), (XVar)(dCondition.operands[1].value), (XVar)(op == Constants.dsopALL_IN_LIST));
									}
									else
									{
										if(op == Constants.dsopIN)
										{
											return this.sqlExpressionIn((XVar)(fieldType), (XVar)(fieldExpr), (XVar)(dCondition.operands[1].value), (XVar)(caseInsensitive));
										}
									}
								}
							}
						}
					}
				}
			}
			return "";
		}
		protected virtual XVar checkBasicOpOperands(dynamic _param_op, dynamic _param_fieldType, dynamic _param_operands)
		{
			#region pass-by-value parameters
			dynamic op = XVar.Clone(_param_op);
			dynamic fieldType = XVar.Clone(_param_fieldType);
			dynamic operands = XVar.Clone(_param_operands);
			#endregion

			dynamic i = null, opCount = null;
			opCount = new XVar(1);
			if(op == Constants.dsopBETWEEN)
			{
				opCount = new XVar(3);
			}
			else
			{
				if((XVar)(op != Constants.dsopEMPTY)  && (XVar)(op != Constants.dsopNULL))
				{
					opCount = new XVar(2);
				}
			}
			if(MVCFunctions.count(operands) < opCount)
			{
				return false;
			}
			if((XVar)(op == Constants.dsopSTART)  || (XVar)(op == Constants.dsopCONTAIN))
			{
				return true;
			}
			if((XVar)((XVar)(op == Constants.dsopIN)  || (XVar)(op == Constants.dsopALL_IN_LIST))  || (XVar)(op == Constants.dsopSOME_IN_LIST))
			{
				return true;
			}
			i = new XVar(1);
			for(;i < opCount; ++(i))
			{
				if(XVar.Pack(!(XVar)(this.validateSQLValue((XVar)(fieldType), (XVar)(operands[i].value)))))
				{
					return false;
				}
			}
			return true;
		}
		protected virtual XVar sqlExpressionFalse()
		{
			return "1=0";
		}
		protected virtual XVar sqlExpressionNull(dynamic _param_fieldExpr)
		{
			#region pass-by-value parameters
			dynamic fieldExpr = XVar.Clone(_param_fieldExpr);
			#endregion

			return MVCFunctions.Concat(fieldExpr, " is null");
		}
		protected virtual XVar sqlExpressionEmpty(dynamic _param_fieldType, dynamic _param_fieldExpr)
		{
			#region pass-by-value parameters
			dynamic fieldType = XVar.Clone(_param_fieldType);
			dynamic fieldExpr = XVar.Clone(_param_fieldExpr);
			#endregion

			dynamic ret = null;
			ret = XVar.Clone(MVCFunctions.Concat(fieldExpr, " is null"));
			if((XVar)((XVar)(GlobalVars.mysqlSupportDates0000)  && (XVar)(CommonFunctions.IsDateFieldType((XVar)(fieldType))))  && (XVar)(this.connection.dbType == Constants.nDATABASE_MySQL))
			{
				return MVCFunctions.Concat(ret, " or ", fieldExpr, " = '0000-00-00'", " or ", fieldExpr, " = '0000-00-00 00:00:00'");
			}
			if((XVar)(!(XVar)(CommonFunctions.IsCharType((XVar)(fieldType))))  || (XVar)(this.connection.dbType == Constants.nDATABASE_Oracle))
			{
				return ret;
			}
			if((XVar)(CommonFunctions.IsTextType((XVar)(fieldType)))  && (XVar)(this.connection.dbType == Constants.nDATABASE_MSSQLServer))
			{
				return MVCFunctions.Concat(ret, " or ", fieldExpr, " like ''");
			}
			else
			{
				return MVCFunctions.Concat(ret, " or ", fieldExpr, " = ''");
			}

			return null;
		}
		protected virtual XVar sqlExpressionEquals(dynamic _param_fieldType, dynamic _param_fieldExpr, dynamic _param_value, dynamic _param_caseInsensitive)
		{
			#region pass-by-value parameters
			dynamic fieldType = XVar.Clone(_param_fieldType);
			dynamic fieldExpr = XVar.Clone(_param_fieldExpr);
			dynamic value = XVar.Clone(_param_value);
			dynamic caseInsensitive = XVar.Clone(_param_caseInsensitive);
			#endregion

			if((XVar)(this.connection.dbType == Constants.nDATABASE_PostgreSQL)  && (XVar)(fieldType == Constants.adBoolean))
			{
				return (XVar.Pack(!(XVar)(!(XVar)(value))) ? XVar.Pack(fieldExpr) : XVar.Pack(MVCFunctions.Concat("NOT (", fieldExpr, ")")));
			}
			if(XVar.Pack(!(XVar)(CommonFunctions.IsCharType((XVar)(fieldType)))))
			{
				return MVCFunctions.Concat(fieldExpr, "=", this.prepareSQLValue((XVar)(fieldType), (XVar)(value)));
			}
			if((XVar)(CommonFunctions.IsTextType((XVar)(fieldType)))  && (XVar)(this.connection.dbType != Constants.nDATABASE_MySQL))
			{
				dynamic likeWord = null;
				likeWord = XVar.Clone((XVar.Pack((XVar)(this.connection.dbType == Constants.nDATABASE_PostgreSQL)  && (XVar)(XVar.Equals(XVar.Pack(caseInsensitive), XVar.Pack(Constants.dsCASE_INSENSITIVE)))) ? XVar.Pack("ilike") : XVar.Pack("like")));
				return MVCFunctions.Concat(fieldExpr, " ", likeWord, " ", this.prepareSQLValue((XVar)(fieldType), (XVar)(value)));
			}
			if(XVar.Equals(XVar.Pack(caseInsensitive), XVar.Pack(Constants.dsCASE_DEFAULT)))
			{
				return MVCFunctions.Concat(fieldExpr, "=", this.prepareSQLValue((XVar)(fieldType), (XVar)(value)));
			}
			return this.connection.comparisonSQL((XVar)(fieldExpr), (XVar)(this.prepareSQLValue((XVar)(fieldType), (XVar)(value))), (XVar)(XVar.Equals(XVar.Pack(caseInsensitive), XVar.Pack(Constants.dsCASE_INSENSITIVE))));
		}
		protected virtual XVar sqlExpressionInList(dynamic _param_fieldType, dynamic _param_fieldExpr, dynamic _param_values, dynamic _param_all)
		{
			#region pass-by-value parameters
			dynamic fieldType = XVar.Clone(_param_fieldType);
			dynamic fieldExpr = XVar.Clone(_param_fieldExpr);
			dynamic values = XVar.Clone(_param_values);
			dynamic all = XVar.Clone(_param_all);
			#endregion

			dynamic conditions = XVar.Array();
			if(XVar.Pack(!(XVar)(CommonFunctions.IsCharType((XVar)(fieldType)))))
			{
				fieldExpr = XVar.Clone(this.connection.field2char((XVar)(fieldExpr), (XVar)(fieldType)));
			}
			conditions = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> v in values.GetEnumerator())
			{
				dynamic strV = null, valueConds = XVar.Array(), vp = null;
				valueConds = XVar.Clone(XVar.Array());
				valueConds.InitAndSetArrayItem(MVCFunctions.Concat(fieldExpr, " = ", this.connection.prepareString((XVar)(v.Value))), null);
				vp = XVar.Clone(this.connection.escapeLIKEpattern((XVar)(v.Value)));
				strV = XVar.Clone(this.connection.addSlashes((XVar)(vp)));
				valueConds.InitAndSetArrayItem(MVCFunctions.Concat(fieldExpr, " LIKE '%,", strV, "'"), null);
				valueConds.InitAndSetArrayItem(MVCFunctions.Concat(fieldExpr, " LIKE '", strV, ",%'"), null);
				valueConds.InitAndSetArrayItem(MVCFunctions.Concat(fieldExpr, " LIKE '%,", strV, ",%'"), null);
				conditions.InitAndSetArrayItem(MVCFunctions.Concat("( ", MVCFunctions.implode(new XVar(" OR "), (XVar)(valueConds)), " )"), null);
			}
			return MVCFunctions.implode((XVar)((XVar.Pack(all) ? XVar.Pack(" AND ") : XVar.Pack(" OR "))), (XVar)(conditions));
		}
		protected virtual XVar sqlExpressionIn(dynamic _param_fieldType, dynamic _param_fieldExpr, dynamic _param_values, dynamic _param_caseInsensitive)
		{
			#region pass-by-value parameters
			dynamic fieldType = XVar.Clone(_param_fieldType);
			dynamic fieldExpr = XVar.Clone(_param_fieldExpr);
			dynamic values = XVar.Clone(_param_values);
			dynamic caseInsensitive = XVar.Clone(_param_caseInsensitive);
			#endregion

			dynamic sqlValues = XVar.Array(), uppercase = null;
			sqlValues = XVar.Clone(XVar.Array());
			uppercase = XVar.Clone((XVar)(CommonFunctions.IsCharType((XVar)(fieldType)))  && (XVar)(caseInsensitive == Constants.dsCASE_INSENSITIVE));
			foreach (KeyValuePair<XVar, dynamic> v in values.GetEnumerator())
			{
				sqlValues.InitAndSetArrayItem((XVar.Pack(uppercase) ? XVar.Pack(this.connection.upper((XVar)(this.prepareSQLValue((XVar)(fieldType), (XVar)(v.Value))))) : XVar.Pack(this.prepareSQLValue((XVar)(fieldType), (XVar)(v.Value)))), null);
			}
			if(XVar.Pack(uppercase))
			{
				fieldExpr = XVar.Clone(this.connection.upper((XVar)(fieldExpr)));
			}
			return MVCFunctions.Concat(fieldExpr, " IN ( ", MVCFunctions.implode(new XVar(", "), (XVar)(sqlValues)), " )");
		}
		protected virtual XVar sqlExpressionMore(dynamic _param_fieldType, dynamic _param_fieldExpr, dynamic _param_value, dynamic _param_caseInsensitive, dynamic _param_more)
		{
			#region pass-by-value parameters
			dynamic fieldType = XVar.Clone(_param_fieldType);
			dynamic fieldExpr = XVar.Clone(_param_fieldExpr);
			dynamic value = XVar.Clone(_param_value);
			dynamic caseInsensitive = XVar.Clone(_param_caseInsensitive);
			dynamic more = XVar.Clone(_param_more);
			#endregion

			dynamic operation = null, valueExpression = null;
			operation = XVar.Clone((XVar.Pack(more) ? XVar.Pack(" > ") : XVar.Pack(" < ")));
			valueExpression = XVar.Clone(this.prepareSQLValue((XVar)(fieldType), (XVar)(value)));
			if((XVar)(CommonFunctions.IsCharType((XVar)(fieldType)))  && (XVar)(XVar.Equals(XVar.Pack(caseInsensitive), XVar.Pack(Constants.dsCASE_INSENSITIVE))))
			{
				if((XVar)(!(XVar)(CommonFunctions.IsTextType((XVar)(fieldType))))  || (XVar)(this.connection.dbType == Constants.nDATABASE_MySQL))
				{
					fieldExpr = XVar.Clone(this.connection.upper((XVar)(fieldExpr)));
					valueExpression = XVar.Clone(this.connection.upper((XVar)(valueExpression)));
				}
			}
			return MVCFunctions.Concat(fieldExpr, operation, valueExpression);
		}
		protected virtual XVar sqlExpressionLike(dynamic _param_fieldType, dynamic _param_fieldExpr, dynamic _param_value, dynamic _param_caseInsensitive, dynamic _param_starts, dynamic _param_likeWrapper)
		{
			#region pass-by-value parameters
			dynamic fieldType = XVar.Clone(_param_fieldType);
			dynamic fieldExpr = XVar.Clone(_param_fieldExpr);
			dynamic value = XVar.Clone(_param_value);
			dynamic caseInsensitive = XVar.Clone(_param_caseInsensitive);
			dynamic starts = XVar.Clone(_param_starts);
			dynamic likeWrapper = XVar.Clone(_param_likeWrapper);
			#endregion

			dynamic likeWord = null, pattern = null, valueExpression = null;
			if(XVar.Pack(!(XVar)(CommonFunctions.IsCharType((XVar)(fieldType)))))
			{
				fieldExpr = XVar.Clone(this.connection.field2char((XVar)(fieldExpr), (XVar)(fieldType)));
			}
			likeWord = XVar.Clone((XVar.Pack((XVar)(this.connection.dbType == Constants.nDATABASE_PostgreSQL)  && (XVar)(XVar.Equals(XVar.Pack(caseInsensitive), XVar.Pack(Constants.dsCASE_INSENSITIVE)))) ? XVar.Pack("ilike") : XVar.Pack("like")));
			if(XVar.Pack(CommonFunctions.IsNumberType((XVar)(fieldType))))
			{
				value = XVar.Clone(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)(value)));
			}
			pattern = XVar.Clone(MVCFunctions.Concat(this.connection.escapeLIKEpattern((XVar)(value)), "%"));
			if(XVar.Pack(!(XVar)(starts)))
			{
				pattern = XVar.Clone(MVCFunctions.Concat("%", pattern));
			}
			if(XVar.Pack(likeWrapper))
			{
				dynamic after = null, before = null;
				before = XVar.Clone(this.connection.escapeLIKEpattern((XVar)(likeWrapper["before"])));
				after = XVar.Clone(this.connection.escapeLIKEpattern((XVar)(likeWrapper["after"])));
				if((XVar)(!(XVar)(starts))  && (XVar)(before))
				{
					before = XVar.Clone(MVCFunctions.Concat("%", before));
				}
				if(XVar.Pack(after))
				{
					after = XVar.Clone(MVCFunctions.Concat(after, "%"));
				}
				pattern = XVar.Clone(MVCFunctions.Concat(before, pattern, after));
			}
			if(this.connection.dbType == Constants.nDATABASE_Access)
			{
				pattern = XVar.Clone(MVCFunctions.str_replace(new XVar("["), new XVar("_"), (XVar)(pattern)));
			}
			valueExpression = XVar.Clone(this.connection.prepareString((XVar)(pattern)));
			if((XVar)(XVar.Equals(XVar.Pack(caseInsensitive), XVar.Pack(Constants.dsCASE_INSENSITIVE)))  && (XVar)(this.connection.dbType != Constants.nDATABASE_PostgreSQL))
			{
				fieldExpr = XVar.Clone(this.connection.upper((XVar)(fieldExpr)));
				valueExpression = XVar.Clone(this.connection.upper((XVar)(valueExpression)));
			}
			return MVCFunctions.Concat(fieldExpr, " ", likeWord, " ", valueExpression);
		}
		protected virtual XVar sqlExpressionBetween(dynamic _param_fieldType, dynamic _param_fieldExpr, dynamic _param_value1, dynamic _param_value2, dynamic _param_caseInsensitive)
		{
			#region pass-by-value parameters
			dynamic fieldType = XVar.Clone(_param_fieldType);
			dynamic fieldExpr = XVar.Clone(_param_fieldExpr);
			dynamic value1 = XVar.Clone(_param_value1);
			dynamic value2 = XVar.Clone(_param_value2);
			dynamic caseInsensitive = XVar.Clone(_param_caseInsensitive);
			#endregion

			dynamic valueExpression1 = null, valueExpression2 = null;
			valueExpression1 = XVar.Clone(this.prepareSQLValue((XVar)(fieldType), (XVar)(value1)));
			valueExpression2 = XVar.Clone(this.prepareSQLValue((XVar)(fieldType), (XVar)(value2)));
			if((XVar)(CommonFunctions.IsCharType((XVar)(fieldType)))  && (XVar)(XVar.Equals(XVar.Pack(caseInsensitive), XVar.Pack(Constants.dsCASE_INSENSITIVE))))
			{
				if((XVar)(!(XVar)(CommonFunctions.IsTextType((XVar)(fieldType))))  || (XVar)(this.connection.dbType == Constants.nDATABASE_MySQL))
				{
					fieldExpr = XVar.Clone(this.connection.upper((XVar)(fieldExpr)));
					valueExpression1 = XVar.Clone(this.connection.upper((XVar)(valueExpression1)));
					valueExpression2 = XVar.Clone(this.connection.upper((XVar)(valueExpression2)));
				}
			}
			return MVCFunctions.Concat(fieldExpr, " BETWEEN ", valueExpression1, " AND ", valueExpression2);
		}
		protected virtual XVar validateSQLValue(dynamic _param_type, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if(XVar.Pack(CommonFunctions.IsNumberType((XVar)(var_type))))
			{
				return MVCFunctions.IsNumeric(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)(value)));
			}
			if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(var_type))))
			{
				dynamic d = null, delim = null, m = null, matches = null, reg = null, y = null;
				y = new XVar("(\\d\\d\\d\\d)");
				m = new XVar("(0?[1-9]|1[0-2])");
				d = new XVar("(0?[1-9]|[1-2][0-9]|3[0-1])");
				delim = XVar.Clone(MVCFunctions.Concat("(-|", MVCFunctions.preg_quote((XVar)(GlobalVars.locale_info["LOCALE_SDATE"]), new XVar("/")), ")"));
				reg = XVar.Clone(MVCFunctions.Concat("/", d, delim, m, delim, y, "|", m, delim, d, delim, y, "|", y, delim, m, delim, d, "/"));
				return MVCFunctions.preg_match((XVar)(reg), (XVar)(value), (XVar)(matches));
			}
			if(XVar.Pack(CommonFunctions.IsGuid((XVar)(var_type))))
			{
				return CommonFunctions.IsGuidString(ref value);
			}
			if((XVar)(CommonFunctions.IsTimeType((XVar)(var_type)))  && (XVar)(this.connection.dbType == Constants.nDATABASE_PostgreSQL))
			{
				return CommonFunctions.validTimeValue((XVar)(CommonFunctions.localtime2db((XVar)(value))));
			}
			return true;
		}
		protected virtual XVar prepareSQLValue(dynamic _param_type, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if(XVar.Pack(!(XVar)(validateSQLValue((XVar)(var_type), (XVar)(value)))))
			{
				return "NULL";
			}
			if(XVar.Pack(CommonFunctions.IsNumberType((XVar)(var_type))))
			{
				return DB.prepareNumberValue((XVar)(value));
			}
			if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(var_type))))
			{
				return this.connection.addDateQuotes((XVar)(value));
			}
			if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(var_type))))
			{
				return this.connection.addSlashesBinary((XVar)(value));
			}
			return this.connection.prepareString((XVar)(value));
		}
		public virtual XVar versionCompare(dynamic _param_lhs, dynamic _param_rhs)
		{
			#region pass-by-value parameters
			dynamic lhs = XVar.Clone(_param_lhs);
			dynamic rhs = XVar.Clone(_param_rhs);
			#endregion

			dynamic diff = null, i = null, leftList = XVar.Array(), len = null, rightList = XVar.Array();
			leftList = XVar.Clone(MVCFunctions.explode(new XVar("."), (XVar)(lhs)));
			rightList = XVar.Clone(MVCFunctions.explode(new XVar("."), (XVar)(rhs)));
			len = XVar.Clone(MVCFunctions.min((XVar)(MVCFunctions.count(leftList)), (XVar)(MVCFunctions.count(rightList))));
			i = new XVar(0);
			for(;i < len; i++)
			{
				diff = XVar.Clone(MVCFunctions.intval((XVar)(leftList[i])) - MVCFunctions.intval((XVar)(rightList[i])));
				if(diff != XVar.Pack(0))
				{
					return diff;
				}
			}
			return 0;
		}
		public virtual XVar dbMysqlV8Higher()
		{
			return (XVar)(this.connection.dbType == Constants.nDATABASE_MySQL)  && (XVar)(0 <= this.versionCompare((XVar)(this.connection.getVersion()), new XVar("8.0.0")));
		}
		public virtual XVar getMysqlV8Filter()
		{
			dynamic filterList = XVar.Array(), keyFields = XVar.Array();
			keyFields = XVar.Clone(this.getKeyFields());
			filterList = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> field in keyFields.GetEnumerator())
			{
				dynamic cmpValue = null, fieldType = null;
				fieldType = XVar.Clone(this.getFieldType((XVar)(field.Value)));
				cmpValue = new XVar(null);
				if(XVar.Pack(CommonFunctions.IsNumberType((XVar)(fieldType))))
				{
					cmpValue = new XVar(0);
				}
				else
				{
					if(XVar.Pack(CommonFunctions.IsCharType((XVar)(fieldType))))
					{
						cmpValue = new XVar("");
					}
					else
					{
						if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(fieldType))))
						{
							cmpValue = new XVar("1970-01-01 00:00:00");
						}
					}
				}
				if(!XVar.Equals(XVar.Pack(cmpValue), XVar.Pack(null)))
				{
					filterList.InitAndSetArrayItem(DataCondition.FieldIs((XVar)(field.Value), new XVar(Constants.dsopEMPTY), new XVar(null)), null);
					filterList.InitAndSetArrayItem(DataCondition.FieldIs((XVar)(field.Value), new XVar(Constants.dsopMORE), (XVar)(cmpValue)), null);
					filterList.InitAndSetArrayItem(DataCondition.FieldIs((XVar)(field.Value), new XVar(Constants.dsopEQUAL), (XVar)(cmpValue)), null);
					filterList.InitAndSetArrayItem(DataCondition.FieldIs((XVar)(field.Value), new XVar(Constants.dsopLESS), (XVar)(cmpValue)), null);
					break;
				}
			}
			return DataCondition._Or((XVar)(filterList));
		}
		private XVar clearDsCommandCache(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dc._cache.Remove("overridden");
			dc._cache.Remove("where");
			dc._cache.Remove("sql");

			return null;
		}
		public override XVar getCount(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic sql = null;
			if((XVar)((XVar)(GlobalVars.doMySQLCountBugWorkaround)  && (XVar)(!(XVar)(dc._cache["overriden"])))  && (XVar)(this.dbMysqlV8Higher()))
			{
				dynamic filterCopy = null;
				filterCopy = XVar.Clone(dc.filter);
				dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, dc.filter, 1, this.getMysqlV8Filter()))));
				this.clearDsCommandCache((XVar)(dc));
				sql = XVar.Clone(this.buildSQL((XVar)(dc), new XVar(false)));
				this.clearDsCommandCache((XVar)(dc));
				dc.filter = XVar.Clone(filterCopy);
			}
			else
			{
				sql = XVar.Clone(this.buildSQL((XVar)(dc), new XVar(false)));
			}
			return this.connection.getFetchedRowsNumber((XVar)(sql));
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
		protected virtual XVar getSQLComponents()
		{
			MVCFunctions.trigger_error(new XVar("Unsupported datasource"), new XVar(Constants.E_USER_ERROR));
			return XVar.Array();
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
				dynamic context = null, havingClause = null, sqlParts = XVar.Array(), whereClause = null;
				sqlParts = XVar.Clone(this.getSQLComponents());
				whereClause = XVar.Clone(this.getWhereClause((XVar)(dc)));
				havingClause = XVar.Clone(this.getHavingClause((XVar)(dc)));
				context = XVar.Clone(dc._cache["context"]);
				if(XVar.Pack(context.joins))
				{
					sqlParts["from"] = MVCFunctions.Concat(sqlParts["from"], " ", MVCFunctions.implode(new XVar(" "), (XVar)(context.joins)));
				}
				if(XVar.Pack(replaceFieldList))
				{
					sqlParts.InitAndSetArrayItem(MVCFunctions.Concat("SELECT ", replaceFieldList), "head");
				}
				if(XVar.Pack(dc.extraColumns))
				{
					dynamic extras = XVar.Array();
					extras = XVar.Clone(XVar.Array());
					foreach (KeyValuePair<XVar, dynamic> ec in dc.extraColumns.GetEnumerator())
					{
						extras.InitAndSetArrayItem(this.extraColumnExpression((XVar)(ec.Value)), null);
					}
					sqlParts["head"] = MVCFunctions.Concat(sqlParts["head"], ", ", MVCFunctions.implode(new XVar(", "), (XVar)(extras)));
				}
				if(XVar.Pack(context.extraColumnSQL))
				{
					sqlParts["head"] = MVCFunctions.Concat(sqlParts["head"], ", ", MVCFunctions.implode(new XVar(", "), (XVar)(context.extraColumnSQL)));
				}
				dc._cache.InitAndSetArrayItem(SQLQuery.buildSQL((XVar)(sqlParts), (XVar)(new XVar(0, whereClause)), (XVar)(new XVar(0, havingClause))), "sql");
			}
			sql = XVar.Clone(dc._cache["sql"]);
			if(XVar.Pack(addOrder))
			{
				sql = MVCFunctions.Concat(sql, this.getOrderClause((XVar)(dc)));
			}
			return sql;
		}
		protected virtual XVar extraColumnExpression(dynamic _param_column)
		{
			#region pass-by-value parameters
			dynamic column = XVar.Clone(_param_column);
			#endregion

			dynamic expression = null;
			if(XVar.Pack(column.sql))
			{
				expression = XVar.Clone(column.sql);
			}
			else
			{
				dynamic columnExpression = null;
				columnExpression = XVar.Clone(this.fieldExpression((XVar)(column.field), (XVar)(column.modifier)));
				if(XVar.Pack(column.joinData))
				{
					expression = XVar.Clone(this.getJoinedSubquery((XVar)(column.joinData), (XVar)(columnExpression), (XVar)(this.getFieldType((XVar)(column.field))), new XVar(null), new XVar(""), new XVar(true)));
				}
				if(expression != XVar.Pack(""))
				{
					expression = XVar.Clone(MVCFunctions.Concat("( ", expression, " )"));
				}
				else
				{
					expression = XVar.Clone(columnExpression);
				}
			}
			if(XVar.Pack(column.alias))
			{
				return MVCFunctions.Concat(expression, " AS ", this.connection.addFieldWrappers((XVar)(column.alias)));
			}
			else
			{
				return expression;
			}

			return null;
		}
		protected virtual XVar fieldExpression(dynamic _param_field, dynamic _param_modifier = null)
		{
			#region default values
			if(_param_modifier as Object == null) _param_modifier = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic modifier = XVar.Clone(_param_modifier);
			#endregion

			dynamic fieldExpr = null;
			fieldExpr = XVar.Clone(this.connection.addFieldWrappers((XVar)(field)));
			return this.applyFieldModifier((XVar)(fieldExpr), (XVar)(this.getFieldType((XVar)(field))), (XVar)(modifier));
		}
		public override XVar getTotalCount(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic data = XVar.Array(), result = null, sql = null;
			sql = XVar.Clone(MVCFunctions.Concat("SELECT COUNT(*) from ( ", this.getTotalsSQL((XVar)(dc), new XVar(true)), " ) a"));
			result = XVar.Clone(this.connection.query((XVar)(sql)));
			if(XVar.Pack(!(XVar)(result)))
			{
				return 0;
			}
			data = XVar.Clone(result.fetchNumeric());
			if(XVar.Pack(!(XVar)(data)))
			{
				return 0;
			}
			return data[0];
		}
		protected virtual XVar getTotalsSQL(dynamic _param_dc, dynamic _param_addOrder)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic addOrder = XVar.Clone(_param_addOrder);
			#endregion

			dynamic baseSQL = null, distinct = null, groupList = XVar.Array(), groupbyClause = null, orderList = XVar.Array(), selectList = XVar.Array(), sql = null, whereClause = null, whereList = XVar.Array();
			baseSQL = XVar.Clone(this.buildSQL((XVar)(dc), new XVar(false)));
			selectList = XVar.Clone(XVar.Array());
			groupList = XVar.Clone(XVar.Array());
			whereList = XVar.Clone(XVar.Array());
			orderList = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> t in dc.totals.GetEnumerator())
			{
				dynamic aliasClause = null, f = null;
				f = XVar.Clone(this.sqlTotalField((XVar)(dc), (XVar)(t.Value)));
				if(XVar.Equals(XVar.Pack(f), XVar.Pack(false)))
				{
					return false;
				}
				aliasClause = XVar.Clone((XVar.Pack(t.Value["alias"]) ? XVar.Pack(MVCFunctions.Concat(" AS ", this.connection.addFieldWrappers((XVar)(t.Value["alias"])))) : XVar.Pack("")));
				selectList.InitAndSetArrayItem(MVCFunctions.Concat(f, aliasClause), null);
				if(XVar.Pack(t.Value["direction"]))
				{
					orderList.InitAndSetArrayItem(MVCFunctions.Concat(t.Key + 1, (XVar.Pack(t.Value["direction"] == "ASC") ? XVar.Pack(" ASC") : XVar.Pack(" DESC"))), null);
				}
				if(XVar.Pack(!(XVar)(t.Value["total"])))
				{
					groupList.InitAndSetArrayItem(f, null);
				}
				if(XVar.Pack(t.Value["skipEmpty"]))
				{
					whereList.InitAndSetArrayItem(MVCFunctions.Concat("( NOT ( ", this.sqlExpressionEmpty((XVar)(this.getFieldType((XVar)(t.Value["field"]))), (XVar)(this.connection.addFieldWrappers((XVar)(t.Value["field"])))), " ) )"), null);
				}
			}
			groupbyClause = XVar.Clone(MVCFunctions.implode(new XVar(","), (XVar)(groupList)));
			if(XVar.Pack(MVCFunctions.count(groupList)))
			{
				groupbyClause = XVar.Clone(MVCFunctions.Concat(" GROUP BY ", groupbyClause));
			}
			whereClause = XVar.Clone(MVCFunctions.implode(new XVar(" AND "), (XVar)(whereList)));
			if(XVar.Pack(MVCFunctions.count(whereList)))
			{
				whereClause = XVar.Clone(MVCFunctions.Concat(" WHERE ", whereClause));
			}
			distinct = XVar.Clone((XVar.Pack(this.totalDistinct((XVar)(dc))) ? XVar.Pack("DISTINCT ") : XVar.Pack("")));
			sql = XVar.Clone(MVCFunctions.Concat("SELECT ", distinct, MVCFunctions.implode(new XVar(","), (XVar)(selectList)), " FROM ( ", baseSQL, " ) a ", whereClause, groupbyClause));
			if(XVar.Pack(addOrder))
			{
				dynamic order = null;
				order = XVar.Clone(MVCFunctions.implode(new XVar(", "), (XVar)(orderList)));
				if(XVar.Pack(order))
				{
					sql = MVCFunctions.Concat(sql, " ORDER BY ", order);
				}
			}
			return sql;
		}
		public override XVar getTotals(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic sql = null;
			sql = XVar.Clone(this.getTotalsSQL((XVar)(dc), new XVar(true)));
			if(XVar.Pack(!(XVar)(sql)))
			{
				return new ArrayResult((XVar)(XVar.Array()));
			}
			return this.connection.limitedQuery((XVar)(sql), (XVar)(dc.startRecord), (XVar)(dc.reccount), new XVar(true));
		}
		protected virtual XVar isAggregateField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return false;
		}
		protected virtual XVar totalDistinct(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> total in dc.totals.GetEnumerator())
			{
				dynamic fType = null;
				if(total.Value["total"] != "distinct")
				{
					return false;
				}
				fType = XVar.Clone(this.getFieldType((XVar)(total.Value["field"])));
				if((XVar)(CommonFunctions.IsTextType((XVar)(fType)))  && (XVar)((XVar)((XVar)(this.connection.dbType == Constants.nDATABASE_MSSQLServer)  || (XVar)(this.connection.dbType == Constants.nDATABASE_Access))  || (XVar)(this.connection.dbType == Constants.nDATABASE_Oracle)))
				{
					return false;
				}
				if((XVar)((XVar)(dc.skipAggregated)  && (XVar)(total.Value["total"] == "distinct"))  && (XVar)(this.isAggregateField((XVar)(total.Value["field"]))))
				{
					return false;
				}
			}
			return true;
		}
		protected virtual XVar sqlTotalField(dynamic _param_dc, dynamic _param_total)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic total = XVar.Clone(_param_total);
			#endregion

			dynamic expr = null, ftype = null;
			if((XVar)((XVar)(dc.skipAggregated)  && (XVar)(total["total"] == "distinct"))  && (XVar)(this.isAggregateField((XVar)(total["field"]))))
			{
				return false;
			}
			if(XVar.Pack(total["timeToSec"]))
			{
				expr = XVar.Clone(this.connection.timeToSecWrapper((XVar)(total["field"])));
			}
			else
			{
				expr = XVar.Clone(this.connection.addFieldWrappers((XVar)(total["field"])));
			}
			if(total["total"] == "distinct")
			{
				dynamic context = null;
				context = XVar.Clone(dc._cache["context"]);
				if(XVar.Pack(context))
				{
					if(XVar.Pack(context.joinedAliases.KeyExists(total["field"])))
					{
						expr = XVar.Clone(this.connection.addFieldWrappers((XVar)(context.joinedAliases[total["field"]])));
					}
				}
				return expr;
			}
			if(XVar.Pack(total["total"]))
			{
				if((XVar)(total["total"] == "count")  && (XVar)(!(XVar)(total["field"])))
				{
					expr = new XVar("1");
				}
				if(XVar.Pack(total["caseStatement"]))
				{
					expr = XVar.Clone(this.getCaseStatement((XVar)(total["caseStatement"])));
				}
				return MVCFunctions.Concat(MVCFunctions.strtoupper((XVar)(total["total"])), "( ", expr, " )");
			}
			ftype = XVar.Clone(this.getFieldType((XVar)(total["field"])));
			return this.applyFieldModifier((XVar)(expr), (XVar)(ftype), (XVar)(total["modifier"]));
		}
		protected virtual XVar totalOperandSQL(dynamic _param_op)
		{
			#region pass-by-value parameters
			dynamic op = XVar.Clone(_param_op);
			#endregion

			if(XVar.Pack(op))
			{
				if(op.var_type == Constants.dsotFIELD)
				{
					return this.connection.addFieldWrappers((XVar)(op.value));
				}
				else
				{
					if(op.var_type == Constants.dsotCONST)
					{
						return op.value;
					}
				}
			}
			return "NULL";
		}
		protected virtual XVar getCaseStatement(dynamic _param_caseExpr)
		{
			#region pass-by-value parameters
			dynamic caseExpr = XVar.Clone(_param_caseExpr);
			#endregion

			dynamic context = null, sqlCases = XVar.Array(), sqlDefault = null, tail = null;
			sqlCases = XVar.Clone(XVar.Array());
			tail = new XVar("");
			context = XVar.Clone(new DsFilterBuildContext());
			context.useSubquery = new XVar(true);
			foreach (KeyValuePair<XVar, dynamic> condition in caseExpr.conditions.GetEnumerator())
			{
				dynamic sqlCondition = null, sqlValue = null;
				sqlCondition = XVar.Clone(this.conditionToSQL((XVar)(condition.Value), (XVar)(context)));
				sqlValue = XVar.Clone(this.totalOperandSQL((XVar)(caseExpr.values[condition.Key])));
				if(this.connection.dbType == Constants.nDATABASE_Access)
				{
					sqlCases.InitAndSetArrayItem(MVCFunctions.Concat("IIF( ", sqlCondition, ", ", sqlValue, ","), null);
					tail = MVCFunctions.Concat(tail, ")");
				}
				else
				{
					sqlCases.InitAndSetArrayItem(MVCFunctions.Concat("WHEN ", sqlCondition, " THEN ", sqlValue), null);
				}
			}
			sqlDefault = XVar.Clone(this.totalOperandSQL((XVar)(caseExpr.defValue)));
			if(this.connection.dbType == Constants.nDATABASE_Access)
			{
				return MVCFunctions.Concat(MVCFunctions.implode(new XVar(" "), (XVar)(sqlCases)), " ", sqlDefault, tail);
			}
			else
			{
				return MVCFunctions.Concat("( CASE ", MVCFunctions.implode(new XVar(" "), (XVar)(sqlCases)), " ELSE ", sqlDefault, " END )");
			}

			return null;
		}
		protected virtual XVar applyFieldModifier(dynamic _param_expr, dynamic _param_ftype, dynamic _param_modifier)
		{
			#region pass-by-value parameters
			dynamic expr = XVar.Clone(_param_expr);
			dynamic ftype = XVar.Clone(_param_ftype);
			dynamic modifier = XVar.Clone(_param_modifier);
			#endregion

			if(XVar.Pack(CommonFunctions.IsNumberType((XVar)(ftype))))
			{
				expr = XVar.Clone(this.connection.intervalExpressionNumber((XVar)(expr), (XVar)(modifier)));
			}
			else
			{
				if(XVar.Pack(CommonFunctions.IsCharType((XVar)(ftype))))
				{
					expr = XVar.Clone(this.connection.intervalExpressionString((XVar)(expr), (XVar)(modifier)));
				}
				else
				{
					if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(ftype))))
					{
						expr = XVar.Clone(this.connection.intervalExpressionDate((XVar)(expr), (XVar)(modifier)));
					}
				}
			}
			return expr;
		}
		public virtual XVar delete(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion


			return null;
		}
		protected override XVar getSingleImpl(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic sql = null;
			sql = XVar.Clone(this.buildSQL((XVar)(dc), new XVar(true)));
			return this.connection.limitedQuery((XVar)(sql), new XVar(0), new XVar(1), new XVar(true));
		}
		protected virtual XVar dbTableName()
		{
			return this._name;
		}
		public override XVar getNextPrevKeys(dynamic _param_dc, dynamic data, dynamic _param_what = null)
		{
			#region default values
			if(_param_what as Object == null) _param_what = new XVar(Constants.BOTH_RECORDS);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic what = XVar.Clone(_param_what);
			#endregion

			dynamic baseSQL = null, keysFields = XVar.Array(), nextPrevComponents = XVar.Array(), prev = null, rs = null, sql = null, strSQL = null, var_next = null;
			nextPrevComponents = XVar.Clone(this.getNextPrevComponents((XVar)(dc), (XVar)(data)));
			if(XVar.Pack(!(XVar)(nextPrevComponents)))
			{
				return XVar.Array();
			}
			baseSQL = XVar.Clone(this.buildSQL((XVar)(dc), new XVar(false)));
			keysFields = XVar.Clone(XVar.Array());
			var_next = XVar.Clone(XVar.Array());
			prev = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> k in this.getKeyFields().GetEnumerator())
			{
				keysFields.InitAndSetArrayItem(this.connection.addFieldWrappers((XVar)(k.Value)), null);
			}
			sql = XVar.Clone(MVCFunctions.Concat("select ", MVCFunctions.implode(new XVar(","), (XVar)(keysFields)), " from ( ", baseSQL, " ) a "));
			if((XVar)(what == Constants.BOTH_RECORDS)  || (XVar)(what == Constants.NEXT_RECORD))
			{
				strSQL = XVar.Clone(sql);
				if(XVar.Pack(MVCFunctions.strlen((XVar)(nextPrevComponents["nextWhere"]))))
				{
					strSQL = MVCFunctions.Concat(strSQL, "where ", nextPrevComponents["nextWhere"]);
				}
				if(XVar.Pack(MVCFunctions.strlen((XVar)(nextPrevComponents["nextOrder"]))))
				{
					strSQL = MVCFunctions.Concat(strSQL, "order by ", nextPrevComponents["nextOrder"]);
				}
				rs = XVar.Clone(this.connection.limitedQuery((XVar)(strSQL), new XVar(0), new XVar(1), new XVar(true)));
				if(XVar.Pack(rs))
				{
					var_next = XVar.Clone(rs.fetchNumeric());
				}
			}
			if((XVar)(what == Constants.BOTH_RECORDS)  || (XVar)(what == Constants.PREV_RECORD))
			{
				strSQL = XVar.Clone(sql);
				if(XVar.Pack(MVCFunctions.strlen((XVar)(nextPrevComponents["prevWhere"]))))
				{
					strSQL = MVCFunctions.Concat(strSQL, "where ", nextPrevComponents["prevWhere"]);
				}
				if(XVar.Pack(MVCFunctions.strlen((XVar)(nextPrevComponents["prevOrder"]))))
				{
					strSQL = MVCFunctions.Concat(strSQL, "order by ", nextPrevComponents["prevOrder"]);
				}
				rs = XVar.Clone(this.connection.limitedQuery((XVar)(strSQL), new XVar(0), new XVar(1), new XVar(true)));
				if(XVar.Pack(rs))
				{
					prev = XVar.Clone(rs.fetchNumeric());
				}
			}
			return new XVar("next", var_next, "prev", prev);
		}
		protected virtual XVar getNextPrevComponents(dynamic _param_dc, dynamic data)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic column = null, context = null, fieldIsNull = null, i = null, nextOrder = XVar.Array(), nextWhere = null, of = XVar.Array(), prev = null, prevOrder = XVar.Array(), prevWhere = null, sqlColumn = null, var_next = null;
			if(XVar.Pack(!(XVar)(dc.order)))
			{
				return XVar.Array();
			}
			nextWhere = new XVar(null);
			prevWhere = new XVar(null);
			nextOrder = XVar.Clone(XVar.Array());
			prevOrder = XVar.Clone(XVar.Array());
			i = XVar.Clone(MVCFunctions.count(dc.order) - 1);
			for(;XVar.Pack(0) <= i; --(i))
			{
				of = XVar.Clone(dc.order[i]);
				column = XVar.Clone(of["column"]);
				sqlColumn = XVar.Clone(this.connection.addFieldWrappers((XVar)(column)));
				nextOrder.InitAndSetArrayItem(MVCFunctions.Concat(sqlColumn, " ", of["dir"]), null);
				prevOrder.InitAndSetArrayItem(MVCFunctions.Concat(sqlColumn, " ", (XVar.Pack(of["dir"] == "ASC") ? XVar.Pack("DESC") : XVar.Pack("ASC"))), null);
				fieldIsNull = XVar.Clone(XVar.Equals(XVar.Pack(data[column]), XVar.Pack(null)));
				if(i < MVCFunctions.count(dc.order) - 1)
				{
					dynamic equal = null;
					equal = XVar.Clone((XVar.Pack(!(XVar)(fieldIsNull)) ? XVar.Pack(DataCondition.FieldEquals((XVar)(column), (XVar)(data[column]))) : XVar.Pack(DataCondition.FieldIsNull((XVar)(column)))));
					nextWhere = XVar.Clone((XVar.Pack(nextWhere) ? XVar.Pack(DataCondition._And((XVar)(new XVar(0, equal, 1, nextWhere)))) : XVar.Pack(equal)));
					prevWhere = XVar.Clone((XVar.Pack(prevWhere) ? XVar.Pack(DataCondition._And((XVar)(new XVar(0, equal, 1, prevWhere)))) : XVar.Pack(equal)));
				}
				if(XVar.Pack(!(XVar)(fieldIsNull)))
				{
					dynamic lessExpression = null, moreExpression = null;
					moreExpression = XVar.Clone(DataCondition.FieldIs((XVar)(column), new XVar(Constants.dsopMORE), (XVar)(data[column])));
					lessExpression = XVar.Clone(DataCondition._Or((XVar)(new XVar(0, DataCondition.FieldIs((XVar)(column), new XVar(Constants.dsopLESS), (XVar)(data[column])), 1, DataCondition.FieldIsNull((XVar)(column))))));
					var_next = XVar.Clone((XVar.Pack(of["dir"] == "DESC") ? XVar.Pack(lessExpression) : XVar.Pack(moreExpression)));
					prev = XVar.Clone((XVar.Pack(of["dir"] == "DESC") ? XVar.Pack(moreExpression) : XVar.Pack(lessExpression)));
				}
				else
				{
					var_next = XVar.Clone((XVar.Pack(of["dir"] == "DESC") ? XVar.Pack(null) : XVar.Pack(DataCondition._Not((XVar)(DataCondition.FieldIsNull((XVar)(column)))))));
					prev = XVar.Clone((XVar.Pack(of["dir"] == "DESC") ? XVar.Pack(DataCondition._Not((XVar)(DataCondition.FieldIsNull((XVar)(column))))) : XVar.Pack(null)));
				}
				if(XVar.Pack(var_next))
				{
					nextWhere = XVar.Clone(DataCondition._Or((XVar)(new XVar(0, var_next, 1, nextWhere))));
				}
				if(XVar.Pack(prev))
				{
					prevWhere = XVar.Clone(DataCondition._Or((XVar)(new XVar(0, prev, 1, prevWhere))));
				}
			}
			context = XVar.Clone(new DsFilterBuildContext());
			context.useSubquery = new XVar(true);
			return new XVar("nextWhere", this.conditionToSQL((XVar)(nextWhere), (XVar)(context)), "prevWhere", this.conditionToSQL((XVar)(prevWhere), (XVar)(context)), "nextOrder", MVCFunctions.implode(new XVar(", "), (XVar)(MVCFunctions.array_reverse((XVar)(nextOrder)))), "prevOrder", MVCFunctions.implode(new XVar(", "), (XVar)(MVCFunctions.array_reverse((XVar)(prevOrder)))));
		}
		protected virtual XVar createJoinedExpression(dynamic _param_field, dynamic _param_fieldExpr, dynamic _param_fieldOperand, dynamic _param_context)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic fieldExpr = XVar.Clone(_param_fieldExpr);
			dynamic fieldOperand = XVar.Clone(_param_fieldOperand);
			dynamic context = XVar.Clone(_param_context);
			#endregion

			dynamic jd = null, joinQuery = XVar.Array();
			jd = XVar.Clone(fieldOperand.joinData);
			if(XVar.Pack(!(XVar)(this.acceptJoinData((XVar)(jd)))))
			{
				return false;
			}
			joinQuery = XVar.Clone(jd.dataSource.getJoinedQuery((XVar)(fieldExpr), (XVar)(fieldOperand), (XVar)(this.getFieldType((XVar)(field)))));
			if(XVar.Pack(!(XVar)(joinQuery)))
			{
				return false;
			}
			context.joins.InitAndSetArrayItem(joinQuery["join"], null);
			if(XVar.Pack(jd.displayAlias))
			{
				context.extraColumnSQL.InitAndSetArrayItem(MVCFunctions.Concat(joinQuery["column"], " AS ", this.connection.addFieldWrappers((XVar)(jd.displayAlias))), null);
				context.joinedAliases.InitAndSetArrayItem(jd.displayAlias, field);
			}
			return joinQuery["column"];
		}
		protected virtual XVar acceptJoinData(dynamic _param_jd)
		{
			#region pass-by-value parameters
			dynamic jd = XVar.Clone(_param_jd);
			#endregion

			if((XVar)(!(XVar)(jd.dataSource.tableBased()))  || (XVar)(!XVar.Equals(XVar.Pack(jd.dataSource.getConnectionId()), XVar.Pack(this.getConnectionId()))))
			{
				return false;
			}
			if(this.connection.dbType == Constants.nDATABASE_Access)
			{
				return false;
			}
			if((XVar)(jd.longList)  && (XVar)(!(XVar)(this.connection.checkIfJoinSubqueriesOptimized())))
			{
				return false;
			}
			return true;
		}
		protected virtual XVar getJoinedSubquery(dynamic _param_jd, dynamic _param_mainFieldExpr, dynamic _param_mainFieldType, dynamic _param_condition = null, dynamic _param_tableAlias = null, dynamic _param_oneRow = null)
		{
			#region default values
			if(_param_condition as Object == null) _param_condition = new XVar();
			if(_param_tableAlias as Object == null) _param_tableAlias = new XVar("");
			if(_param_oneRow as Object == null) _param_oneRow = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic jd = XVar.Clone(_param_jd);
			dynamic mainFieldExpr = XVar.Clone(_param_mainFieldExpr);
			dynamic mainFieldType = XVar.Clone(_param_mainFieldType);
			dynamic condition = XVar.Clone(_param_condition);
			dynamic tableAlias = XVar.Clone(_param_tableAlias);
			dynamic oneRow = XVar.Clone(_param_oneRow);
			#endregion

			dynamic displayColumn = null, fullCondition = null, innerDc = null, innerSQL = null, linkAlias = null, linkFieldExpr = null, linkFieldType = null, returnExpression = null, secondSQL = null, sqlCondition = null, thirdSQL = null;
			if(XVar.Pack(!(XVar)(this.acceptJoinData((XVar)(jd)))))
			{
				return "";
			}
			innerDc = XVar.Clone(new DsCommand());
			if(XVar.Pack(jd.displayExpression))
			{
				displayColumn = XVar.Clone(CommonFunctions.generateAlias());
				innerDc.extraColumns.InitAndSetArrayItem(new DsFieldData((XVar)(jd.displayExpression), (XVar)(displayColumn), new XVar("")), null);
			}
			else
			{
				displayColumn = XVar.Clone(jd.displayField);
			}
			innerSQL = XVar.Clone(jd.dataSource.buildSQL((XVar)(innerDc), new XVar(false)));
			linkAlias = XVar.Clone(CommonFunctions.generateAlias());
			secondSQL = XVar.Clone(MVCFunctions.Concat("SELECT ", this.wrap((XVar)(jd.linkField)), " AS ", this.wrap((XVar)(linkAlias)), ", ", this.wrap((XVar)(displayColumn)), " AS ", this.wrap((XVar)(jd.displayAlias)), " FROM ( ", innerSQL, " ) a "));
			if(linkFieldExpr == XVar.Pack(""))
			{
				linkFieldExpr = XVar.Clone(this.fieldExpression((XVar)(jd.linkField)));
			}
			linkFieldType = XVar.Clone(jd.dataSource.getFieldType((XVar)(jd.linkField)));
			linkFieldExpr = XVar.Clone(this.wrap((XVar)(linkAlias)));
			if(CommonFunctions.IsCharType((XVar)(linkFieldType)) != CommonFunctions.IsCharType((XVar)(mainFieldType)))
			{
				if(XVar.Pack(CommonFunctions.IsCharType((XVar)(linkFieldType))))
				{
					mainFieldExpr = XVar.Clone(this.connection.field2char((XVar)(mainFieldExpr)));
				}
				else
				{
					linkFieldExpr = XVar.Clone(this.connection.field2char((XVar)(linkFieldExpr)));
				}
			}
			fullCondition = XVar.Clone(DataCondition._And((XVar)(new XVar(0, condition, 1, DataCondition.SQLCondition((XVar)(MVCFunctions.Concat(linkFieldExpr, " = ", mainFieldExpr)))))));
			sqlCondition = XVar.Clone(jd.dataSource.conditionToSQL((XVar)(fullCondition), (XVar)(new DsFilterBuildContext())));
			tableAlias = XVar.Clone((XVar.Pack(tableAlias == XVar.Pack("")) ? XVar.Pack(CommonFunctions.generateAlias()) : XVar.Pack(tableAlias)));
			returnExpression = XVar.Clone(this.wrap((XVar)(jd.displayAlias)));
			if(XVar.Pack(oneRow))
			{
				returnExpression = XVar.Clone(MVCFunctions.Concat("MIN( ", returnExpression, " )"));
			}
			thirdSQL = XVar.Clone(MVCFunctions.Concat("SELECT ", returnExpression, " FROM ", "( ", secondSQL, " ) ", this.wrap((XVar)(tableAlias)), " WHERE ", sqlCondition));
			return thirdSQL;
		}
		protected virtual XVar createJoinedSearchClause(dynamic _param_field, dynamic _param_fieldExpr, dynamic _param_dCondition)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic fieldExpr = XVar.Clone(_param_fieldExpr);
			dynamic dCondition = XVar.Clone(_param_dCondition);
			#endregion

			dynamic displayOperand = null, fieldOperand = null, jCondition = null, jd = null, subQuery = null, tableAlias = null, valueOperand = null;
			fieldOperand = XVar.Clone(dCondition.operands[0]);
			valueOperand = XVar.Clone(dCondition.operands[1]);
			jd = XVar.Clone(fieldOperand.joinData);
			if(XVar.Pack(!(XVar)(this.acceptJoinData((XVar)(jd)))))
			{
				return "";
			}
			tableAlias = XVar.Clone(CommonFunctions.generateAlias());
			displayOperand = XVar.Clone(new DsOperand(new XVar(Constants.dsotSQL), (XVar)(MVCFunctions.Concat(this.wrap((XVar)(tableAlias)), ".", this.wrap((XVar)(jd.displayAlias)))), new XVar(0), new XVar(null), new XVar(null), new XVar(true)));
			jCondition = XVar.Clone(new DsCondition((XVar)(new XVar(0, displayOperand, 1, valueOperand)), (XVar)(dCondition.operation), (XVar)(dCondition.caseInsensitive)));
			subQuery = XVar.Clone(this.getJoinedSubquery((XVar)(jd), (XVar)(fieldExpr), (XVar)(this.getFieldType((XVar)(field))), (XVar)(jCondition), (XVar)(tableAlias)));
			return MVCFunctions.Concat("EXISTS( ", subQuery, " )");
		}
		public override XVar tableBased()
		{
			return true;
		}
		protected virtual XVar getJoinedQuery(dynamic _param_fieldExpr, dynamic _param_fieldOperand, dynamic _param_fieldType)
		{
			#region pass-by-value parameters
			dynamic fieldExpr = XVar.Clone(_param_fieldExpr);
			dynamic fieldOperand = XVar.Clone(_param_fieldOperand);
			dynamic fieldType = XVar.Clone(_param_fieldType);
			#endregion

			dynamic column = null, dc = null, dispFieldTotal = XVar.Array(), displayAlias = null, jd = null, join = null, joinLinkExpr = null, linkAlias = null, sql = null, tableAlias = null;
			jd = XVar.Clone(fieldOperand.joinData);
			dc = XVar.Clone(new DsCommand());
			tableAlias = XVar.Clone(CommonFunctions.generateAlias());
			linkAlias = XVar.Clone(CommonFunctions.generateAlias());
			displayAlias = XVar.Clone(CommonFunctions.generateAlias());
			dc.totals.InitAndSetArrayItem(new XVar("alias", linkAlias, "field", jd.linkField, "caseInsensitive", Constants.dsCASE_DEFAULT), null);
			dispFieldTotal = XVar.Clone(new XVar("alias", displayAlias, "total", "min"));
			if(XVar.Pack(jd.displayExpression))
			{
				dispFieldTotal.InitAndSetArrayItem(CommonFunctions.generateAlias(), "field");
				dc.extraColumns.InitAndSetArrayItem(new DsFieldData((XVar)(jd.displayExpression), (XVar)(dispFieldTotal["field"]), new XVar("")), null);
			}
			else
			{
				dispFieldTotal.InitAndSetArrayItem(jd.displayField, "field");
			}
			dc.totals.InitAndSetArrayItem(dispFieldTotal, null);
			sql = XVar.Clone(this.getTotalsSQL((XVar)(dc), new XVar(false)));
			column = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers((XVar)(tableAlias)), ".", this.connection.addFieldWrappers((XVar)(displayAlias))));
			joinLinkExpr = XVar.Clone(this.connection.addFieldWrappers((XVar)(linkAlias)));
			if(!XVar.Equals(XVar.Pack(CommonFunctions.IsCharType((XVar)(fieldType))), XVar.Pack(CommonFunctions.IsCharType((XVar)(this.getFieldType((XVar)(jd.linkField)))))))
			{
				if(XVar.Pack(CommonFunctions.IsCharType((XVar)(fieldType))))
				{
					joinLinkExpr = XVar.Clone(this.connection.field2char((XVar)(joinLinkExpr)));
				}
				else
				{
					fieldExpr = XVar.Clone(this.connection.field2char((XVar)(fieldExpr)));
				}
			}
			join = XVar.Clone(MVCFunctions.Concat("LEFT JOIN (", sql, ") ", this.connection.addFieldWrappers((XVar)(tableAlias)), " ON ", joinLinkExpr, " = ", fieldExpr));
			return new XVar("join", join, "column", column);
		}
		public override XVar lastError()
		{
			return this.connection.lastError();
		}
		public virtual XVar getFieldValues(dynamic _param_field, dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic sql = null;
			sql = XVar.Clone(this.buildSQL((XVar)(dc), new XVar(false), (XVar)(this.extraColumnExpression((XVar)(field)))));
			return this.connection.limitedQuery((XVar)(sql), (XVar)(dc.startRecord), (XVar)(dc.reccount), new XVar(true));
		}
		protected virtual XVar getColumnCount()
		{
			return 0;
		}
		protected virtual XVar encryptField(dynamic _param_field, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return value;
		}
		public override XVar wrap(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return this.connection.addFieldWrappers((XVar)(str));
		}
		public override XVar updateRowNumberAvailable(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			return (XVar)((XVar)((XVar)((XVar)(this.connection.dbType == Constants.nDATABASE_MSSQLServer)  && (XVar)(dc.order))  || (XVar)((XVar)(GlobalVars.projectLanguage == "php")  && (XVar)(this.connection.dbType == Constants.nDATABASE_MySQL)))  || (XVar)(this.connection.dbType == Constants.nDATABASE_PostgreSQL))  || (XVar)((XVar)(this.connection.dbType == Constants.nDATABASE_Oracle)  && (XVar)(dc.order));
		}
		public override XVar updateRowNumber(dynamic _param_dc, dynamic _param_startNumber = null)
		{
			#region default values
			if(_param_startNumber as Object == null) _param_startNumber = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic startNumber = XVar.Clone(_param_startNumber);
			#endregion

			dynamic keys = XVar.Array(), orderByClause = null, orderField = null, orderWhere = null, overClause = null, queryAlias = null, rownoAlias = null, sql = null, table = null;
			orderByClause = XVar.Clone(this.getOrderClause((XVar)(dc), new XVar(true)));
			queryAlias = XVar.Clone(this.wrap((XVar)(CommonFunctions.generateAlias())));
			rownoAlias = XVar.Clone(this.wrap((XVar)(CommonFunctions.generateAlias())));
			table = XVar.Clone(this.connection.addTableWrappers((XVar)(this.dbTableName())));
			keys = XVar.Clone(MVCFunctions.array_keys((XVar)(dc.advValues)));
			orderField = XVar.Clone(this.wrap((XVar)(keys[0])));
			orderWhere = XVar.Clone(this.getWhereClause((XVar)(dc)));
			orderWhere = XVar.Clone((XVar.Pack(orderWhere) ? XVar.Pack(MVCFunctions.Concat(" WHERE ", orderWhere)) : XVar.Pack("")));
			if(this.connection.dbType == Constants.nDATABASE_MSSQLServer)
			{
				overClause = XVar.Clone(MVCFunctions.Concat("OVER( ", orderByClause, ")"));
				sql = XVar.Clone(MVCFunctions.Concat("UPDATE ", queryAlias, " SET ", queryAlias, ".", orderField, " = ", queryAlias, ".", rownoAlias, "+", startNumber, " FROM ( SELECT ", orderField, ", ROW_NUMBER() ", overClause, " AS ", rownoAlias, " FROM ", table, orderWhere, " ) ", queryAlias));
				this.connection.exec((XVar)(sql));
			}
			else
			{
				if(this.connection.dbType == Constants.nDATABASE_MySQL)
				{
					dynamic sqls = XVar.Array();
					sqls.InitAndSetArrayItem(MVCFunctions.Concat("SET @rowno:=", startNumber, ";"), null);
					sqls.InitAndSetArrayItem(MVCFunctions.Concat("UPDATE ", table, " SET ", orderField, " = @rowno := @rowno + 1 ", orderWhere, " ", orderByClause, ";"), null);
					this.connection.execMultiple((XVar)(sqls));
				}
				else
				{
					dynamic keyFieldsWhere = XVar.Array(), keyName = null, keysFields = XVar.Array();
					if(this.connection.dbType == Constants.nDATABASE_Oracle)
					{
						overClause = XVar.Clone(MVCFunctions.Concat("OVER( ", orderByClause, ")"));
						keyFieldsWhere = XVar.Clone(XVar.Array());
						foreach (KeyValuePair<XVar, dynamic> k in this.getKeyFields().GetEnumerator())
						{
							keyName = XVar.Clone(this.wrap((XVar)(k.Value)));
							keysFields.InitAndSetArrayItem(keyName, null);
							keyFieldsWhere.InitAndSetArrayItem(MVCFunctions.Concat(queryAlias, ".", keyName, "=", table, ".", keyName), null);
						}
						sql = XVar.Clone(MVCFunctions.Concat("UPDATE ", table, " SET ", orderField, "=( SELECT ", rownoAlias, " + ", startNumber, " FROM ( select ", MVCFunctions.implode(new XVar(", "), (XVar)(keysFields)), ", ROW_NUMBER() ", overClause, " AS ", rownoAlias, " FROM ", table, " ", orderWhere, " ) ", queryAlias, " WHERE ", MVCFunctions.implode(new XVar("AND "), (XVar)(keyFieldsWhere)), ") ", orderWhere));
						this.connection.exec((XVar)(sql));
					}
					else
					{
						if(this.connection.dbType == Constants.nDATABASE_PostgreSQL)
						{
							overClause = XVar.Clone((XVar.Pack(orderByClause) ? XVar.Pack(MVCFunctions.Concat("OVER( ", orderByClause, ")")) : XVar.Pack("OVER()")));
							keyFieldsWhere = XVar.Clone(XVar.Array());
							foreach (KeyValuePair<XVar, dynamic> k in this.getKeyFields().GetEnumerator())
							{
								keyName = XVar.Clone(this.wrap((XVar)(k.Value)));
								keysFields.InitAndSetArrayItem(keyName, null);
								keyFieldsWhere.InitAndSetArrayItem(MVCFunctions.Concat(queryAlias, ".", keyName, "=", table, ".", keyName), null);
							}
							sql = XVar.Clone(MVCFunctions.Concat("UPDATE ", table, " SET ", orderField, " = ", queryAlias, ".", rownoAlias, "+", startNumber, " FROM ( SELECT ", MVCFunctions.implode(new XVar(", "), (XVar)(keysFields)), ",", orderField, ", ROW_NUMBER() ", overClause, " AS ", rownoAlias, " FROM ", table, orderWhere, " ) ", queryAlias, " WHERE ", MVCFunctions.implode(new XVar("AND "), (XVar)(keyFieldsWhere))));
							this.connection.exec((XVar)(sql));
						}
					}
				}
			}

			return null;
		}
		protected virtual XVar prepareInsertValues(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic blobTypes = null, blobs = null, sqlValues = XVar.Array(), values = null;
			values = XVar.Clone(dc.values);
			sqlValues = XVar.Clone(XVar.Array());
			blobs = XVar.Clone(XVar.Array());
			blobTypes = XVar.Clone(XVar.Array());
			this.makeAdvancedValues((XVar)(dc));
			foreach (KeyValuePair<XVar, dynamic> valueOp in dc.advValues.GetEnumerator())
			{
				dynamic sqlField = null;
				sqlField = XVar.Clone(this.getUpdateFieldSQL((XVar)(valueOp.Key)));
				if(XVar.Equals(XVar.Pack(valueOp.Value.var_type), XVar.Pack(Constants.dsotCONST)))
				{
					if((XVar)(XVar.Equals(XVar.Pack(valueOp.Value.value), XVar.Pack(null)))  || (XVar)((XVar)(this.insertNull((XVar)(valueOp.Key)))  && (XVar)(XVar.Equals(XVar.Pack(MVCFunctions.trim((XVar)(valueOp.Value.value))), XVar.Pack("")))))
					{
						sqlValues.InitAndSetArrayItem("NULL", sqlField);
					}
					else
					{
						dynamic sqlValue = null;
						if(XVar.Pack(sqlValue = XVar.Clone(this.prepareBlob((XVar)(valueOp.Key), (XVar)(valueOp.Value.value), (XVar)(blobs), (XVar)(blobTypes)))))
						{
							sqlValues.InitAndSetArrayItem(sqlValue, sqlField);
						}
						else
						{
							sqlValues.InitAndSetArrayItem(this.prepareInsertSQLValue((XVar)(valueOp.Key), (XVar)(valueOp.Value.value)), sqlField);
						}
					}
				}
				else
				{
					if(XVar.Equals(XVar.Pack(valueOp.Value.var_type), XVar.Pack(Constants.dsotSQL)))
					{
						sqlValues.InitAndSetArrayItem(valueOp.Value.value, sqlField);
					}
					else
					{
						if(XVar.Equals(XVar.Pack(valueOp.Value.var_type), XVar.Pack(Constants.dsotFIELD)))
						{
							sqlValues.InitAndSetArrayItem(this.getUpdateFieldSQL((XVar)(valueOp.Value.value)), sqlField);
						}
					}
				}
			}
			dc._cache.InitAndSetArrayItem(sqlValues, "sqlValues");
			dc._cache.InitAndSetArrayItem(blobs, "blobs");
			dc._cache.InitAndSetArrayItem(blobTypes, "blobTypes");

			return null;
		}
		protected virtual XVar getUpdateFieldSQL(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.connection.addFieldWrappers((XVar)(field));
		}
		protected virtual XVar insertNull(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return false;
		}
		protected virtual XVar prepareBlob(dynamic _param_field, dynamic value, dynamic blobs, dynamic blobTypes)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic blobExpression = null, blobKey = null;
			if(XVar.Pack(!(XVar)(CommonFunctions.IsBinaryType((XVar)(this.getFieldType((XVar)(field)))))))
			{
				return false;
			}
			if(GlobalVars.projectLanguage == "php")
			{
				if((XVar)((XVar)(this.connection.dbType == Constants.nDATABASE_Oracle)  || (XVar)(this.connection.dbType == Constants.nDATABASE_DB2))  || (XVar)(this.connection.dbType == Constants.nDATABASE_Informix))
				{
					blobKey = XVar.Clone(field);
					if(this.connection.dbType == Constants.nDATABASE_Oracle)
					{
						blobKey = XVar.Clone(this.getUpdateFieldSQL((XVar)(field)));
					}
					blobs.InitAndSetArrayItem(value, blobKey);
					blobTypes.InitAndSetArrayItem(this.getFieldType((XVar)(field)), blobKey);
					blobExpression = XVar.Clone((XVar.Pack(this.connection.dbType == Constants.nDATABASE_Oracle) ? XVar.Pack("EMPTY_BLOB()") : XVar.Pack("?")));
					return blobExpression;
				}
			}
			else
			{
				if(GlobalVars.projectLanguage == "aspx")
				{
					if((XVar)(this.connection.dbType == Constants.nDATABASE_Oracle)  || (XVar)(this.connection.dbType == Constants.nDATABASE_DB2))
					{
						dynamic blobAlias = null;
						blobAlias = XVar.Clone(MVCFunctions.Concat("bnd", MVCFunctions.count(blobs) + 1));
						blobKey = XVar.Clone(blobAlias);
						if(this.connection.dbType == Constants.nDATABASE_Oracle)
						{
							blobKey = XVar.Clone(this.getUpdateFieldSQL((XVar)(field)));
						}
						blobs.InitAndSetArrayItem(value, blobKey);
						blobTypes.InitAndSetArrayItem(this.getFieldType((XVar)(field)), blobKey);
						blobExpression = XVar.Clone((XVar.Pack(this.connection.dbType == Constants.nDATABASE_Oracle) ? XVar.Pack(MVCFunctions.Concat(":", blobAlias)) : XVar.Pack("?")));
						return blobExpression;
					}
				}
			}
			return null;
		}
		public virtual XVar prepareInsertSQLValue(dynamic _param_field, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic fieldType = null;
			fieldType = XVar.Clone(this.getFieldType((XVar)(field)));
			return this.prepareSQLValue((XVar)(fieldType), (XVar)(value));
		}
		public override XVar silentMode(dynamic _param_mode)
		{
			#region pass-by-value parameters
			dynamic mode = XVar.Clone(_param_mode);
			#endregion

			this.connection.setSilentMode((XVar)(mode));

			return null;
		}
	}
	public partial class DsFilterBuildContext : XClass
	{
		public dynamic joins = XVar.Array();
		public dynamic extraColumnSQL = XVar.Array();
		public dynamic joinedAliases = XVar.Array();
		public dynamic useSubquery = XVar.Pack(false);
	}
}

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
	public partial class SQLEntity : XClass
	{
		public dynamic sql;
		public SQLEntity(dynamic _param_proto)
		{
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

		}
		public virtual XVar IsAggregationFunctionCall()
		{
			return false;
		}
		public virtual XVar IsBinary()
		{
			return false;
		}
		public virtual XVar IsAsterisk()
		{
			return false;
		}
		public virtual XVar SetQuery(dynamic query)
		{

			return null;
		}
		public virtual XVar IsSQLField()
		{
			return false;
		}
	}
	public partial class SQLNonParsed : SQLEntity
	{
		protected static bool skipSQLNonParsedCtor = false;
		public SQLNonParsed(dynamic _param_proto)
			:base((XVar)_param_proto)
		{
			if(skipSQLNonParsedCtor)
			{
				skipSQLNonParsedCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			this.sql = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["sql"])));
		}
		public virtual XVar toSql(dynamic _param_query)
		{
			#region pass-by-value parameters
			dynamic query = XVar.Clone(_param_query);
			#endregion

			return this.sql;
		}
		public override XVar IsAsterisk()
		{
			dynamic last = null;
			last = XVar.Clone(MVCFunctions.substr((XVar)(this.sql), (XVar)(MVCFunctions.strlen((XVar)(this.sql)) - 1)));
			return last == "*";
		}
		public virtual XVar fromJson(dynamic _param_proto)
		{
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			return new SQLNonParsed((XVar)(proto));
		}
	}
	public partial class SQLField : SQLEntity
	{
		public dynamic name;
		public dynamic table;
		protected static bool skipSQLFieldCtor = false;
		public SQLField(dynamic _param_proto)
			:base((XVar)_param_proto)
		{
			if(skipSQLFieldCtor)
			{
				skipSQLFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			this.name = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["name"])));
			this.table = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["table"])));
		}
		public virtual XVar toSql(dynamic _param_query)
		{
			#region pass-by-value parameters
			dynamic query = XVar.Clone(_param_query);
			#endregion

			dynamic fieldName = null;
			if(query.cipherer != null)
			{
				return query.cipherer.GetFieldName((XVar)(MVCFunctions.Concat((XVar.Pack((XVar)(this.table != "")  && (XVar)(1 < query.TableCount())) ? XVar.Pack(MVCFunctions.Concat(query.connection.addTableWrappers((XVar)(this.table)), ".")) : XVar.Pack("")), query.connection.addFieldWrappers((XVar)(this.name)))));
			}
			fieldName = XVar.Clone(query.connection.addFieldWrappers((XVar)(this.name)));
			if((XVar)(this.table == "")  || (XVar)(query.TableCount() == 1))
			{
				return fieldName;
			}
			return MVCFunctions.Concat(query.connection.addTableWrappers((XVar)(this.table)), ".", fieldName);
		}
		public virtual XVar GetType()
		{
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.table)));
			return pSet.getFieldType((XVar)(this.name));
		}
		public override XVar IsBinary()
		{
			return CommonFunctions.IsBinaryType((XVar)(this.GetType()));
		}
		public override XVar IsSQLField()
		{
			return true;
		}
	}
	public partial class SQLFieldListItem : SQLEntity
	{
		public dynamic expression;
		public dynamic alias;
		public dynamic columnName;
		public dynamic encrypted = XVar.Pack(false);
		protected static bool skipSQLFieldListItemCtor = false;
		public SQLFieldListItem(dynamic _param_proto)
			:base((XVar)_param_proto)
		{
			if(skipSQLFieldListItemCtor)
			{
				skipSQLFieldListItemCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			this.expression = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["expression"])));
			this.alias = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["alias"])));
			this.sql = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["sql"])));
			this.columnName = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["columnName"])));
			this.encrypted = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["encrypted"])));
			if(XVar.Pack(!(XVar)(this.expression)))
			{
				this.expression = XVar.Clone(new SQLNonParsed((XVar)(new XVar("sql", this.sql))));
			}
		}
		public virtual XVar toSql(dynamic _param_query, dynamic _param_addAlias = null)
		{
			#region default values
			if(_param_addAlias as Object == null) _param_addAlias = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic query = XVar.Clone(_param_query);
			dynamic addAlias = XVar.Clone(_param_addAlias);
			#endregion

			dynamic ret = null;
			ret = new XVar("");
			if(XVar.Pack(this.expression))
			{
				if(XVar.Pack(MVCFunctions.is_string((XVar)(this.expression))))
				{
					ret = XVar.Clone(this.expression);
				}
				else
				{
					if(XVar.Pack(MVCFunctions.is_a((XVar)(this.expression), new XVar("SQLQuery"))))
					{
						ret = XVar.Clone(this.sql);
					}
					else
					{
						ret = XVar.Clone(this.expression.toSql((XVar)(query)));
					}
				}
			}
			if(XVar.Pack(this.encrypted))
			{
				if(XVar.Pack(!(XVar)(query.cipherer.isEncryptionByPHPEnabled())))
				{
					ret = XVar.Clone(query.cipherer.GetEncryptedFieldName((XVar)(ret)));
				}
			}
			if(XVar.Pack(addAlias))
			{
				if(this.alias != "")
				{
					ret = MVCFunctions.Concat(ret, " as ", query.connection.addFieldWrappers((XVar)(this.alias)));
				}
				else
				{
					if(XVar.Pack(this.encrypted))
					{
						ret = MVCFunctions.Concat(ret, " as ", query.connection.addFieldWrappers((XVar)(this.columnName)));
					}
				}
			}
			return ret;
		}
		public override XVar IsAsterisk()
		{
			if(XVar.Pack(MVCFunctions.is_object((XVar)(this.expression))))
			{
				return this.expression.IsAsterisk();
			}
			return false;
		}
		public override XVar IsAggregationFunctionCall()
		{
			if(XVar.Pack(MVCFunctions.is_object((XVar)(this.expression))))
			{
				return this.expression.IsAggregationFunctionCall();
			}
			return false;
		}
		public virtual XVar getAlias()
		{
			return this.columnName;
		}
	}
	public partial class SQLFromListItem : SQLEntity
	{
		public dynamic link;
		public dynamic table;
		public dynamic alias;
		public dynamic joinOn;
		protected static bool skipSQLFromListItemCtor = false;
		public SQLFromListItem(dynamic _param_proto)
			:base((XVar)_param_proto)
		{
			if(skipSQLFromListItemCtor)
			{
				skipSQLFromListItemCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			this.link = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["link"])));
			this.table = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["table"])));
			this.alias = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["alias"])));
			this.joinOn = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["joinOn"])));
			this.sql = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["sql"])));
			if(XVar.Pack(!(XVar)(this.table)))
			{
				this.table = XVar.Clone(new SQLNonParsed((XVar)(new XVar("sql", this.sql))));
			}
		}
		public override XVar SetQuery(dynamic query)
		{
			if(XVar.Pack(MVCFunctions.is_object((XVar)(this.table))))
			{
				this.table.SetQuery((XVar)(query));
			}

			return null;
		}
		public virtual XVar toSql(dynamic _param_query, dynamic _param_first)
		{
			#region pass-by-value parameters
			dynamic query = XVar.Clone(_param_query);
			dynamic first = XVar.Clone(_param_first);
			#endregion

			dynamic joinStr = null, ret = null, skipAlias = null;
			ret = new XVar("");
			skipAlias = new XVar(false);
			if(XVar.Pack(MVCFunctions.is_a((XVar)(this.table), new XVar("SQLTable"))))
			{
				ret = MVCFunctions.Concat(ret, this.table.toSql((XVar)(query)));
			}
			else
			{
				return this.sql;
			}
			if((XVar)(this.alias != "")  && (XVar)(!(XVar)(skipAlias)))
			{
				ret = MVCFunctions.Concat(ret, " ", query.connection.addFieldWrappers((XVar)(this.alias)));
			}
			if(this.link == Constants.sqlLinkMAIN)
			{
				return ret;
			}
			switch(((XVar)this.link).ToInt())
			{
				case Constants.sqlLinkINNERJOIN:
					ret = XVar.Clone(MVCFunctions.Concat(" INNER JOIN ", ret));
					break;
				case Constants.sqlLinkNATURALJOIN:
					ret = XVar.Clone(MVCFunctions.Concat(" NATURAL JOIN ", ret));
					break;
				case Constants.sqlLinkLEFTJOIN:
					ret = XVar.Clone(MVCFunctions.Concat(" LEFT OUTER JOIN ", ret));
					break;
				case Constants.sqlLinkRIGHTJOIN:
					ret = XVar.Clone(MVCFunctions.Concat(" RIGHT OUTER JOIN ", ret));
					break;
				case Constants.sqlLinkFULLOUTERJOIN:
					ret = XVar.Clone(MVCFunctions.Concat(" FULL OUTER JOIN ", ret));
					break;
				case Constants.sqlLinkCROSSJOIN:
					ret = XVar.Clone(MVCFunctions.Concat((XVar.Pack(!(XVar)(first)) ? XVar.Pack(",") : XVar.Pack("")), ret));
					break;
			}
			joinStr = XVar.Clone(this.joinOn.toSql((XVar)(query)));
			if(joinStr != XVar.Pack(""))
			{
				ret = MVCFunctions.Concat(ret, " ON ", joinStr);
			}
			return ret;
		}
		public virtual XVar getIdentifier()
		{
			if(this.alias != "")
			{
				return this.alias;
			}
			return this.table;
		}
	}
	public partial class SQLJoinOn : SQLEntity
	{
		public dynamic field1;
		public dynamic field2;
		protected static bool skipSQLJoinOnCtor = false;
		public SQLJoinOn(dynamic _param_proto)
			:base((XVar)_param_proto)
		{
			if(skipSQLJoinOnCtor)
			{
				skipSQLJoinOnCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			this.field1 = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["field1"])));
			this.field2 = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["field2"])));
		}
	}
	public partial class SQLFunctionCall : SQLEntity
	{
		public dynamic functionType;
		public dynamic functionName;
		public dynamic arguments;
		protected static bool skipSQLFunctionCallCtor = false;
		public SQLFunctionCall(dynamic _param_proto)
			:base((XVar)_param_proto)
		{
			if(skipSQLFunctionCallCtor)
			{
				skipSQLFunctionCallCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			this.functionType = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["functionType"])));
			this.functionName = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["functionName"])));
			this.arguments = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["arguments"])));
		}
		public virtual XVar toSql(dynamic _param_query)
		{
			#region pass-by-value parameters
			dynamic query = XVar.Clone(_param_query);
			#endregion

			dynamic args = XVar.Array(), ret = null;
			ret = new XVar("");
			switch(((XVar)this.functionType).ToInt())
			{
				case Constants.SQLF_AVG:
					ret = MVCFunctions.Concat(ret, " AVG");
					break;
				case Constants.SQLF_SUM:
					ret = MVCFunctions.Concat(ret, " SUM");
					break;
				case Constants.SQLF_MIN:
					ret = MVCFunctions.Concat(ret, " MIN");
					break;
				case Constants.SQLF_MAX:
					ret = MVCFunctions.Concat(ret, " MAX");
					break;
				case Constants.SQLF_COUNT:
					ret = MVCFunctions.Concat(ret, " COUNT");
					break;
				default:
					ret = MVCFunctions.Concat(ret, this.functionName);
					break;
			}
			args = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> a in this.arguments.GetEnumerator())
			{
				args.InitAndSetArrayItem(a.Value.toSql((XVar)(query)), null);
			}
			ret = MVCFunctions.Concat(ret, "(", MVCFunctions.implode(new XVar(","), (XVar)(args)), ")");
			return ret;
		}
		public override XVar IsAggregationFunctionCall()
		{
			switch(((XVar)this.functionType).ToInt())
			{
				case Constants.SQLF_AVG:
				case Constants.SQLF_SUM:
				case Constants.SQLF_MIN:
				case Constants.SQLF_MAX:
				case Constants.SQLF_COUNT:
					return true;
			}
			if(MVCFunctions.strtolower((XVar)(this.functionName)) == "group_concat")
			{
				return true;
			}
			return false;
		}
	}
	public partial class SQLGroupByItem : SQLEntity
	{
		public dynamic column;
		protected static bool skipSQLGroupByItemCtor = false;
		public SQLGroupByItem(dynamic _param_proto)
			:base((XVar)_param_proto)
		{
			if(skipSQLGroupByItemCtor)
			{
				skipSQLGroupByItemCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			this.column = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["column"])));
			this.sql = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["sql"])));
			if(XVar.Pack(!(XVar)(this.column)))
			{
				this.column = XVar.Clone(new SQLNonParsed((XVar)(new XVar("sql", this.sql))));
			}
		}
		public virtual XVar toSql(dynamic _param_query)
		{
			#region pass-by-value parameters
			dynamic query = XVar.Clone(_param_query);
			#endregion

			return this.column.toSql((XVar)(query));
		}
	}
	public partial class SQLLogicalExpr : SQLEntity
	{
		public dynamic unionType;
		public dynamic column;
		public dynamic var_case;
		public dynamic havingMode;
		public dynamic contained;
		public dynamic inBrackets;
		public dynamic useAlias;
		public dynamic query;
		public dynamic postSql;
		protected static bool skipSQLLogicalExprCtor = false;
		public SQLLogicalExpr(dynamic _param_proto)
			:base((XVar)_param_proto)
		{
			if(skipSQLLogicalExprCtor)
			{
				skipSQLLogicalExprCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			this.sql = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["sql"])));
			this.unionType = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["unionType"])));
			this.column = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["column"])));
			this.var_case = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["case"])));
			this.havingMode = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["havingMode"])));
			this.contained = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["contained"])));
			this.inBrackets = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["inBrackets"])));
			this.useAlias = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["useAlias"])));
			this.postSql = XVar.Clone(XVar.Array());
			if(XVar.Pack(!(XVar)(this.column)))
			{
				this.column = XVar.Clone(new SQLNonParsed((XVar)(new XVar("sql", this.sql))));
			}
		}
		public override XVar SetQuery(dynamic query)
		{
			dynamic nCnt = null;
			this.query = query;
			nCnt = new XVar(0);
			for(;nCnt < MVCFunctions.count(this.contained); nCnt++)
			{
				this.contained[nCnt].SetQuery((XVar)(query));
			}

			return null;
		}
		public virtual XVar toSql(dynamic _param_query)
		{
			#region pass-by-value parameters
			dynamic query = XVar.Clone(_param_query);
			#endregion

			dynamic ret = null;
			ret = new XVar("");
			if(XVar.Pack(this.haveContained()))
			{
				dynamic contained = XVar.Array(), glue = null;
				glue = new XVar("");
				if(this.unionType == Constants.sqlUnionAND)
				{
					glue = new XVar(" AND ");
				}
				else
				{
					if(this.unionType == Constants.sqlUnionOR)
					{
						glue = new XVar(" OR ");
					}
					else
					{
						MVCFunctions.ob_flush();
						HttpContext.Current.Response.End();
						throw new RunnerInlineOutputException();
					}
				}
				contained = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> c in this.contained.GetEnumerator())
				{
					dynamic cSql = null;
					cSql = XVar.Clone(c.Value.toSql((XVar)(query)));
					if(cSql != XVar.Pack(""))
					{
						contained.InitAndSetArrayItem(MVCFunctions.Concat("(", cSql, ")"), null);
					}
				}
				if(0 < MVCFunctions.count(contained))
				{
					ret = XVar.Clone(MVCFunctions.implode((XVar)(glue), (XVar)(contained)));
				}
				if(0 < MVCFunctions.count(this.postSql))
				{
					dynamic nCnt = null;
					if(ret == XVar.Pack(""))
					{
						ret = MVCFunctions.Concat(ret, "(", this.postSql[0], ")");
					}
					else
					{
						ret = XVar.Clone(MVCFunctions.Concat("(", ret, ")", glue, "(", this.postSql[0], ")"));
					}
					nCnt = new XVar(1);
					for(;nCnt < MVCFunctions.count(this.postSql); nCnt++)
					{
						ret = MVCFunctions.Concat(ret, glue, "(", this.postSql[nCnt], ")");
					}
				}
				if(XVar.Pack(this.inBrackets))
				{
					ret = XVar.Clone(MVCFunctions.Concat("(", ret, ")"));
				}
				return ret;
			}
			if(XVar.Pack(this.sql))
			{
				return this.sql;
			}
			if(XVar.Pack(!(XVar)(this.column)))
			{
				ret = XVar.Clone(this.sql);
			}
			else
			{
				if(XVar.Pack(this.useAlias))
				{
					ret = XVar.Clone(this.column.alias);
				}
				else
				{
					ret = XVar.Clone(this.column.toSql((XVar)(query)));
				}
			}
			if(this.var_case == "NOT")
			{
				return MVCFunctions.Concat(" NOT ", ret);
			}
			else
			{
				if(ret != XVar.Pack(""))
				{
					ret = MVCFunctions.Concat(ret, " ", this.var_case);
				}
			}
			if(XVar.Pack(this.inBrackets))
			{
				ret = XVar.Clone(MVCFunctions.Concat("(", ret, ")"));
			}
			return ret;
		}
		public virtual XVar haveContained()
		{
			return (XVar)(0 < MVCFunctions.count(this.contained))  || (XVar)(0 < MVCFunctions.count(this.postSql));
		}
	}
	public partial class SQLOrderByItem : SQLEntity
	{
		public dynamic column;
		public dynamic asc;
		public dynamic columnNumber;
		protected static bool skipSQLOrderByItemCtor = false;
		public SQLOrderByItem(dynamic _param_proto)
			:base((XVar)_param_proto)
		{
			if(skipSQLOrderByItemCtor)
			{
				skipSQLOrderByItemCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			this.column = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["column"])));
			this.asc = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["asc"])));
			this.columnNumber = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["columnNumber"])));
			this.sql = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["sql"])));
			if(XVar.Pack(!(XVar)(this.column)))
			{
				this.column = XVar.Clone(new SQLNonParsed((XVar)(new XVar("sql", this.sql))));
			}
		}
		public virtual XVar toSql(dynamic _param_query)
		{
			#region pass-by-value parameters
			dynamic query = XVar.Clone(_param_query);
			#endregion

			dynamic ret = null;
			ret = new XVar("");
			if(0 == this.columnNumber)
			{
				ret = MVCFunctions.Concat(ret, this.column.toSql((XVar)(query)));
			}
			else
			{
				ret = MVCFunctions.Concat(ret, this.columnNumber);
			}
			if(XVar.Pack(!(XVar)(this.asc)))
			{
				ret = MVCFunctions.Concat(ret, " DESC ");
			}
			return ret;
		}
	}
	public partial class SQLTable : SQLEntity
	{
		public dynamic name;
		public dynamic columns;
		public dynamic query;
		protected static bool skipSQLTableCtor = false;
		public SQLTable(dynamic _param_proto)
			:base((XVar)_param_proto)
		{
			if(skipSQLTableCtor)
			{
				skipSQLTableCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			this.name = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["name"])));
			this.columns = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["columns"])));
		}
		public override XVar SetQuery(dynamic query)
		{
			this.query = XVar.Clone(query);

			return null;
		}
		public virtual XVar toSql(dynamic _param_query)
		{
			#region pass-by-value parameters
			dynamic query = XVar.Clone(_param_query);
			#endregion

			return query.connection.addTableWrappers((XVar)(this.name));
		}
	}
	public partial class SQLQuery : SQLEntity
	{
		public dynamic headSql;
		public dynamic fieldListSql;
		public dynamic fromListSql;
		public dynamic whereSql;
		public dynamic orderBySql;
		public dynamic where;
		public dynamic having;
		public dynamic fieldList;
		public dynamic fromList;
		public dynamic groupBy;
		public dynamic orderBy;
		public dynamic bHasAsterisks = XVar.Pack(false);
		public dynamic connection = XVar.Pack(null);
		public dynamic cipherer = XVar.Pack(null);
		protected static bool skipSQLQueryCtor = false;
		public SQLQuery(dynamic _param_proto)
			:base((XVar)_param_proto)
		{
			if(skipSQLQueryCtor)
			{
				skipSQLQueryCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			dynamic i = null, nCnt = null;
			this.headSql = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["headSql"])));
			this.fieldListSql = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["fieldListSql"])));
			this.fromListSql = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["fromListSql"])));
			this.whereSql = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["whereSql"])));
			this.orderBySql = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["orderBySql"])));
			this.where = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["where"])));
			this.having = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["having"])));
			this.fieldList = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["fieldList"])));
			this.fromList = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["fromList"])));
			this.groupBy = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["groupBy"])));
			this.orderBy = XVar.Clone(CommonFunctions.sqlFromJson((XVar)(proto["orderBy"])));
			nCnt = new XVar(0);
			for(;nCnt < MVCFunctions.count(this.fromList); nCnt++)
			{
				this.fromList[nCnt].SetQuery(this);
			}
			this.where.SetQuery(this);
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(this.fieldList)))))
			{
				return;
			}
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.fieldList); i++)
			{
				if(XVar.Pack(this.fieldList[i].IsAsterisk()))
				{
					this.bHasAsterisks = new XVar(true);
					break;
				}
			}
		}
		public virtual XVar FromToSql()
		{
			dynamic nCnt = null, sql = null;
			sql = new XVar("");
			if(0 < MVCFunctions.count(this.fromList))
			{
				sql = MVCFunctions.Concat(sql, "\r\n");
				sql = MVCFunctions.Concat(sql, " FROM ");
			}
			if(this.connection.dbType == Constants.nDATABASE_Access)
			{
				dynamic sqlFromList = null;
				sqlFromList = new XVar("");
				nCnt = new XVar(0);
				for(;nCnt < MVCFunctions.count(this.fromList); nCnt++)
				{
					if(!XVar.Equals(XVar.Pack(sqlFromList), XVar.Pack("")))
					{
						sqlFromList = XVar.Clone(MVCFunctions.Concat("(", sqlFromList, ")"));
					}
					sqlFromList = MVCFunctions.Concat(sqlFromList, this.fromList[nCnt].toSql(this, (XVar)(nCnt == XVar.Pack(0))));
				}
				sql = MVCFunctions.Concat(sql, sqlFromList);
			}
			else
			{
				dynamic fromList = XVar.Array();
				fromList = XVar.Clone(XVar.Array());
				nCnt = new XVar(0);
				for(;nCnt < MVCFunctions.count(this.fromList); nCnt++)
				{
					fromList.InitAndSetArrayItem(this.fromList[nCnt].toSql(this, (XVar)(nCnt == XVar.Pack(0))), null);
				}
				sql = MVCFunctions.Concat(sql, MVCFunctions.implode(new XVar("\r\n"), (XVar)(fromList)));
			}
			return sql;
		}
		public virtual XVar HavingToSql()
		{
			return this.having.toSql(this);
		}
		public virtual XVar OrderByToSql()
		{
			return this.orderBySql;
		}
		public virtual XVar HeadToSql(dynamic _param_oneRecordMode = null)
		{
			#region default values
			if(_param_oneRecordMode as Object == null) _param_oneRecordMode = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic oneRecordMode = XVar.Clone(_param_oneRecordMode);
			#endregion

			dynamic fields = XVar.Array(), sql = null;
			sql = new XVar("");
			sql = MVCFunctions.Concat(sql, this.headSql);
			if((XVar)(this.connection.dbType == Constants.nDATABASE_MSSQLServer)  || (XVar)(this.connection.dbType == Constants.nDATABASE_Access))
			{
				if(XVar.Pack(oneRecordMode))
				{
					sql = MVCFunctions.Concat(sql, " top 1 ");
				}
			}
			if(sql != XVar.Pack(""))
			{
				sql = MVCFunctions.Concat(sql, "\r\n");
			}
			fields = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in this.fieldList.GetEnumerator())
			{
				fields.InitAndSetArrayItem(f.Value.toSql(this), null);
			}
			if(0 < MVCFunctions.count(fields))
			{
				sql = MVCFunctions.Concat(sql, MVCFunctions.implode(new XVar(", "), (XVar)(fields)));
			}
			return sql;
		}
		public virtual XVar AddCustomExpression(dynamic _param_expression, dynamic _param_pSet_packed, dynamic _param_mainTable, dynamic _param_mainFiled, dynamic _param_alias = null)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region default values
			if(_param_alias as Object == null) _param_alias = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic expression = XVar.Clone(_param_expression);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic mainTable = XVar.Clone(_param_mainTable);
			dynamic mainFiled = XVar.Clone(_param_mainFiled);
			dynamic alias = XVar.Clone(_param_alias);
			#endregion

			dynamic index = null, itemFound = null;
			index = XVar.Clone(MVCFunctions.count(this.fieldList));
			itemFound = new XVar(false);
			foreach (KeyValuePair<XVar, dynamic> listItem in this.fieldList.GetEnumerator())
			{
				if((XVar)(listItem.Value.expression == expression)  && (XVar)(listItem.Value.alias == alias))
				{
					index = XVar.Clone(listItem.Key);
					itemFound = new XVar(true);
					break;
				}
			}
			if(XVar.Pack(!(XVar)(itemFound)))
			{
				this.fieldList.InitAndSetArrayItem(new SQLFieldListItem((XVar)(new XVar("expression", expression, "alias", alias))), null);
			}
			pSet.addCustomExpressionIndex((XVar)(mainTable), (XVar)(mainFiled), (XVar)(index));

			return null;
		}
		public virtual XVar GroupByToSql()
		{
			dynamic groupBy = XVar.Array(), sql = null;
			sql = new XVar("");
			groupBy = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> g in this.groupBy.GetEnumerator())
			{
				groupBy.InitAndSetArrayItem(g.Value.toSql(this), null);
			}
			if(0 < MVCFunctions.count(groupBy))
			{
				sql = MVCFunctions.Concat(sql, " GROUP BY ");
				sql = MVCFunctions.Concat(sql, MVCFunctions.implode(new XVar(","), (XVar)(groupBy)));
				sql = MVCFunctions.Concat(sql, " ");
			}
			return sql;
		}
		public virtual XVar GroupByHavingToSql()
		{
			dynamic sql = null, sqlGroup = null, sqlHaving = null;
			sql = new XVar("");
			sqlGroup = XVar.Clone(this.GroupByToSql());
			sqlHaving = XVar.Clone(this.HavingToSql());
			if(XVar.Pack(MVCFunctions.strlen((XVar)(sqlGroup))))
			{
				sql = MVCFunctions.Concat(sql, sqlGroup);
			}
			if(XVar.Pack(MVCFunctions.strlen((XVar)(sqlHaving))))
			{
				sql = MVCFunctions.Concat(sql, " HAVING ", sqlHaving);
			}
			return sql;
		}
		public virtual XVar toSql(dynamic _param_query)
		{
			#region pass-by-value parameters
			dynamic query = XVar.Clone(_param_query);
			#endregion

			dynamic joinFromPart = null, oneRecordMode = null, sql = null, strhaving = null, strorderby = null, strwhere = null;
			this.connection = XVar.Clone(query.connection);
			this.cipherer = XVar.Clone(query.cipherer);
			sql = XVar.Clone(this.HeadToSql((XVar)(oneRecordMode)));
			sql = MVCFunctions.Concat(sql, this.FromToSql());
			sql = MVCFunctions.Concat(sql, joinFromPart);
			if(strwhere != null)
			{
				if(!XVar.Equals(XVar.Pack(strwhere), XVar.Pack("")))
				{
					sql = MVCFunctions.Concat(sql, " WHERE ", strwhere, "\r\n");
				}
			}
			else
			{
				dynamic where = null;
				where = XVar.Clone(this.where.toSql(this));
				if(where != XVar.Pack(""))
				{
					sql = MVCFunctions.Concat(sql, " WHERE ", where, "\r\n");
				}
			}
			sql = MVCFunctions.Concat(sql, this.GroupByToSql());
			if(strhaving != null)
			{
				if(!XVar.Equals(XVar.Pack(strhaving), XVar.Pack("")))
				{
					sql = MVCFunctions.Concat(sql, " HAVING ", strhaving, "\r\n");
				}
			}
			else
			{
				dynamic having = null;
				having = XVar.Clone(this.having.toSql(this));
				if(having != XVar.Pack(""))
				{
					sql = MVCFunctions.Concat(sql, " HAVING ", having, "\r\n");
				}
			}
			if(!XVar.Equals(XVar.Pack(strorderby), XVar.Pack(null)))
			{
				sql = MVCFunctions.Concat(sql, strorderby, "\r\n");
			}
			else
			{
				dynamic orderBy = XVar.Array();
				orderBy = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> g in this.orderBy.GetEnumerator())
				{
					orderBy.InitAndSetArrayItem(g.Value.toSql(this), null);
				}
				if(0 < MVCFunctions.count(orderBy))
				{
					sql = MVCFunctions.Concat(sql, " ORDER BY ");
					sql = MVCFunctions.Concat(sql, MVCFunctions.implode(new XVar(","), (XVar)(orderBy)));
					sql = MVCFunctions.Concat(sql, "\r\n");
				}
			}
			if(this.connection.dbType == Constants.nDATABASE_MySQL)
			{
				if(XVar.Pack(oneRecordMode))
				{
					sql = MVCFunctions.Concat(sql, " limit 0,1");
				}
			}
			if(this.connection.dbType == Constants.nDATABASE_PostgreSQL)
			{
				if(XVar.Pack(oneRecordMode))
				{
					sql = MVCFunctions.Concat(sql, " limit 1");
				}
			}
			if(this.connection.dbType == Constants.nDATABASE_DB2)
			{
				if(XVar.Pack(oneRecordMode))
				{
					sql = MVCFunctions.Concat(sql, " fetch first 1 rows only");
				}
			}
			return sql;
		}
		public virtual XVar TableCount()
		{
			return MVCFunctions.count(this.fromList);
		}
		public virtual XVar IsAggrFuncField(dynamic _param_idx)
		{
			#region pass-by-value parameters
			dynamic idx = XVar.Clone(_param_idx);
			#endregion

			if(XVar.Pack(this.HasAsterisks()))
			{
				return false;
			}
			if(XVar.Pack(!(XVar)(this.fieldList.KeyExists(idx))))
			{
				return false;
			}
			return this.fieldList[idx].IsAggregationFunctionCall();
		}
		public virtual XVar ReplaceFieldsWithDummies(dynamic _param_fieldindices)
		{
			#region pass-by-value parameters
			dynamic fieldindices = XVar.Clone(_param_fieldindices);
			#endregion

			if(XVar.Pack(this.HasAsterisks()))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> idx in fieldindices.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(this.fieldList.KeyExists(idx.Value - 1))))
				{
					return null;
				}
				this.fieldList.InitAndSetArrayItem(new SQLFieldListItem((XVar)(new XVar("alias", MVCFunctions.Concat("runnerdummy", idx.Value), "expression", "1"))), idx.Value - 1);
			}

			return null;
		}
		public virtual XVar RemoveAllFieldsExcept(dynamic _param_idx)
		{
			#region pass-by-value parameters
			dynamic idx = XVar.Clone(_param_idx);
			#endregion

			dynamic i = null, removeindices = XVar.Array();
			if(XVar.Pack(this.HasAsterisks()))
			{
				return null;
			}
			removeindices = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.fieldList); i++)
			{
				if(i != idx - 1)
				{
					removeindices.InitAndSetArrayItem(i + 1, null);
				}
			}
			this.ReplaceFieldsWithDummies((XVar)(removeindices));

			return null;
		}
		public virtual XVar RemoveAllFieldsExceptList(dynamic _param_arr)
		{
			#region pass-by-value parameters
			dynamic arr = XVar.Clone(_param_arr);
			#endregion

			dynamic i = null, removeindices = XVar.Array();
			if(XVar.Pack(this.HasAsterisks()))
			{
				return null;
			}
			removeindices = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.fieldList); i++)
			{
				if(XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(i + 1), (XVar)(arr))), XVar.Pack(false)))
				{
					removeindices.InitAndSetArrayItem(i + 1, null);
				}
			}
			this.ReplaceFieldsWithDummies((XVar)(removeindices));

			return null;
		}
		public virtual XVar WhereToSql()
		{
			return this.where.toSql(this);
		}
		public virtual XVar Where()
		{
			return this.where;
		}
		public virtual SQLLogicalExpr Having()
		{
			return XVar.UnPackSQLLogicalExpr(this.having ?? new XVar());
		}
		public virtual XVar Copy()
		{
			return MVCFunctions.unserialize((XVar)(MVCFunctions.serialize(this)));
		}
		public virtual XVar HasGroupBy()
		{
			return 0 < MVCFunctions.count(this.groupBy);
		}
		public virtual XVar HasSubQueryInFromClause()
		{
			foreach (KeyValuePair<XVar, dynamic> fromList in this.fromList.GetEnumerator())
			{
				if((XVar)(MVCFunctions.is_object((XVar)(fromList.Value.table)))  && (XVar)(MVCFunctions.is_a((XVar)(fromList.Value.table), new XVar("SQLQuery"))))
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar HasJoinInFromClause()
		{
			return 1 < MVCFunctions.count(this.fromList);
		}
		public virtual XVar HasTableInFormClause(dynamic _param_tName)
		{
			#region pass-by-value parameters
			dynamic tName = XVar.Clone(_param_tName);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> fromList in this.fromList.GetEnumerator())
			{
				if(tName == fromList.Value.getIdentifier())
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar HasAsterisks()
		{
			return this.bHasAsterisks;
		}
		public virtual XVar addWhere(dynamic _param__where)
		{
			#region pass-by-value parameters
			dynamic _where = XVar.Clone(_param__where);
			#endregion

			dynamic myobj = null, myproto = XVar.Array(), newobj = null, newproto = XVar.Array();
			if(MVCFunctions.trim((XVar)(_where)) == "")
			{
				return null;
			}
			myproto = XVar.Clone(XVar.Array());
			myproto.InitAndSetArrayItem(_where, "sql");
			myproto.InitAndSetArrayItem(Constants.sqlUnionUNKNOWN, "unionType");
			myproto.InitAndSetArrayItem(null, "column");
			myproto.InitAndSetArrayItem(XVar.Array(), "contained");
			myproto.InitAndSetArrayItem("", "case");
			myproto.InitAndSetArrayItem(false, "havingMode");
			myproto.InitAndSetArrayItem(true, "inBrackets");
			myproto.InitAndSetArrayItem(false, "useAlias");
			myobj = XVar.Clone(new SQLLogicalExpr((XVar)(myproto)));
			myobj.query = XVar.Clone(this);
			if(XVar.Pack(!(XVar)(this.where)))
			{
				this.where = XVar.Clone(myobj);
				return null;
			}
			newproto = XVar.Clone(XVar.Array());
			newproto.InitAndSetArrayItem(Constants.sqlUnionAND, "unionType");
			newproto.InitAndSetArrayItem(XVar.Array(), "contained");
			newproto.InitAndSetArrayItem(this.where, "contained", null);
			newproto.InitAndSetArrayItem(myobj, "contained", null);
			newobj = XVar.Clone(new SQLLogicalExpr((XVar)(newproto)));
			newobj.query = XVar.Clone(this);
			this.where = XVar.Clone(newobj);

			return null;
		}
		public virtual XVar replaceWhere(dynamic _param__where)
		{
			#region pass-by-value parameters
			dynamic _where = XVar.Clone(_param__where);
			#endregion

			dynamic myobj = null, myproto = XVar.Array();
			if(MVCFunctions.trim((XVar)(_where)) == "")
			{
				myproto = XVar.Clone(XVar.Array());
				myobj = XVar.Clone(new SQLLogicalExpr((XVar)(myproto)));
				myobj.query = XVar.Clone(this);
				this.where = XVar.Clone(myobj);
				return null;
			}
			myproto = XVar.Clone(XVar.Array());
			myproto.InitAndSetArrayItem(_where, "sql");
			myproto.InitAndSetArrayItem(Constants.sqlUnionUNKNOWN, "unionType");
			myproto.InitAndSetArrayItem(null, "column");
			myproto.InitAndSetArrayItem(XVar.Array(), "contained");
			myproto.InitAndSetArrayItem("", "case");
			myproto.InitAndSetArrayItem(false, "havingMode");
			myproto.InitAndSetArrayItem(true, "inBrackets");
			myproto.InitAndSetArrayItem(false, "useAlias");
			myobj = XVar.Clone(new SQLLogicalExpr((XVar)(myproto)));
			myobj.query = XVar.Clone(this);
			this.where = XVar.Clone(myobj);

			return null;
		}
		public virtual XVar addField(dynamic _param__field, dynamic _param__alias)
		{
			#region pass-by-value parameters
			dynamic _field = XVar.Clone(_param__field);
			dynamic _alias = XVar.Clone(_param__alias);
			#endregion

			dynamic myobj = null, myproto = XVar.Array();
			myproto = XVar.Clone(XVar.Array());
			myobj = XVar.Clone(new SQLNonParsed((XVar)(new XVar("sql", _field))));
			myproto.InitAndSetArrayItem(myobj, "expression");
			myproto.InitAndSetArrayItem(_alias, "alias");
			myobj = XVar.Clone(new SQLFieldListItem((XVar)(myproto)));
			this.fieldList.InitAndSetArrayItem(myobj, null);

			return null;
		}
		public virtual XVar replaceField(dynamic _param__field, dynamic _param__newfield, dynamic _param__newalias = null)
		{
			#region default values
			if(_param__newalias as Object == null) _param__newalias = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic _field = XVar.Clone(_param__field);
			dynamic _newfield = XVar.Clone(_param__newfield);
			dynamic _newalias = XVar.Clone(_param__newalias);
			#endregion

			dynamic myobj = null, myproto = XVar.Array();
			myproto = XVar.Clone(XVar.Array());
			myobj = XVar.Clone(new SQLNonParsed((XVar)(new XVar("sql", _newfield))));
			myproto.InitAndSetArrayItem(myobj, "expression");
			if(XVar.Pack(!(XVar)(MVCFunctions.IsNumeric(_field))))
			{
				foreach (KeyValuePair<XVar, dynamic> obj in this.fieldList.GetEnumerator())
				{
					if(this.fieldList[obj.Key].getAlias() == _field)
					{
						_field = XVar.Clone(obj.Key);
						break;
					}
				}
			}
			if(XVar.Pack(MVCFunctions.IsNumeric(_field)))
			{
				if(XVar.Pack(!(XVar)(_newalias)))
				{
					_newalias = XVar.Clone(this.fieldList[_field].getAlias());
				}
				myproto.InitAndSetArrayItem(_newalias, "alias");
				myobj = XVar.Clone(new SQLFieldListItem((XVar)(myproto)));
				this.fieldList.InitAndSetArrayItem(myobj, _field);
			}

			return null;
		}
		public virtual XVar deleteField(dynamic _param__field)
		{
			#region pass-by-value parameters
			dynamic _field = XVar.Clone(_param__field);
			#endregion

			if(XVar.Pack(!(XVar)(MVCFunctions.IsNumeric(_field))))
			{
				foreach (KeyValuePair<XVar, dynamic> obj in this.fieldList.GetEnumerator())
				{
					if(this.fieldList[obj.Key].getAlias() == _field)
					{
						_field = XVar.Clone(obj.Key);
						break;
					}
				}
			}
			if(XVar.Pack(MVCFunctions.IsNumeric(_field)))
			{
				dynamic fieldList = null;
				fieldList = XVar.Clone(this.fieldList);
				MVCFunctions.array_splice((XVar)(fieldList), (XVar)(_field), new XVar(1));
				this.fieldList = XVar.Clone(fieldList);
			}

			return null;
		}
		public virtual XVar deleteAllFieldsExcept(dynamic _param_idx)
		{
			#region pass-by-value parameters
			dynamic idx = XVar.Clone(_param_idx);
			#endregion

			dynamic field = null;
			field = XVar.Clone(this.fieldList[idx]);
			this.fieldList = XVar.Clone(XVar.Array());
			this.fieldList.InitAndSetArrayItem(field, null);

			return null;
		}
		public virtual XVar getSqlComponents(dynamic _param_connection, dynamic _param_cipherer, dynamic _param_oneRecordMode = null)
		{
			#region default values
			if(_param_oneRecordMode as Object == null) _param_oneRecordMode = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic connection = XVar.Clone(_param_connection);
			dynamic cipherer = XVar.Clone(_param_cipherer);
			dynamic oneRecordMode = XVar.Clone(_param_oneRecordMode);
			#endregion

			this.connection = XVar.Clone(connection);
			this.cipherer = XVar.Clone(cipherer);
			return new XVar("head", this.HeadToSql((XVar)(oneRecordMode)), "from", this.FromToSql(), "where", this.WhereToSql(), "groupby", this.GroupByToSql(), "having", this.Having().toSql(this));
		}
		public static XVar buildSQL(dynamic _param_sqlParts, dynamic _param_mandatoryWhere = null, dynamic _param_mandatoryHaving = null, dynamic _param_optionalWhere = null, dynamic _param_optionalHaving = null)
		{
			#region default values
			if(_param_mandatoryWhere as Object == null) _param_mandatoryWhere = new XVar(XVar.Array());
			if(_param_mandatoryHaving as Object == null) _param_mandatoryHaving = new XVar(XVar.Array());
			if(_param_optionalWhere as Object == null) _param_optionalWhere = new XVar(XVar.Array());
			if(_param_optionalHaving as Object == null) _param_optionalHaving = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic sqlParts = XVar.Clone(_param_sqlParts);
			dynamic mandatoryWhere = XVar.Clone(_param_mandatoryWhere);
			dynamic mandatoryHaving = XVar.Clone(_param_mandatoryHaving);
			dynamic optionalWhere = XVar.Clone(_param_optionalWhere);
			dynamic optionalHaving = XVar.Clone(_param_optionalHaving);
			#endregion

			dynamic mHaving = null, mWhere = null, oHaving = null, oWhere = null, prepSqlParts = XVar.Array(), sqlHead = null, unionMode = null;
			prepSqlParts = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> s in sqlParts.GetEnumerator())
			{
				prepSqlParts.InitAndSetArrayItem(DB.PrepareSQL((XVar)(s.Value)), s.Key);
			}
			sqlHead = XVar.Clone(prepSqlParts["head"]);
			if(0 == MVCFunctions.strlen((XVar)(sqlHead)))
			{
				sqlHead = new XVar("select * ");
			}
			unionMode = XVar.Clone((XVar)(optionalWhere)  && (XVar)(optionalHaving));
			mWhere = XVar.Clone(SQLQuery.combineCases((XVar)(mandatoryWhere), new XVar("and")));
			oWhere = XVar.Clone(SQLQuery.combineCases((XVar)(optionalWhere), new XVar("or")));
			mHaving = XVar.Clone(SQLQuery.combineCases((XVar)(mandatoryHaving), new XVar("and")));
			oHaving = XVar.Clone(SQLQuery.combineCases((XVar)(optionalHaving), new XVar("or")));
			if((XVar)(MVCFunctions.strlen((XVar)(oWhere)))  && (XVar)(MVCFunctions.strlen((XVar)(oHaving))))
			{
				dynamic having1 = null, having2 = null, where1 = null, where2 = null;
				where1 = XVar.Clone(SQLQuery.combineCases((XVar)(new XVar(0, mWhere, 1, oWhere, 2, prepSqlParts["where"])), new XVar("and")));
				having1 = XVar.Clone(SQLQuery.combineCases((XVar)(new XVar(0, mHaving, 1, prepSqlParts["having"])), new XVar("and")));
				where2 = XVar.Clone(SQLQuery.combineCases((XVar)(new XVar(0, mWhere, 1, prepSqlParts["where"])), new XVar("and")));
				having2 = XVar.Clone(SQLQuery.combineCases((XVar)(new XVar(0, mHaving, 1, oHaving, 2, prepSqlParts["having"])), new XVar("and")));
				if(0 != MVCFunctions.strlen((XVar)(where1)))
				{
					where1 = XVar.Clone(MVCFunctions.Concat(" WHERE ", where1));
				}
				if(0 != MVCFunctions.strlen((XVar)(having1)))
				{
					having1 = XVar.Clone(MVCFunctions.Concat(" HAVING ", having1));
				}
				if(0 != MVCFunctions.strlen((XVar)(where2)))
				{
					where2 = XVar.Clone(MVCFunctions.Concat(" WHERE ", where2));
				}
				if(0 != MVCFunctions.strlen((XVar)(having2)))
				{
					having2 = XVar.Clone(MVCFunctions.Concat(" HAVING ", having2));
				}
				return MVCFunctions.implode(new XVar(" "), (XVar)(new XVar(0, sqlHead, 1, prepSqlParts["from"], 2, where1, 3, prepSqlParts["groupby"], 4, having1, 5, "union", 6, sqlHead, 7, prepSqlParts["from"], 8, where2, 9, prepSqlParts["groupby"], 10, having2)));
			}
			else
			{
				dynamic having = null, where = null;
				where = XVar.Clone(SQLQuery.combineCases((XVar)(new XVar(0, mWhere, 1, oWhere, 2, prepSqlParts["where"])), new XVar("and")));
				having = XVar.Clone(SQLQuery.combineCases((XVar)(new XVar(0, mHaving, 1, oHaving, 2, prepSqlParts["having"])), new XVar("and")));
				if(0 != MVCFunctions.strlen((XVar)(where)))
				{
					where = XVar.Clone(MVCFunctions.Concat(" WHERE ", where));
				}
				if(0 != MVCFunctions.strlen((XVar)(having)))
				{
					having = XVar.Clone(MVCFunctions.Concat(" HAVING ", having));
				}
				return MVCFunctions.implode(new XVar(" "), (XVar)(new XVar(0, sqlHead, 1, prepSqlParts["from"], 2, where, 3, prepSqlParts["groupby"], 4, having)));
			}

			return null;
		}
		public static XVar combineCases(dynamic _param__cases, dynamic _param_operator)
		{
			#region pass-by-value parameters
			dynamic _cases = XVar.Clone(_param__cases);
			dynamic var_operator = XVar.Clone(_param_operator);
			#endregion

			dynamic cases = XVar.Array(), result = null;
			cases = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> c in _cases.GetEnumerator())
			{
				if(0 != MVCFunctions.strlen((XVar)(MVCFunctions.trim((XVar)(c.Value)))))
				{
					cases.InitAndSetArrayItem(c.Value, null);
				}
			}
			result = XVar.Clone(MVCFunctions.implode((XVar)(MVCFunctions.Concat(" ) ", var_operator, " ( ")), (XVar)(cases)));
			if(0 == MVCFunctions.strlen((XVar)(result)))
			{
				return "";
			}
			return MVCFunctions.Concat("( ", result, " )");
		}
		public virtual XVar fieldComesFromTheTableAsIs(dynamic _param_index, dynamic _param_tableName, dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic index = XVar.Clone(_param_index);
			dynamic tableName = XVar.Clone(_param_tableName);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic fieldListItem = null;
			fieldListItem = XVar.Clone(this.fieldList[index]);
			if(XVar.Pack(!(XVar)(MVCFunctions.is_object((XVar)(fieldListItem)))))
			{
				return false;
			}
			if(0 != MVCFunctions.strlen((XVar)(fieldListItem.alias)))
			{
				return false;
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.is_a((XVar)(fieldListItem.expression), new XVar("SQLField")))))
			{
				return false;
			}
			if((XVar)(MVCFunctions.strlen((XVar)(fieldListItem.expression.table)) != 0)  && (XVar)(fieldListItem.expression.table != tableName))
			{
				return false;
			}
			return 0 == MVCFunctions.strcasecmp((XVar)(fieldListItem.expression.name), (XVar)(field));
		}
	}
	public partial class SQLCountQuery : SQLQuery
	{
		protected static bool skipSQLCountQueryCtor = false;
		public SQLCountQuery(dynamic _param_src)
			:base((XVar)_param_src)
		{
			if(skipSQLCountQueryCtor)
			{
				skipSQLCountQueryCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic src = XVar.Clone(_param_src);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> v in src.GetEnumerator())
			{
				this.InitAndSetArrayItem(v.Value, v.Key);
			}
			this.headSql = new XVar("");
			if(XVar.Pack(!(XVar)(this.HasGroupBy())))
			{
				this.fieldList = XVar.Clone(XVar.Array());
			}
		}
		public virtual XVar toSql(dynamic _param_strwhere = null, dynamic _param_strorderby = null, dynamic _param_strhaving = null, dynamic _param_oneRecordMode = null, dynamic _param_joinFromPart = null)
		{
			#region default values
			if(_param_strwhere as Object == null) _param_strwhere = new XVar();
			if(_param_strorderby as Object == null) _param_strorderby = new XVar();
			if(_param_strhaving as Object == null) _param_strhaving = new XVar();
			if(_param_oneRecordMode as Object == null) _param_oneRecordMode = new XVar(false);
			if(_param_joinFromPart as Object == null) _param_joinFromPart = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic strwhere = XVar.Clone(_param_strwhere);
			dynamic strorderby = XVar.Clone(_param_strorderby);
			dynamic strhaving = XVar.Clone(_param_strhaving);
			dynamic oneRecordMode = XVar.Clone(_param_oneRecordMode);
			dynamic joinFromPart = XVar.Clone(_param_joinFromPart);
			#endregion

			dynamic ret = null, sql = null;
			sql = XVar.Clone(base.toSql((XVar)(strwhere)));
			if(XVar.Pack(this.HasGroupBy()))
			{
				ret = XVar.Clone(MVCFunctions.Concat("select count(*) from (", sql, ") a"));
			}
			else
			{
				ret = XVar.Clone(MVCFunctions.Concat("select count(*) from ", sql));
			}
			return ret;
		}
	}
	// Included file globals
	public partial class CommonFunctions
	{
		public static XVar sqlFromJson(dynamic _param_proto)
		{
			#region pass-by-value parameters
			dynamic proto = XVar.Clone(_param_proto);
			#endregion

			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(proto)))))
			{
				return proto;
			}
			if(XVar.Pack(!(XVar)(proto["type"])))
			{
				dynamic ret = XVar.Array();
				ret = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> obj in proto.GetEnumerator())
				{
					ret.InitAndSetArrayItem(CommonFunctions.sqlFromJson((XVar)(obj.Value)), null);
				}
				return ret;
			}
			switch(((XVar)proto["type"]).ToString())
			{
				case "FieldListItem":
					return new SQLFieldListItem((XVar)(proto));
				case "SQLField":
					return new SQLField((XVar)(proto));
				case "JoinOn":
					return new SQLJoinOn((XVar)(proto));
				case "FromListItem":
					return new SQLFromListItem((XVar)(proto));
				case "FunctionCall":
					return new SQLFunctionCall((XVar)(proto));
				case "GroupByListItem":
					return new SQLGroupByItem((XVar)(proto));
				case "LogicalExpression":
					return new SQLLogicalExpr((XVar)(proto));
				case "OrderByListItem":
					return new SQLOrderByItem((XVar)(proto));
				case "SQLQuery":
					return new SQLQuery((XVar)(proto));
				case "SQLTable":
					return new SQLTable((XVar)(proto));
				case "NonParsedEntity":
					return new SQLNonParsed((XVar)(proto));
				case "Entity":
					return new SQLEntity((XVar)(proto));
			}
			return null;
		}
	}
}

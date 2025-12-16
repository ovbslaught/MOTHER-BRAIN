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
	public partial class MySQLFunctions : DBFunctions
	{
		protected dynamic conn = XVar.Pack(null);
		protected static bool skipMySQLFunctionsCtor = false;
		public MySQLFunctions(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipMySQLFunctionsCtor)
			{
				skipMySQLFunctionsCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.strLeftWrapper = new XVar("`");
			this.strRightWrapper = new XVar("`");
			this.escapeChars.InitAndSetArrayItem(true, "\\");
			this.conn = XVar.Clone(var_params["conn"]);
		}
		public override XVar escapeLIKEpattern(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.str_replace((XVar)(new XVar(0, "\\", 1, "%", 2, "_")), (XVar)(new XVar(0, "\\\\", 1, "\\%", 2, "\\_")), (XVar)(str));
		}
		public override XVar addSlashes(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			if((XVar)(MVCFunctions.useMySQLiLib())  && (XVar)(this.conn))
			{
				if(XVar.Pack(this.conn))
				{
					return CommonFunctions.mysqli_real_escape_string((XVar)(this.conn), (XVar)(str));
				}
			}
			else
			{
				return MVCFunctions.str_replace((XVar)(new XVar(0, "\\", 1, "'")), (XVar)(new XVar(0, "\\\\", 1, "\\'")), (XVar)(str));
			}

			return null;
		}
		public override XVar addSlashesBinary(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(str)))))
			{
				return "''";
			}
			return MVCFunctions.Concat("0x", MVCFunctions.bin2hex((XVar)(str)));
		}
		public override XVar upper(dynamic _param_dbval)
		{
			#region pass-by-value parameters
			dynamic dbval = XVar.Clone(_param_dbval);
			#endregion

			return MVCFunctions.Concat("upper(", dbval, ")");
		}
		public override XVar field2char(dynamic _param_value, dynamic _param_type = null)
		{
			#region default values
			if(_param_type as Object == null) _param_type = new XVar(3);
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			return MVCFunctions.Concat("CAST( ", value, " AS CHAR )");
		}
		public override XVar field2time(dynamic _param_value, dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(var_type))))
			{
				return MVCFunctions.Concat("time(", value, ")");
			}
			return value;
		}
		public override XVar getInsertedIdSQL(dynamic _param_key = null, dynamic _param_table = null)
		{
			#region default values
			if(_param_key as Object == null) _param_key = new XVar();
			if(_param_table as Object == null) _param_table = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			dynamic table = XVar.Clone(_param_table);
			#endregion

			return "SELECT LAST_INSERT_ID()";
		}
		public override XVar timeToSecWrapper(dynamic _param_strName)
		{
			#region pass-by-value parameters
			dynamic strName = XVar.Clone(_param_strName);
			#endregion

			return MVCFunctions.Concat("TIME_TO_SEC(", this.addTableWrappers((XVar)(strName)), ")");
		}
		public override XVar schemaSupported()
		{
			return false;
		}
		public override XVar caseSensitiveComparison(dynamic _param_val1, dynamic _param_val2)
		{
			#region pass-by-value parameters
			dynamic val1 = XVar.Clone(_param_val1);
			dynamic val2 = XVar.Clone(_param_val2);
			#endregion

			return MVCFunctions.Concat("binary ", val1, " = ", val2);
		}
		public override XVar limitedQuery(dynamic _param_connection, dynamic _param_strSQL, dynamic _param_skip, dynamic _param_total, dynamic _param_applyLimit)
		{
			#region pass-by-value parameters
			dynamic connection = XVar.Clone(_param_connection);
			dynamic strSQL = XVar.Clone(_param_strSQL);
			dynamic skip = XVar.Clone(_param_skip);
			dynamic total = XVar.Clone(_param_total);
			dynamic applyLimit = XVar.Clone(_param_applyLimit);
			#endregion

			if((XVar)(applyLimit)  && (XVar)((XVar)(skip)  || (XVar)(XVar.Pack(0) < total)))
			{
				strSQL = MVCFunctions.Concat(strSQL, " limit ", skip, ", ", (XVar.Pack(XVar.Pack(0) <= total) ? XVar.Pack(total) : XVar.Pack(2000000000)));
			}
			return connection.query((XVar)(strSQL));
		}
		public override XVar intervalExpressionString(dynamic _param_expr, dynamic _param_interval)
		{
			#region pass-by-value parameters
			dynamic expr = XVar.Clone(_param_expr);
			dynamic interval = XVar.Clone(_param_interval);
			#endregion

			return DBFunctions.intervalExprLeft((XVar)(expr), (XVar)(interval));
		}
		public override XVar intervalExpressionNumber(dynamic _param_expr, dynamic _param_interval)
		{
			#region pass-by-value parameters
			dynamic expr = XVar.Clone(_param_expr);
			dynamic interval = XVar.Clone(_param_interval);
			#endregion

			return DBFunctions.intervalExprFloor((XVar)(expr), (XVar)(interval));
		}
		protected virtual XVar weekIntervalExpressionDate(dynamic _param_expr)
		{
			#region pass-by-value parameters
			dynamic expr = XVar.Clone(_param_expr);
			#endregion

			dynamic firstDayOfWeek = null, mode = null;
			firstDayOfWeek = XVar.Clone(GlobalVars.locale_info["LOCALE_IFIRSTDAYOFWEEK"]);
			mode = XVar.Clone(-1);
			if(firstDayOfWeek == XVar.Pack(0))
			{
				mode = new XVar(5);
			}
			else
			{
				if(firstDayOfWeek == 6)
				{
					mode = new XVar(0);
				}
			}
			if(mode != -1)
			{
				return MVCFunctions.Concat("year(", expr, ")*10000 + week(", expr, ",", mode, ")*100+01");
			}
			return MVCFunctions.Concat("year(", expr, ")*10000 + week(", expr, ")*100+01");
		}
		public override XVar intervalExpressionDate(dynamic _param_expr, dynamic _param_interval)
		{
			#region pass-by-value parameters
			dynamic expr = XVar.Clone(_param_expr);
			dynamic interval = XVar.Clone(_param_interval);
			#endregion

			if(interval == 1)
			{
				return MVCFunctions.Concat("year(", expr, ")*10000 + 101");
			}
			if(interval == 2)
			{
				return MVCFunctions.Concat("year(", expr, ")*10000 + QUARTER(", expr, ")*100+1");
			}
			if(interval == 3)
			{
				return MVCFunctions.Concat("year(", expr, ")*10000 + month(", expr, ")*100+1");
			}
			if(interval == 4)
			{
				return this.weekIntervalExpressionDate((XVar)(expr));
			}
			if(interval == 5)
			{
				return MVCFunctions.Concat("year(", expr, ")*10000 + month(", expr, ")*100 + day(", expr, ")");
			}
			if(interval == 6)
			{
				return MVCFunctions.Concat("year(", expr, ")*1000000 + month(", expr, ")*10000 + day(", expr, ")*100 + HOUR(", expr, ")");
			}
			if(interval == 7)
			{
				return MVCFunctions.Concat("year(", expr, ")*100000000 + month(", expr, ")*1000000 + day(", expr, ")*10000 + HOUR(", expr, ")*100 + minute(", expr, ")");
			}
			return expr;
		}
	}
}

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
	public partial class MSSQLFunctions : DBFunctions
	{
		protected static bool skipMSSQLFunctionsCtor = false;
		public MSSQLFunctions(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipMSSQLFunctionsCtor)
			{
				skipMSSQLFunctionsCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.strLeftWrapper = new XVar("[");
			this.strRightWrapper = new XVar("]");
		}
		public override XVar escapeLIKEpattern(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.str_replace((XVar)(new XVar(0, "[", 1, "%", 2, "_")), (XVar)(new XVar(0, "[[]", 1, "[%]", 2, "[_]")), (XVar)(str));
		}
		public override XVar prepareString(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.Concat("N'", this.addSlashes((XVar)(str)), "'");
		}
		public override XVar addSlashesBinary(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.Concat("0x", MVCFunctions.bin2hex((XVar)(str)));
		}
		public override XVar upper(dynamic _param_dbval)
		{
			#region pass-by-value parameters
			dynamic dbval = XVar.Clone(_param_dbval);
			#endregion

			return MVCFunctions.Concat("upper(", dbval, ")");
		}
		public override XVar addDateQuotes(dynamic _param_val)
		{
			#region pass-by-value parameters
			dynamic val = XVar.Clone(_param_val);
			#endregion

			if((XVar)(val == XVar.Pack(""))  || (XVar)(XVar.Equals(XVar.Pack(val), XVar.Pack(null))))
			{
				return "null";
			}
			return MVCFunctions.Concat("convert(datetime,'", this.addSlashes((XVar)(val)), "',120)");
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

			if(XVar.Pack(CommonFunctions.IsCharType((XVar)(var_type))))
			{
				return value;
			}
			if(XVar.Pack(!(XVar)(CommonFunctions.IsDateFieldType((XVar)(var_type)))))
			{
				return MVCFunctions.Concat("convert(varchar(250),", value, ")");
			}
			return MVCFunctions.Concat("convert(varchar(50),", value, ", 120)");
		}
		public override XVar field2time(dynamic _param_value, dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

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

			return "SELECT SCOPE_IDENTITY()";
		}
		public override XVar timeToSecWrapper(dynamic _param_strName)
		{
			#region pass-by-value parameters
			dynamic strName = XVar.Clone(_param_strName);
			#endregion

			dynamic wrappedFieldName = null;
			wrappedFieldName = XVar.Clone(this.addTableWrappers((XVar)(strName)));
			return MVCFunctions.Concat("(DATEPART(HOUR, ", wrappedFieldName, ") * 3600) + (DATEPART(MINUTE, ", wrappedFieldName, ") * 60) + (DATEPART(SECOND, ", wrappedFieldName, "))");
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

			dynamic qResult = null;
			if((XVar)(applyLimit)  && (XVar)(XVar.Pack(0) <= total))
			{
				strSQL = XVar.Clone(CommonFunctions.AddTop((XVar)(strSQL), (XVar)(skip + total)));
			}
			qResult = XVar.Clone(connection.query((XVar)(strSQL)));
			qResult.seekRecord((XVar)(skip));
			return qResult;
		}
		public override XVar caseSensitiveComparison(dynamic _param_val1, dynamic _param_val2)
		{
			#region pass-by-value parameters
			dynamic val1 = XVar.Clone(_param_val1);
			dynamic val2 = XVar.Clone(_param_val2);
			#endregion

			return MVCFunctions.Concat(val1, " = ", val2, " COLLATE SQL_Latin1_General_CP1_CS_AS");
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
		public override XVar intervalExpressionDate(dynamic _param_expr, dynamic _param_interval)
		{
			#region pass-by-value parameters
			dynamic expr = XVar.Clone(_param_expr);
			dynamic interval = XVar.Clone(_param_interval);
			#endregion

			if(interval == 1)
			{
				return MVCFunctions.Concat("convert(varchar(50), datepart(yyyy,", expr, ")*10000+0101)");
			}
			if(interval == 2)
			{
				return MVCFunctions.Concat("convert(varchar(50), datepart(yyyy,", expr, ")*10000 + datepart(qq,", expr, ")*100+1)");
			}
			if(interval == 3)
			{
				return MVCFunctions.Concat("convert(varchar(50), datepart(yyyy,", expr, ")*10000+datepart(mm,", expr, ")*100+1)");
			}
			if(interval == 4)
			{
				return MVCFunctions.Concat("convert(varchar(50), datepart(yyyy,", expr, ")*10000+(datepart(ww,", expr, ")-1)*100+01)");
			}
			if(interval == 5)
			{
				return MVCFunctions.Concat("convert(varchar(50), datepart(yyyy,", expr, ")*10000+datepart(mm,", expr, ")*100+datepart(dd,", expr, "))");
			}
			if(interval == 6)
			{
				return MVCFunctions.Concat("convert(varchar(50), convert(numeric(20,0), datepart(yyyy,", expr, "))*1000000+datepart(mm,", expr, ")*10000+datepart(dd,", expr, ")*100+datepart(hh,", expr, "))");
			}
			if(interval == 7)
			{
				return MVCFunctions.Concat("convert(varchar(50), convert(numeric(20,0), datepart(yyyy,", expr, "))*100000000+datepart(mm,", expr, ")*1000000+datepart(dd,", expr, ")*10000+datepart(hh,", expr, ")*100+datepart(mi,", expr, "))");
			}
			return expr;
		}
		public override XVar addSlashes(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.str_replace((XVar)(new XVar(0, "'", 1, MVCFunctions.chr(new XVar(0)))), (XVar)(new XVar(0, "''", 1, "\\u0000")), (XVar)(str));
		}
		public override XVar initializationSQL()
		{
			dynamic firstDayOfWeek = null;
			firstDayOfWeek = XVar.Clone(GlobalVars.locale_info["LOCALE_IFIRSTDAYOFWEEK"] + 1);
			return new XVar(0, MVCFunctions.Concat("SET DATEFIRST ", firstDayOfWeek));
		}
	}
}

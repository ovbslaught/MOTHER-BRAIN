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
	public partial class DB2Functions : DBFunctions
	{
		protected static bool skipDB2FunctionsCtor = false;
		public DB2Functions(dynamic _param_params) // proxy constructor
			:base((XVar)_param_params) {}

		public override XVar escapeLIKEpattern(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return str;
		}
		public override XVar addSlashes(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.str_replace(new XVar("'"), new XVar("''"), (XVar)(str));
		}
		public override XVar addSlashesBinary(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.Concat("blob(x'", MVCFunctions.bin2hex((XVar)(str)), "')");
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

			if(XVar.Pack(CommonFunctions.IsCharType((XVar)(var_type))))
			{
				return value;
			}
			return MVCFunctions.Concat("char(", value, ")");
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

			return "SELECT IDENTITY_VAL_LOCAL() FROM SYSIBM.SYSDUMMY1";
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
				strSQL = MVCFunctions.Concat(strSQL, " FETCH FIRST ", total + skip, " ROWS ONLY");
			}
			qResult = XVar.Clone(connection.query((XVar)(strSQL)));
			if(XVar.Pack(applyLimit))
			{
				qResult.seekRecord((XVar)(skip));
			}
			return qResult;
		}
		public override XVar intervalExpressionString(dynamic _param_expr, dynamic _param_interval)
		{
			#region pass-by-value parameters
			dynamic expr = XVar.Clone(_param_expr);
			dynamic interval = XVar.Clone(_param_interval);
			#endregion

			return DBFunctions.intervalExprSubstr((XVar)(expr), (XVar)(interval));
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
				return MVCFunctions.Concat("year(", expr, ")*10000+0101");
			}
			if(interval == 2)
			{
				return MVCFunctions.Concat("year(", expr, ")*10000+QUARTER(", expr, ")*100+1");
			}
			if(interval == 3)
			{
				return MVCFunctions.Concat("year(", expr, ")*10000+month(", expr, ")*100+1");
			}
			if(interval == 4)
			{
				return MVCFunctions.Concat("year(", expr, ")*10000+week(", expr, ")*100+01");
			}
			if(interval == 5)
			{
				return MVCFunctions.Concat("year(", expr, ")*10000+month(", expr, ")*100+day(", expr, ")");
			}
			if(interval == 6)
			{
				return MVCFunctions.Concat("year(", expr, ")*1000000+month(", expr, ")*10000+day(", expr, ")*100+HOUR(", expr, ")");
			}
			if(interval == 7)
			{
				return MVCFunctions.Concat("year(", expr, ")*100000000+month(", expr, ")*1000000+day(", expr, ")*10000+HOUR(", expr, ")*100+minute(", expr, ")");
			}
			return expr;
		}
	}
}

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
	public partial class PostgreFunctions : DBFunctions
	{
		protected dynamic postgreDbVersion;
		protected static bool skipPostgreFunctionsCtor = false;
		public PostgreFunctions(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipPostgreFunctionsCtor)
			{
				skipPostgreFunctionsCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic extraParams = XVar.Array();
			this.escapeChars.InitAndSetArrayItem(true, "\\");
			this.postgreDbVersion = XVar.Clone(extraParams["postgreDbVersion"]);
		}
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

			if(ProjectSettings.ext() == "php")
			{
				return CommonFunctions.pg_escape_string((XVar)(str));
			}
			return base.addSlashes((XVar)(str));
		}
		public override XVar addSlashesBinary(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			if(ProjectSettings.ext() == "php")
			{
				if(this.postgreDbVersion < 9)
				{
					return MVCFunctions.Concat("'", CommonFunctions.pg_escape_bytea((XVar)(str)), "'");
				}
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(str)))))
			{
				return "''";
			}
			return MVCFunctions.Concat("E'\\\\x", MVCFunctions.bin2hex((XVar)(str)), "'");
		}
		public override XVar stripSlashesBinary(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			if(ProjectSettings.ext() == "php")
			{
				if(this.postgreDbVersion < 9)
				{
					return CommonFunctions.pg_unescape_bytea((XVar)(str));
				}
			}
			return MVCFunctions.hex2bin((XVar)(MVCFunctions.substr((XVar)(str), new XVar(2))));
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
			return MVCFunctions.Concat("''||(", value, ")");
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

			return "SELECT LASTVAL()";
		}
		public override XVar crossDbSupported()
		{
			return false;
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

			if((XVar)(applyLimit)  && (XVar)((XVar)(skip)  || (XVar)(XVar.Pack(0) <= total)))
			{
				strSQL = MVCFunctions.Concat(strSQL, " limit ", (XVar.Pack(XVar.Pack(0) <= total) ? XVar.Pack(total) : XVar.Pack("ALL")), " offset ", skip);
			}
			return connection.query((XVar)(strSQL));
		}
		public override XVar intervalExpressionString(dynamic _param_expr, dynamic _param_interval)
		{
			#region pass-by-value parameters
			dynamic expr = XVar.Clone(_param_expr);
			dynamic interval = XVar.Clone(_param_interval);
			#endregion

			return DBFunctions.intervalExprSubstring((XVar)(expr), (XVar)(interval));
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
				return MVCFunctions.Concat("date_part('year',", expr, ")*10000+0101");
			}
			if(interval == 2)
			{
				return MVCFunctions.Concat("date_part('year',", expr, ")*10000+date_part('quarter',", expr, ")*100+1");
			}
			if(interval == 3)
			{
				return MVCFunctions.Concat("date_part('year',", expr, ")*10000+date_part('month',", expr, ")*100+1");
			}
			if(interval == 4)
			{
				return MVCFunctions.Concat("date_part('year',", expr, ")*10000+(date_part('week',", expr, ")-1)*100+01");
			}
			if(interval == 5)
			{
				return MVCFunctions.Concat("date_part('year',", expr, ")*10000+date_part('month',", expr, ")*100+date_part('days',", expr, ")");
			}
			if(interval == 6)
			{
				return MVCFunctions.Concat("date_part('year',", expr, ")*1000000+date_part('month',", expr, ")*10000+date_part('days',", expr, ")*100+date_part('hour',", expr, ")");
			}
			if(interval == 7)
			{
				return MVCFunctions.Concat("date_part('year',", expr, ")*100000000+date_part('month',", expr, ")*1000000+date_part('days',", expr, ")*10000+date_part('hour',", expr, ")*100+date_part('minute',", expr, ")");
			}
			return expr;
		}
	}
}

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
	public partial class ODBCFunctions : DBFunctions
	{
		protected static bool skipODBCFunctionsCtor = false;
		public ODBCFunctions(dynamic _param_params) // proxy constructor
			:base((XVar)_param_params) {}

		public override XVar escapeLIKEpattern(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return str;
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
		public override XVar stripSlashesBinary(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.db_stripslashesbinaryAccess((XVar)(str));
		}
		public override XVar addTableWrappers(dynamic _param_strName)
		{
			#region pass-by-value parameters
			dynamic strName = XVar.Clone(_param_strName);
			#endregion

			if(this.strLeftWrapper != "\"")
			{
				return this.addFieldWrappers((XVar)(strName));
			}
			return base.addTableWrappers((XVar)(strName));
		}
		public override XVar upper(dynamic _param_dbval)
		{
			#region pass-by-value parameters
			dynamic dbval = XVar.Clone(_param_dbval);
			#endregion

			return MVCFunctions.Concat("ucase(", dbval, ")");
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
			return MVCFunctions.Concat("#", this.addSlashes((XVar)(val)), "#");
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

			return value;
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

			return "SELECT @@IDENTITY";
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

			if(XVar.Pack(!(XVar)(interval)))
			{
				return expr;
			}
			return MVCFunctions.Concat("int( ", expr, " / ", interval, " ) * ", interval);
		}
		public override XVar intervalExpressionDate(dynamic _param_expr, dynamic _param_interval)
		{
			#region pass-by-value parameters
			dynamic expr = XVar.Clone(_param_expr);
			dynamic interval = XVar.Clone(_param_interval);
			#endregion

			if(interval == 1)
			{
				return MVCFunctions.Concat("Trim(datepart('yyyy',", expr, ")*10000+0101)");
			}
			if(interval == 2)
			{
				return MVCFunctions.Concat("Trim(datepart('yyyy',", expr, ")*10000+datepart('q',", expr, ")*100+1)");
			}
			if(interval == 3)
			{
				return MVCFunctions.Concat("Trim(datepart('yyyy',", expr, ")*10000+datepart('m',", expr, ")*100+1)");
			}
			if(interval == 4)
			{
				return MVCFunctions.Concat("Trim(datepart('yyyy',", expr, ")*10000+(datepart('ww',", expr, ")-1)*100+01)");
			}
			if(interval == 5)
			{
				return MVCFunctions.Concat("Trim(datepart('yyyy',", expr, ")*10000+datepart('m',", expr, ")*100+datepart('d',", expr, "))");
			}
			if(interval == 6)
			{
				return MVCFunctions.Concat("Trim(CVar(datepart('yyyy',", expr, "))*1000000+datepart('m',", expr, ")*10000+datepart('d',", expr, ")*100+datepart('h',", expr, "))");
			}
			if(interval == 7)
			{
				return MVCFunctions.Concat("Trim(CVar(datepart('yyyy',", expr, "))*100000000+CVar(datepart('m',", expr, "))*1000000+datepart('d',", expr, ")*10000+datepart('h',", expr, ")*100+datepart('n',", expr, "))");
			}
			return expr;
		}
	}
}

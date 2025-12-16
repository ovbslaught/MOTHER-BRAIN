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
	public partial class CommonFunctions
	{
		public static XVar db_connect()
		{
			return null;
		}
		public static XVar db_close(dynamic _param_conn)
		{
			#region pass-by-value parameters
			dynamic conn = _param_conn as Object == null ? new XVar() : (_param_conn).Clone();
			#endregion

			return null;
		}
		public static XVar db_query(dynamic _param_sql, dynamic _param_conn)
		{
			#region pass-by-value parameters
			dynamic sql = _param_sql as Object == null ? new XVar() : (_param_sql).Clone();
			dynamic conn = _param_conn as Object == null ? new XVar() : (_param_conn).Clone();
			#endregion

			dynamic connection = null;
			connection = CommonFunctions.getDefaultConnection();
			dynamic result = connection.query((XVar)(sql));
			if( result != null)
				return result.getQueryHandle();
			return null;
		}
		public static XVar db_exec(dynamic _param_sql, dynamic _param_conn)
		{
			#region pass-by-value parameters
			dynamic sql = _param_sql as Object == null ? new XVar() : (_param_sql).Clone();
			dynamic conn = _param_conn as Object == null ? new XVar() : (_param_conn).Clone();
			#endregion

			dynamic connection = null;
			connection = CommonFunctions.getDefaultConnection();
			connection.exec((XVar)(sql));
			return null;
		}
		public static XVar db_insertid(dynamic _param_qHandle)
		{
			#region pass-by-value parameters
			dynamic qHandle = _param_qHandle as Object == null ? new XVar() : (_param_qHandle).Clone();
			#endregion

			dynamic connection = null;
			connection = CommonFunctions.getDefaultConnection();
			return connection.getInsertedId();
		}
		public static XVar db_fetch_array(dynamic _param_qHandle)
		{
			#region pass-by-value parameters
			dynamic qHandle = _param_qHandle as Object == null ? new XVar() : (_param_qHandle).Clone();
			#endregion

			dynamic connection = null;
			connection = CommonFunctions.getDefaultConnection();
			return connection.fetch_array((XVar)(qHandle));
		}
		public static XVar db_fetch_numarray(dynamic _param_qHandle)
		{
			#region pass-by-value parameters
			dynamic qHandle = _param_qHandle as Object == null ? new XVar() : (_param_qHandle).Clone();
			#endregion

			dynamic connection = null;
			connection = CommonFunctions.getDefaultConnection();
			return connection.fetch_numarray((XVar)(qHandle));
		}
		public static XVar db_prepare_string(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = _param_str as Object == null ? new XVar() : (_param_str).Clone();
			#endregion

			dynamic connection = null;
			connection = CommonFunctions.getDefaultConnection();
			return connection.prepareString((XVar)(str));
		}
		public static XVar db_addslashes(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = _param_str as Object == null ? new XVar() : (_param_str).Clone();
			#endregion

			dynamic connection = null;
			connection = CommonFunctions.getDefaultConnection();
			return connection.addSlashes((XVar)(str));
		}
		public static XVar AddFieldWrappers(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = _param_str as Object == null ? new XVar() : (_param_str).Clone();
			#endregion

			dynamic connection = null;
			connection = CommonFunctions.getDefaultConnection();
			return connection.addFieldWrappers((XVar)(str));
		}
		public static XVar AddTableWrappers(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = _param_str as Object == null ? new XVar() : (_param_str).Clone();
			#endregion

			dynamic connection = null;
			connection = CommonFunctions.getDefaultConnection();
			return connection.addTableWrappers((XVar)(str));
		}
		public static XVar db_upper(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = _param_value as Object == null ? new XVar() : (_param_value).Clone();
			#endregion

			dynamic connection = null;
			connection = CommonFunctions.getDefaultConnection();
			return connection.upper((XVar)(value));
		}
		public static XVar db_datequotes(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = _param_value as Object == null ? new XVar() : (_param_value).Clone();
			#endregion

			dynamic connection = null;
			connection = CommonFunctions.getDefaultConnection();
			return connection.addDateQuotes((XVar)(value));
		}
		public static XVar GetDatabaseType(dynamic _param_connection = null)
		{
			#region default values
			if(_param_connection as Object == null) _param_connection = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic connection = _param_connection as Object == null ? new XVar() : (_param_connection).Clone();
			#endregion

			if(XVar.Pack(connection == null))
			{
				connection = CommonFunctions.getDefaultConnection();
			}
			return connection.dbType;
		}
	}
}

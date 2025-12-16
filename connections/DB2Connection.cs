using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data.Common;
using runnerDotNet;

namespace runnerDotNet
{
    public class DB2Connection : Connection
    {
        protected bool useOleDb = false;
        public DB2Connection(XVar parameters) : base(parameters)
        {
            string firstClause = GlobalVars.ConnectionStrings[parameters["connId"]].ToString().Substring(0, 9).ToUpper();
            useOleDb = firstClause == "PROVIDER=";
        }

        protected override ConnectionsPool GetConnectionsPool(string connStr)
        {
             string firstClause = connStr.Substring(0, 9).ToUpper();
            if (firstClause == "PROVIDER=")
                return new OLEDBConnectionPool(connStr);
            else
                return new ODBCConnectionPool(connStr);
        }

        protected override DbCommand GetCommand()
        {
            if (useOleDb)
                return new System.Data.OleDb.OleDbCommand();
            else
                return new System.Data.Odbc.OdbcCommand();
        }

        protected DbParameter GetBlobParameter(string name)
        {
            if (useOleDb)
                return new System.Data.OleDb.OleDbParameter(name, System.Data.DbType.Binary);
            else
                return new System.Data.Odbc.OdbcParameter(name, System.Data.DbType.Binary);
        }

        public override XVar execWithBlobProcessing(XVar strSQL, XVar blobs, XVar blobTypes = null, XVar autoincField = null)
        {
            try
            {
                DbConnection connection = connectionsPool.FreeConnection;
                if (connection.State != System.Data.ConnectionState.Open)
                {
					connection.Open();
					if( initializingSQL != null )
					{
						querySingle( initializingSQL, connection, false );
					}
				}
                DbCommand cmd = GetCommand();
                cmd.Connection = connection;
                cmd.CommandText = strSQL;

                if (MVCFunctions.count(blobs) != 0)
                {
                    int idx = 1;
                    foreach (KeyValuePair<XVar, dynamic> value in blobs.GetEnumerator())
                    {
                        DbParameter parameter = GetBlobParameter(MVCFunctions.Concat("@bnd", idx));
                        parameter.Direction = System.Data.ParameterDirection.Input;
                        parameter.Size = (value.Value.Value as byte[]).Length;
                        parameter.Value = value.Value.Value;
                        cmd.Parameters.Add(parameter);
                        idx++;
                    }
                }

                CommonFunctions.LogInfo(strSQL);
                cmd.ExecuteNonQuery();

                bool hasInsertQuery = strSQL.Value.ToString().Substring(0,6).ToLower( System.Globalization.CultureInfo.InvariantCulture ) == "insert";
                if( hasInsertQuery ) {
					CalculateLastInsertedId( connection );
				}
            }
            catch (Exception e)
            {
                GlobalVars.LastDBError = e.Message;
                if (!silentMode)
                {
                    throw e;
                }
                return false;
            }
            return true;
        }
    }
}
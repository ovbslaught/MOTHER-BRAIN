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
	abstract public class Connection : XClass
	{
		protected ConnectionsPool connectionsPool;

		protected XVar lastInsertedID = new XVar(0);

		// The database type identifier
		public XVar dbType;

		// The db connection id
		public XVar connId;

		public XVar conn = null; // DO NOT USE. should be eliminated in php

		public XVar _encryptInfo;

		protected DBFunctions _functions;

		protected DBInfo _info;

		abstract protected DbCommand GetCommand();

		abstract protected ConnectionsPool GetConnectionsPool(string connStr);

		protected string initializingSQL = null;

		protected XVar silentMode;

		public dynamic escapeCharts = XVar.Array();

		protected string[] stopCommandList = {"insert", "update", "delete", "create", "drop", "rename", "alter", "set" };


		public Connection(XVar parameters)
		{
			assignConnectionParams(parameters);

			// set the db connection
			connect();

			InitDbFunctions(parameters["leftWrap"], parameters["rightWrap"]);
			InitDbInfo();
			this._encryptInfo = ProjectSettings.encryptSettings( connId );
			this.escapeCharts = new XVar(0,"'");

		}

		public XVar isEncryptionByPHPEnabled()
		{
			return new XVar( _encryptInfo["encryptMethod"] == Constants.ENCRYPTION_PHP );
		}

		/**
		 * Set db connection's properties
		 * @param Array parameters
		 */
		protected XVar assignConnectionParams(XVar parameters)
		{
			dbType = parameters["dbType"];
			connId = parameters["connId"];
			return null;
		}

		/**
		 * Set the DBpublic XVar object
		 * @param String leftWrapper
		 * @param String rightWrapper
		 */
		private XVar InitDbFunctions(XVar leftWrapper, XVar rightWrapper)
		{
			XVar extraParams = getDbFunctionsExtraParams();
			if(!leftWrapper.IsEmpty() && !rightWrapper.IsEmpty())
			{
				extraParams.InitAndSetArrayItem(leftWrapper, "leftWrap");
				extraParams.InitAndSetArrayItem(rightWrapper, "rightWrap");
			}
			_functions = GetDbFunctions(extraParams);
			return null;
		}

		protected virtual DBInfo GetDbInfo()
		{
			switch((int)dbType)
			{
				case Constants.nDATABASE_SQLite3:
					return new SQLite3Info(this);
				default:
				{
					String firstClause = GlobalVars.ConnectionStrings[connId].ToString().Substring(0,9).ToUpper();
					if(  firstClause == "PROVIDER=" )
						return new OLEDBInfo(this);
					else
						return new ODBCInfo(this);
				}
			}
		}

		protected virtual DBFunctions GetDbFunctions( XVar extraParams)
		{
			switch((int)dbType)
			{
				case Constants.nDATABASE_Access:
					extraParams["leftWrap"] = "[";
					extraParams["rightWrap"] = "]";
					return new ODBCFunctions( extraParams);
				case Constants.nDATABASE_SQLite3:
					return new SQLite3Functions( extraParams);
				default:
					return new DBFunctions( extraParams);
			}
		}

		/**
		 * Get extra connection parameters that may be connected
		 * with the db connection link directly
		 * @return Array
		 */
		protected XVar getDbFunctionsExtraParams()
		{
			return XVar.Array();
		}

		/**
		 * Set the DbInfo object
		 * @param String ODBCString
		 */
		private XVar InitDbInfo()
		{
			_info = GetDbInfo();
			return null;
		}


		/**
		 * An interface stub.
		 * Open a connection to db
		 */
		public XVar connect()
		{
			// db_connect
			try
			{
				connectionsPool =  GetConnectionsPool(GlobalVars.ConnectionStrings[connId]);
				XVar connect = new XVar(connectionsPool.FreeConnection);
				return connect;
			}
			catch(Exception e)
			{
				if( !silentMode )
				{
					throw e;
				}
				return null;
			}
		}

		/**
		 * An interface stub
		 * Close the db connection
		 */
		public XVar close()
		{
			// db_close
			connectionsPool.CloseConnections();
			return null;
		}

		public QueryResult query(XVar sql ) {
			return _query( sql, true );
		}

		protected QueryResult querySingle( XVar sql, DbConnection connection, bool resultNeeded ) {
			if (GlobalVars.dDebug)
                MVCFunctions.EchoToOutput(sql.ToString() + "<br />");
			GlobalVars.strLastSQL = sql;
			DbCommand cmd = GetCommand();
			cmd.Connection = connection;
			cmd.CommandText = sql;
            string commandStr = sql.Substring(0, 6).ToLower();
			if( stopCommandList.Any(x => commandStr.Substring(0, x.Length ) == x ) ) {
				resultNeeded = false;
			}
			if( resultNeeded ) {
				RunnerDBReader rdr = cmd.ExecuteReader();
				rdr.Connection = cmd.Connection;
				return new QueryResult(this, rdr);
			} else {
				cmd.ExecuteNonQuery();
				return new QueryResult(this, null);
			}
		}

		public QueryResult queryMultiple( XVar sqlStrings, bool resultNeeded ) {

			DbConnection connection = connectionsPool.FreeConnection;
            try {

				//	?????
				if (connection.State != System.Data.ConnectionState.Open) {
					connection.Open();
					XVar initSQLList = this._functions.initializationSQL();
					if( initializingSQL != null )
					{
						initSQLList.Add(initializingSQL);
					}

					foreach( KeyValuePair<XVar, dynamic> sql in initSQLList.GetEnumerator() ) {
						querySingle( sql.Value, connection, false );
					}

				}

				bool hasInsertQuery = false;

				//	run SQL
				QueryResult ret = null;
				foreach (KeyValuePair<XVar, dynamic> sql in sqlStrings.GetEnumerator() ) {
					ret = querySingle( sql.Value, connection, resultNeeded );
					if( !hasInsertQuery && !resultNeeded && sql.Value.ToString().Substring(0,6).ToLower( System.Globalization.CultureInfo.InvariantCulture ) == "insert" ) {
						hasInsertQuery = true;
					}
				}

				if( hasInsertQuery ) {
					CalculateLastInsertedId( connection );
				}


				if( !resultNeeded ) {
					connection.Close();
				}

				return ret;
			}
            catch(Exception e)
            {
                GlobalVars.LastDBError = e.Message;
				if ( connection != null)
					connection.Close();

				if( !silentMode )
				{
					throw e;
				}
				return null;
            }

		}

		public QueryResult _query(XVar sql, bool resultNeeded ) {
			return queryMultiple( new XVar( 0, sql ), resultNeeded );
		}

		public bool execMultiple( XVar sqls ) {

			XVar silent = setSilentMode( true );
			QueryResult ret = queryMultiple( sqls, false );
			setSilentMode( silent );
			return ret != null;
		}


		virtual protected void CalculateLastInsertedId( DbConnection connection )
		{
			lastInsertedID = "";
			try
			{
				DbCommand cmd = GetCommand();
				cmd.Connection = connection;
				cmd.CommandText = GetLastInsertedIdSql();
				RunnerDBReader rdr = cmd.ExecuteReader();
				rdr.Connection = connection;
				if (rdr.Read())
				{
					lastInsertedID = new XVar(rdr[0]);
				}
			}
			catch(Exception e) { }
		}

		virtual protected string GetLastInsertedIdSql()
		{
			return _functions.getInsertedIdSQL();
		}

		public void setInitializingSQL(XVar sql)
		{
			initializingSQL = sql;
		}

		/**
		 * An interface stub
		 * Execute an SQL query
		 * @param String sql
		 */
		public QueryResult exec(XVar sql)
		{
            return _query(sql, false );
		}

		/**
		 * An interface stub
		 * Get a description of the last error
		 * @return String
		 */
		public XVar lastError()
		{
			//db_error
			return GlobalVars.LastDBError;
		}

		/**
		 * An interface stub
		 * Get the auto generated id used in the last query
		 * @return Number
		 */
		public XVar getInsertedId(XVar key = null, XVar table = null)
		{
			//db_insertid
			return lastInsertedID;
		}

		/**
		 * The stub openSchema method overrided in the ADO connection class
		 * @param Number querytype
		 * @return Mixed
		 */
		public XVar openSchema(XVar querytype)
		{
			return null;
		}

		/**
		 * An interface stub
		 * Fetch a result row as an associative array
		 * @param Mixed qHanle		The query handle
		 * @return Array
		 */
		public XVar fetch_array(dynamic qHanle)
		{
			//db_fetch_array
			if (qHanle != null)
            {
                RunnerDBReader reader = (RunnerDBReader)qHanle;
                if (reader.Read())
                {
                    XVar result = new XVar();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        result.SetArrayItem(reader.GetName(i), reader[i]);
                    }
                    return result;
                }
            }

            return XVar.Array();
		}

		/**
		 * An interface stub
		 * Fetch a result row as a numeric array
		 * @param Mixed qHanle		The query handle
		 * @return Array
		 */
		public XVar fetch_numarray(dynamic qHanle)
		{
			//db_fetch_numarray
			if (qHanle != null)
            {
                RunnerDBReader reader = (RunnerDBReader)qHanle;
                if (reader.Read())
                {
                    XVar result = new XVar();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        result.SetArrayItem(i, reader[i]);
                    }
                    return result;
                }
            }

            return XVar.Array();
		}

		/**
		 * An interface stub
		 * Free resources associated with a query result set
		 * @param Mixed qHanle		The query handle
		 */
		public XVar closeQuery(dynamic qHanle)
		{
			//db_closequery
			((RunnerDBReader)qHanle).Close();
			return null;
		}

		/**
		 * An interface stub.
		 * Get number of fields in a result
		 * @param Mixed qHanle		The query handle
		 * @return Number
		 */
		public XVar num_fields(XVar qHandle)
		{
			//db_numfields
			return (qHandle as RunnerDBReader).FieldCount;
		}

		/**
		 * An interface stub.
		 * Get the name of the specified field in a result
		 * @param Mixed qHanle		The query handle
		 * @param Number offset
		 * @return String
		 */
		public XVar field_name(XVar qHandle, XVar offset)
		{
			//db_fieldname
			return (qHandle as RunnerDBReader).GetName(offset);
		}

        /**
		 * An interface stub
		 * @param Mixed qHandle
		 * @param Number pageSize
		 * @param Number page
		 */
        public XVar seekRecord(XVar qHandle, XVar n)
        {
            if (qHandle != null)
            {
                RunnerDBReader reader = qHandle as RunnerDBReader;
                for (int i = 0; i < n; i++)
                {
                    if (!reader.Read())
                    {
                        return null;
                    }
                }
            }
            return null;
        }

		/**
		 * @param String str
		 * @return String
		 */
		public XVar escapeLIKEpattern(XVar str)
		{
			return _functions.escapeLIKEpattern(str);
		}


		public XVar getTableNameComponents(XVar name)
		{
			return _functions.getTableNameComponents(name);
		}

		/**
		 * @param String str
		 * @return String
		 */
		public XVar prepareString(XVar str)
		{
			return _functions.prepareString(str);
		}

		/**
		 * @param String str
		 * @return Boolean
		 */
		public XVar positionQuoted(XVar str, XVar pos)
		{
			return _functions.positionQuoted(str, pos);
		}


		/**
		 * @param String str
		 * @return String
		 */
		public XVar addSlashes(XVar str)
		{
			return _functions.addSlashes(str);
		}

		/**
		 * @param String str
		 * @return String
		 */
		public XVar addSlashesBinary(XVar str)
		{
			return _functions.addSlashesBinary(str);
		}

		/**
		 * @param String str
		 * @return String
		 */
		public XVar stripSlashesBinary(XVar str)
		{
			return _functions.stripSlashesBinary(str);
		}

		/**
		 * @param String fName
		 * @return String
		 */
		public XVar addFieldWrappers(XVar fName)
		{
			return _functions.addFieldWrappers(fName);
		}

		/**
		 * @param String tName
		 * @return String
		 */
		public XVar addTableWrappers(XVar tName)
		{
			return _functions.addTableWrappers(tName);
		}

		/**
		 * @param String str
		 * @return String
		 */
		public XVar upper(XVar str)
		{
			return _functions.upper(str);
		}

		/**
		 * @param Mixed value
		 * @return String
		 */
		public XVar addDateQuotes(XVar value)
		{
			return _functions.addDateQuotes(value);
		}

		/**
		 * @param Mixed value
		 * @param Number type ( optional )
		 * @return String
		 */
		public XVar field2char(XVar value, XVar type = null)
		{
			if(type as object == null)
				type = 3;

			return _functions.field2char(value, type);
		}

		/**
		 * It's invoked when search is running on the 'View as:Time' field
		 * @param Mixed value
		 * @param Number type
		 * @return String
		 */
		public XVar field2time(XVar value, XVar type)
		{
			return _functions.field2time(value, type);
		}
		

		/**
		 * @param String tName
		 * @return String
		 */	
		public XVar timeToSecWrapper( XVar tName )
		{
			return _functions.timeToSecWrapper( tName );
		}		

		/**
		 * @return Array
		 */
		public XVar getTableList()
		{
			return _info.db_gettablelist();
		}

		/**
		 * @param String
		 * @return Array
		 */
		public XVar getFieldsList(XVar sql)
		{
			return _info.db_getfieldslist( sql );
		}

		/**
		 * Check if the db supports subqueries
		 * @return Boolean
		 */
		public XVar checkDBSubqueriesSupport()
		{
			return true;
		}

		/**
		 * @param String sql
		 * @param Number pageStart
		 * @param Number pageSize
		 * @param Boolean applyLimit
		 */
		public virtual XVar queryPage(XVar strSQL, XVar pageStart, XVar pageSize, XVar applyLimit)
		{
			return limitedQuery( strSQL, (pageStart-1)*pageSize, pageSize, applyLimit );
		}


		public virtual XVar limitedQuery(XVar strSQL, XVar skip, XVar total, XVar applyLimit)
		{
			return _functions.limitedQuery( this, strSQL, skip, total, applyLimit );
		}

		/**
		 * Execute an SQL query with blob fields processing
		 * @param String sql
		 * @param Array blobs
		 * @param Array blobTypes
		 */
		public virtual XVar execWithBlobProcessing(XVar sql, XVar blobs, XVar blobTypes = null, XVar autoincField = null )
		{
			return exec( sql );
		}

		/**
		 * Get the number of rows fetched by an SQL query
		 * @param String sql	A part of an SQL query or a full SQL query
		 * @param Boolean  		The flag indicating if the full SQL query (that can be used as a subquery)
		 * or the part of an sql query ( from + where clauses ) is passed as the first param
		 */
		public XVar getFetchedRowsNumber(XVar sql)
		{
			string countSql = "select count(*) from (" + sql.ToString() + ") a";

			XVar countdata = query( countSql ).fetchNumeric();
			return countdata[0];
		}

		/**
		 * Check if SQL queries containing joined subquery are optimized
		 * @return Boolean
		 */
		public XVar checkIfJoinSubqueriesOptimized()
		{
			return true;
		}

		virtual public void db_multipleInsertQuery(XVar qstringArray, XVar table = null, XVar isIdentityOffNeeded = null)
        {
            DbConnection connection = connectionsPool.FreeConnection;
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            DbCommand cmd = GetCommand();
            cmd.Connection = connection;

            foreach (var qstring in qstringArray.GetEnumerator())
            {
                if (GlobalVars.dDebug)
                    MVCFunctions.EchoToOutput(qstring.Value.ToString() + "<br />");

                GlobalVars.strLastSQL = qstring.Value;
                cmd.CommandText = qstring.Value.ToString();
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }

		/*
		 *	Enables or disables Silent Mode, when no SQL errors are displayed.
		 *	@param 	Boolean silent
		 *  @return Boolean - previous Silent mode
		*/
		public XVar setSilentMode( XVar silent )
		{
			XVar oldMode = silentMode;
			silentMode = silent;
			return oldMode;
		}

		public QueryResult querySilent( XVar sql )
		{
			XVar silent = setSilentMode( true );
			QueryResult ret = query( sql );

			setSilentMode( silent );
			return ret;
		}

		public QueryResult execSilent( XVar sql )
		{
			XVar silent = setSilentMode( true );
			QueryResult ret = exec( sql );

			setSilentMode( silent );
			return ret;
		}

		public XVar comparisonSQL( XVar val1, XVar val2, XVar ignoreCase ) {
			return ignoreCase
				? this.caseInsensitiveComparison( val1, val2 )
				: this.caseSensitiveComparison( val1, val2 );
		}


		public XVar caseSensitiveComparison( XVar val1, XVar val2 )
		{
			return this._functions.caseSensitiveComparison( val1, val2 );
		}

		public XVar caseInsensitiveComparison( XVar val1, XVar val2 )
		{
			return this._functions.caseInsensitiveComparison( val1, val2 );
		}

		public virtual XVar execSilentWithBlobProcessing( XVar sql, XVar blobs, XVar blobTypes, XVar autoincField = null ) {
			XVar silent = setSilentMode( true );
			XVar ret = execWithBlobProcessing( sql, blobs, blobTypes, autoincField );
			setSilentMode( silent );
			return ret;
		}

		public virtual XVar intervalExpressionDate( XVar expr, XVar interval )
		{
			return this._functions.intervalExpressionDate( expr, interval );
		}

		public virtual XVar intervalExpressionString( XVar expr, XVar interval )
		{
			return this._functions.intervalExpressionString( expr, interval );
		}

		public virtual XVar intervalExpressionNumber( XVar expr, XVar interval )
		{
			return this._functions.intervalExpressionNumber( expr, interval );
		}

		public virtual XVar dbBased()
		{
			return true;
		}

        public static XVar getAutoincField(XVar tableInfo)
        {
            if (XVar.Pack(!(XVar)(tableInfo)))
                return null;

            if (!MVCFunctions.is_array((XVar)tableInfo["fields"]))
                return null;

            XVar keys = XVar.Array();
            XVar autoincField = null;
            foreach (KeyValuePair<XVar, dynamic> f in ((XVar)tableInfo["fields"]).GetEnumerator())
            {
                if (XVar.Pack(f.Value["key"]))
                {
                    keys.InitAndSetArrayItem(f.Value["type"], f.Value["name"]);
                    if (XVar.Pack(f.Value["autoInc"]))
                    {
                        autoincField = XVar.Clone(f.Value["name"]);
                        break;
                    }
                }
            }
            if (autoincField == null && MVCFunctions.count(keys) == 1)
            {
                foreach (KeyValuePair<XVar, dynamic> var_type in keys.GetEnumerator())
                {
                    if (CommonFunctions.IsNumberType((XVar)(var_type.Value)))
                    {
                        autoincField = XVar.Clone(var_type.Key);
                    }
                }
            }
            return autoincField;
        }

		public virtual string getVersion()
		{
			return "";
		}

	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.SQLite;

namespace runnerDotNet
{
    abstract public class ConnectionsPool
    {
        protected List<DbConnection> m_connections = new List<DbConnection>();
        protected string m_connectionString = "";

        public ConnectionsPool(string connectionString)
        {
            m_connectionString = connectionString;
        }

        public virtual DbConnection FreeConnection
        {
            get
            {
                foreach (var connection in m_connections)
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        return connection;
                    }
                }
				
				DbConnection newConnection = GetConnection();
				m_connections.Add(newConnection);
                return newConnection;
			}
		}
		
		abstract protected DbConnection GetConnection();

		public void CloseConnections()
		{
			foreach (var connection in m_connections)
			{
				if (connection.State != System.Data.ConnectionState.Closed)
				{
					connection.Close();
				}
			}
		}
    }
	
	public class SQLiteConnectionPool : ConnectionsPool
	{
		public SQLiteConnectionPool(string connectionString) : base(connectionString) { }
		
		protected override DbConnection GetConnection()
		{
			return new System.Data.SQLite.SQLiteConnection(m_connectionString);
		}
	}
	public class ODBCConnectionPool : ConnectionsPool
	{
		public ODBCConnectionPool(string connectionString) : base(connectionString) { }
		
		protected override DbConnection GetConnection()
		{
			return new System.Data.Odbc.OdbcConnection(m_connectionString);
		}
	}
	public class OLEDBConnectionPool : ConnectionsPool
	{
		public OLEDBConnectionPool(string connectionString) : base(connectionString) { }
		
		protected override DbConnection GetConnection()
		{
			return new System.Data.OleDb.OleDbConnection(m_connectionString);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Common;

namespace runnerDotNet
{
    public class RunnerDBReader : XClass
    {
		private int ConnectionType;
        private XVar buffer = XVar.Array();
        private DbDataReader dbReader;
        private XVar fieldsNames = XVar.Array();
        private Dictionary<int, Type> fieldsTypes = new Dictionary<int,Type>();

        private bool isFirstRecordReaded = false;

        public DbConnection Connection { get; set; }
        public bool IsLastRecordReaded { get; set; }
        public int FieldCount
        {
            get
            {
                return fieldsNames.Count();
            }
        }

        public RunnerDBReader(DbDataReader reader)
        {
            this.dbReader = reader;
            IsLastRecordReaded = false;
            for (int i = 0; i < this.dbReader.FieldCount; i++)
            {
                fieldsNames.SetArrayItem(i, this.dbReader.GetName(i));
                fieldsTypes[i] = this.dbReader.GetFieldType(i);
            }
			
			if(dbReader is System.Data.OleDb.OleDbDataReader)
				ConnectionType = Constants.nDATABASE_Access;
			if(dbReader is System.Data.SQLite.SQLiteDataReader)
				ConnectionType = Constants.nDATABASE_SQLite3;
        }

        public static implicit operator RunnerDBReader(DbDataReader reader)
        {
            return new RunnerDBReader(reader);
        }

        public object this[XVar index]
        {
            get
            {
                if (buffer.KeyExists(index))
                {
                    return buffer[index];
                }
                return null;
            }
        }

        public XVar GetName(XVar index)
        {
            return fieldsNames[index];
        }

		// ado types emulation. usec only in webreports for non-project tables
        public XVar GetFieldType(XVar index)
		{
			if (fieldsTypes[index] == typeof(TimeSpan))
				return 134;

			if (fieldsTypes[index].IsArray)
				return 128;

			if (fieldsTypes[index] == typeof(Guid))
				return 72;

			switch (Type.GetTypeCode(fieldsTypes[index]))
			{
				case TypeCode.Object:
					return 128;

				case TypeCode.DBNull:
					return 200;
				case TypeCode.Boolean:
					return 11;
				case TypeCode.Char:
					return 129;
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
					return 17;
				case TypeCode.Int32:
				case TypeCode.UInt32:
					return 3;
				case TypeCode.Int64:
				case TypeCode.UInt64:
					return 3;
				case TypeCode.Single:
				case TypeCode.Double:
					return 5;
				case TypeCode.Decimal:
					return 14;
				case TypeCode.DateTime:
					return 135;
				case TypeCode.String:
					return 200;
			}

			return 0;
		}

        public void Close()
        {
            this.dbReader.Close();
        }

        public bool Read()
        {
            bool result = false;
            if (buffer != null)
            {
                if (!IsLastRecordReaded || !dbReader.IsClosed)
                {
                    if (!isFirstRecordReaded)
                    {
                        if (!dbReader.Read())
                        {
                            if (!isFirstRecordReaded)
                            {
                                dbReader.Close();
                                Connection.Close();
                            }
                            IsLastRecordReaded = true;
                            buffer = null;
                            return false;
                        }
                        isFirstRecordReaded = true;
                    }
                    buffer.RemoveAll();
                    for (int i = 0; i < dbReader.FieldCount; i++)
                    {
						object value = null;
						if(ConnectionType == Constants.nDATABASE_Oracle)
						{
						}
						else
						{
							value = dbReader[i];
							//if(value is DBNull)
							//	value = null;
						}


						if(ConnectionType == Constants.nDATABASE_MSSQLServer) {
							if (GetFieldType(i) == 11 && value.Equals( false ) ) {
                                value = 0;
                            }
						}

                        buffer.SetArrayItem(i, value);
                        buffer.SetArrayItem(dbReader.GetName(i), value);
                    }
                    if (!dbReader.IsClosed && !dbReader.Read())
                    {
                        IsLastRecordReaded = true;
                        dbReader.Close();
                        Connection.Close();
                    }
                    result = true;
                }
                else
                {
                    buffer = null;
                }
            }
            return result;
        }
    }
}
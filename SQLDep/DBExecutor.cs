using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Data.OleDb;



namespace SQLDep
{
    /// <summary>
    /// vse co jde do databaze by melo jit pres tuto tridu
    /// </summary>
    public class DBExecutor
    {

        private enum UseDriver
        {
            UNDEF_TYPE = 0,
            ODBC = 1,
            OLEDB = 2
        };

        public DBExecutor ()
        {
            MyDriver = DBExecutor.UseDriver.UNDEF_TYPE;
        }

        private UseDriver MyDriver { get; set; }
        public string ConnectString { get; set; }

        private OdbcConnection ODBCConnection { get; set; }

        private OleDbConnection OleDbConnection { get; set; }

        public void BuildConnectionString(string dbType, string auth_type, string server, string database, string loginName, string loginpassword)
        {
            string ret = string.Empty;

            // native support
            if (dbType == "oracle")
            {
                this.MyDriver = DBExecutor.UseDriver.OLEDB;
                ret = "provider = OraOLEDB.Oracle; data source = " + database + ";user id=" + loginName + ";password=" + loginpassword;
                this.ConnectString = ret;
                return;
            }

            // others
            List<string> drivers = ODBCUtils.GetSystemDriverList();
            string driverName = string.Empty;
            switch (dbType)
            {
                case "oracle":
                    {
                        driverName = drivers.Where(x => x.IndexOf("Oracle") >= 0).FirstOrDefault();
                        break;
                    }
                case "mssql":
                default:
                    {
                        driverName = drivers.Where(x => x.IndexOf("SQL Server") >= 0).FirstOrDefault();
                        break;
                    }
            }
            if (string.IsNullOrEmpty(driverName))
            {
                throw new Exception("No ODBC driver found, please install and try again");
            }
            else
            {
                ret += "Driver={" + driverName + "};";
            }
            ret += "Server=" + server + ";";
            ret += "Database=" + database + ";";

            switch (auth_type)
            {
                case "win_auth":
                {
                    ret += "Authentication=Windows Authentication;";
                    break;
                }
                default:
                case "sql_auth":
                {
                    ret += "UID=" + loginName + ";";
                    ret += "PWD=" + loginpassword + ";";
                    break;
                }
            }
            this.MyDriver = DBExecutor.UseDriver.ODBC;
            this.ConnectString = ret;
        }

        public void Connect ()
        {
            if (this.MyDriver == UseDriver.ODBC)
            {
                OdbcConnection connection = new OdbcConnection(this.ConnectString);
                connection.Open();
                this.ODBCConnection = connection;
            }
            else if (this.MyDriver == UseDriver.OLEDB)
            {
                OleDbConnection connection = new OleDbConnection(this.ConnectString);
                connection.Open();
                this.OleDbConnection = connection;
            }
        }
        public void Close()
        {
            if (this.MyDriver == UseDriver.ODBC)
            {
                this.ODBCConnection.Close();
            }
            else if (this.MyDriver == UseDriver.OLEDB)
            {
                this.OleDbConnection.Close();
            }
        }
        public void RunSql(List<SQLResult> result, string cmd)
        {
            if (this.MyDriver == UseDriver.ODBC)
            {
                this.RunSqlODBC(result, cmd);
            }
            else if (this.MyDriver == UseDriver.OLEDB)
            {
                this.RunSqlOLEDB(result, cmd);
            }
        }

        private void RunSqlODBC(List<SQLResult> result, string cmd)
        {

            OdbcCommand toGo = this.ODBCConnection.CreateCommand();
            toGo.CommandTimeout = 3600 * 12;
            toGo.CommandText = cmd;

            OdbcDataReader reader = toGo.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int nCol = reader.FieldCount;
                    SQLResult newItem = new SQLResult();

                    if (nCol > 0)
                        newItem.Column0 = reader.IsDBNull(0) ? String.Empty : reader.GetValue(0).ToString();
                    if (nCol > 1)
                        newItem.Column1 = reader.IsDBNull(1) ? String.Empty : reader.GetValue(1).ToString();
                    if (nCol > 2)
                        newItem.Column2 = reader.IsDBNull(2) ? String.Empty : reader.GetValue(2).ToString();
                    if (nCol > 3)
                        newItem.Column3 = reader.IsDBNull(3) ? String.Empty : reader.GetValue(3).ToString();
                    if (nCol > 4)
                        newItem.Column4 = reader.IsDBNull(4) ? String.Empty : reader.GetValue(4).ToString();
                    if (nCol > 5)
                        newItem.Column5 = reader.IsDBNull(5) ? String.Empty : reader.GetValue(5).ToString();
                    if (nCol > 6)
                        newItem.Column6 = reader.IsDBNull(6) ? String.Empty : reader.GetValue(6).ToString();

                    result.Add(newItem);

                }
            }

            reader.Close();
            toGo.Dispose();
        }

        private void RunSqlOLEDB(List<SQLResult> result, string cmd)
        {

            OleDbCommand toGo = this.OleDbConnection.CreateCommand();
            toGo.CommandTimeout = 3600 * 12;
            toGo.CommandText = cmd;

            OleDbDataReader reader = toGo.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int nCol = reader.FieldCount;
                    SQLResult newItem = new SQLResult();

                    if (nCol > 0)
                        newItem.Column0 = reader.IsDBNull(0) ? String.Empty : reader.GetValue(0).ToString();
                    if (nCol > 1)
                        newItem.Column1 = reader.IsDBNull(1) ? String.Empty : reader.GetValue(1).ToString();
                    if (nCol > 2)
                        newItem.Column2 = reader.IsDBNull(2) ? String.Empty : reader.GetValue(2).ToString();
                    if (nCol > 3)
                        newItem.Column3 = reader.IsDBNull(3) ? String.Empty : reader.GetValue(3).ToString();
                    if (nCol > 4)
                        newItem.Column4 = reader.IsDBNull(4) ? String.Empty : reader.GetValue(4).ToString();
                    if (nCol > 5)
                        newItem.Column5 = reader.IsDBNull(5) ? String.Empty : reader.GetValue(5).ToString();
                    if (nCol > 6)
                        newItem.Column6 = reader.IsDBNull(6) ? String.Empty : reader.GetValue(6).ToString();

                    result.Add(newItem);

                }
            }

            reader.Close();
            toGo.Dispose();
        }
    }
}

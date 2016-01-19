using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Teradata.Client.Provider;

namespace SQLDepLib
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
            OLEDB = 2,
            TERADATA = 3,
        };

        public DBExecutor ()
        {
            MyDriver = DBExecutor.UseDriver.UNDEF_TYPE;
        }

        private UseDriver MyDriver { get; set; }
        public string ConnectString { get; set; }

        private OdbcConnection ODBCConnection { get; set; }

        private OracleConnection OleDbConnection { get; set; }

        private TdConnection TdConnection { get; set; }

        public string Hostname { get; set; }

        public string BuildConnectionString(string dbType, string auth_type, string server, string port, string database, string loginName, string loginpassword)
        {
            string ret = string.Empty;
            this.Hostname = server;

            // native support
            if (dbType == "oracle")
            {
                if (string.IsNullOrEmpty(port))
                {
                    port = "1521";
                }
                this.MyDriver = DBExecutor.UseDriver.OLEDB;
                this.ConnectString = String.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={4})))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={3})));User Id = {1}; Password = {2}; ",
                        server, loginName, loginpassword, database, port);

                return this.ConnectString;
            }

            // teradata - we have own driver
            if (dbType == "teradata")
            {
                this.MyDriver = DBExecutor.UseDriver.TERADATA;
                this.ConnectString = String.Format("Data Source = {0}; User ID = {1}; Password = {2};", server, loginName, loginpassword);
                return this.ConnectString;
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
            if (string.IsNullOrEmpty(port))
            {
                ret += "Server=" + server + ";";
            } else {
                ret += "Server=" + server + ":" + port + ";";
            }
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
            return ret;
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
                OracleConnection connection = new OracleConnection(this.ConnectString);
                connection.Open();
                this.OleDbConnection = connection;
            }
            else if (this.MyDriver == UseDriver.TERADATA)
            {
                TdConnection connection = new TdConnection(this.ConnectString);
                connection.Open();
                this.TdConnection = connection;
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
            else if (this.MyDriver == UseDriver.TERADATA)
            {
                this.TdConnection.Close();
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
                this.RunSqlOLEDB(result, cmd, false);
            }
            else if (this.MyDriver == UseDriver.TERADATA)
            {
                this.RunTeradata(result, cmd);
            }
        }
        public void RunQuerySql(List<SQLResult> result, string cmd)
        {
            if (this.MyDriver == UseDriver.ODBC)
            {
                this.RunSqlODBC(result, cmd);
            }
            else if (this.MyDriver == UseDriver.OLEDB)
            {
                this.RunSqlOLEDB(result, cmd, true);
            }
            else if (this.MyDriver == UseDriver.TERADATA)
            {
                this.RunTeradata(result, cmd);
            }
        }

        private void RunTeradata(List<SQLResult> result, string cmd)
        {
            try
            {
                TdCommand toGo = this.TdConnection.CreateCommand();
                toGo.CommandTimeout = 3600 * 12;
                toGo.CommandText = cmd;

                TdDataReader reader = toGo.ExecuteReader();

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
            catch (Exception ex)
            {
                throw;
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

        private void RunSqlOLEDB(List<SQLResult> result, string cmd, Boolean codeFirst)
        {

            OracleCommand toGo = this.OleDbConnection.CreateCommand();
            toGo.CommandTimeout = 3600 * 12;
            toGo.CommandText = cmd;
            toGo.InitialLONGFetchSize = -1;

            OracleDataReader reader = toGo.ExecuteReader();
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
            //toGo.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Teradata.Client.Provider;
using Npgsql;

namespace SQLDepLib
{
    /// <summary>
    /// vse co jde do databaze by melo jit pres tuto tridu
    /// </summary>
    public class DBExecutor
    {

        public enum UseDriver
        {
            DEFAULT = 0,
            ODBC = 1,
            ORACLE = 2,
            TERADATA = 3,
            POSTGRESQL = 4,  
        };

        public DBExecutor ()
        {
            MyDriver = DBExecutor.UseDriver.DEFAULT;
        }

        private UseDriver MyDriver { get; set; }
        public string ConnectString { get; set; }

        private OdbcConnection ODBCConnection { get; set; }

        private OracleConnection OleDbConnection { get; set; }

        private TdConnection TdConnection { get; set; }
        private NpgsqlConnection NpgsqlConnection { get; set; }

        public string Server { get; private set; }

        public string BuildConnectionString(string dbType, string dsnName, string auth_type, string server, string port, string database, string loginName, string loginpassword, string userDefinedDriverName, UseDriver useDriverType)
        {
            Logger.Log("Creating connection string, dbType: " + dbType);
            string ret = string.Empty;
            this.Server = server;

            if (dsnName != string.Empty)
            {
                // I wish to connect through named DSN, ODBC only
                useDriverType = UseDriver.ODBC;
            }

            if (useDriverType == UseDriver.DEFAULT || useDriverType == UseDriver.ORACLE)
            {
                // built in native support for oracle and teradata
                if (dbType == "oracle")
                {
                    if (string.IsNullOrEmpty(port))
                    {
                        port = "1521";
                    }
                    this.MyDriver = DBExecutor.UseDriver.ORACLE;
                    this.ConnectString = String.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={4})))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={3})));User Id = {1}; Password = {2}; ",
                            server, loginName, loginpassword, database, port);

                    return this.ConnectString;
                }
            }

            if (useDriverType == UseDriver.DEFAULT || useDriverType == UseDriver.TERADATA)
            {
                // teradata - we have own driver
                if (dbType == "teradata")
                {
                    this.MyDriver = DBExecutor.UseDriver.TERADATA;
                    TdConnectionStringBuilder builder = new TdConnectionStringBuilder();
                    builder.SessionCharacterSet = "UTF8";
                    builder.DataSource = server;
                    builder.UserId = loginName;
                    builder.Password = loginpassword;
                    this.ConnectString = builder.ToString();
                    return this.ConnectString;
                }
            }

            if (useDriverType == UseDriver.DEFAULT || useDriverType == UseDriver.POSTGRESQL)
            {
                // greenplum, redhift - we have own driver
                if (dbType == "greenplum" || dbType == "redshift" || dbType == "postgres")
                {
                    this.MyDriver = DBExecutor.UseDriver.POSTGRESQL;
                    NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
                    builder.Host = server;
                    builder.Encoding = "UTF8";

                    if (!string.IsNullOrEmpty(database))
                        builder.Database = database;                    

                    if (!string.IsNullOrEmpty(port))
                    {
                        if (int.TryParse(port, out int portNum) == false)
                        {
                            throw new ArgumentException("Could not parse port number");
                        }
                        else
                        {
                            builder.Port = portNum;
                        }
                    }

                    switch (auth_type)
                    {
                        case "win_auth":
                            builder.IntegratedSecurity = true;
                            builder.Username = loginName;
                            Logger.Log("Using Integrated Security: username may be required.");
                            Logger.Log("Connection string: " + builder.ToString());
                            break;
                        case "sql_auth":
                            // logging
                            builder.Username = "**user***";
                            builder.Password = "**passw**";
                            Logger.Log("Using passw");
                            Logger.Log("Connection string: " + builder.ToString());
                            builder.Password = loginpassword;
                            builder.Username = loginName;
                            break;
                        case "dsn_auth":
                        default: break;
                    }

                    this.ConnectString = builder.ToString();
                    return this.ConnectString;
                }
            }

            // the only solution is find an ODBC driver
            if (string.IsNullOrEmpty(dsnName))
            {
                // I wish to go through driver
                string driverName = string.Empty;

                if (userDefinedDriverName == string.Empty)
                {
                    List<string> drivers = ODBCUtils.GetSystemDriverList();
                    switch (dbType)
                    {
                        case "oracle":
                            driverName = drivers.Where(x => x.IndexOf("Oracle") >= 0).FirstOrDefault();
                            break;
                        case "mssql":
                        default:
                            driverName = drivers.Where(x => x.IndexOf("SQL Server") >= 0).FirstOrDefault();
                            break;
                    }
                }
                else
                {
                    driverName = userDefinedDriverName;
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
                }
                else
                {
                    ret += "Server=" + server + ":" + port + ";";
                }

                if (!string.IsNullOrEmpty(database))
                {
                    ret += "Database=" + database + ";";
                }
            }
            else
            {
                // I wish to go through named DSN
                ret = "DSN=" + dsnName + ";";
            }

            // add properties both for DSN or driver
            switch (auth_type)
            {
                case "win_auth":
                    ret += "Authentication=Windows Authentication;";
                    break;
                case "sql_auth":
                    ret += "UID=" + loginName + ";";
                    ret += "PWD=" + loginpassword + ";";
                    break;
                case "dsn_auth":
                default: break;
            }

            this.MyDriver = DBExecutor.UseDriver.ODBC;
            this.ConnectString = ret;
            return ret;
        }

        public void Connect ()
        {
            Logger.Log("Connecting to server");
            if (this.MyDriver == UseDriver.ODBC)
            {
                OdbcConnection connection = new OdbcConnection(this.ConnectString);
                connection.Open();
                this.ODBCConnection = connection;
            }
            else if (this.MyDriver == UseDriver.ORACLE)
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
            else if (this.MyDriver == UseDriver.POSTGRESQL)
            {
                NpgsqlConnection connection = new NpgsqlConnection(this.ConnectString);
                connection.Open();
                this.NpgsqlConnection = connection;
            }

            Logger.Log("Connection succesfully established");
        }
        public void Close()
        {
            if (this.MyDriver == UseDriver.ODBC)
            {
                this.ODBCConnection.Close();
            }
            else if (this.MyDriver == UseDriver.ORACLE)
            {
                this.OleDbConnection.Close();
            }
            else if (this.MyDriver == UseDriver.TERADATA)
            {
                this.TdConnection.Close();
            }
            else if (this.MyDriver == UseDriver.POSTGRESQL)
            {
                this.NpgsqlConnection.Close();
            }
        }
        public void RunSql(List<SQLResult> result, string cmd)
        {
            if (this.MyDriver == UseDriver.ODBC)
            {
                this.RunSqlODBC(result, cmd);
            }
            else if (this.MyDriver == UseDriver.ORACLE)
            {
                this.RunSqlOLEDB(result, cmd, false);
            }
            else if (this.MyDriver == UseDriver.TERADATA)
            {
                this.RunTeradata(result, cmd);
            }
            else if (this.MyDriver == UseDriver.POSTGRESQL)
            {
                this.RunNpsql(result, cmd);
            }

        }
        public void RunQuerySql(List<SQLResult> result, string cmd)
        {
            if (this.MyDriver == UseDriver.ODBC)
            {
                this.RunSqlODBC(result, cmd);
            }
            else if (this.MyDriver == UseDriver.ORACLE)
            {
                this.RunSqlOLEDB(result, cmd, true);
            }
            else if (this.MyDriver == UseDriver.TERADATA)
            {
                this.RunTeradata(result, cmd);
            }
            else if (this.MyDriver == UseDriver.POSTGRESQL)
            {
                this.RunNpsql(result, cmd);
            }
        }

        private void RunNpsql(List<SQLResult> result, string cmd)
        {
            try
            {
                NpgsqlCommand toGo = this.NpgsqlConnection.CreateCommand();
                toGo.CommandTimeout = 3600 * 12;
                toGo.CommandText = cmd;

                NpgsqlDataReader reader = toGo.ExecuteReader();
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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
            catch (Exception)
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
        }
    }
}

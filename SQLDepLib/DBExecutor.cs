using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Runtime.Hosting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Teradata.Client.Provider;
using Npgsql;
using Snowflake.Data.Client;

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
            SNOWFLAKE = 5,
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
        private SnowflakeDbConnection SnowflakeConnection { get; set; }

        public string Server { get; private set; }

        public string BuildConnectionString(Arguments args, UseDriver useDriverType)
        {
            Logger.Log("Creating connection string for " + args.dbType);
            string ret = string.Empty;
            this.Server = args.server;

            if (args.dsnName != string.Empty)
            {
                // I wish to connect through named DSN, ODBC only
                useDriverType = UseDriver.ODBC;
            }

            if (useDriverType == UseDriver.DEFAULT || useDriverType == UseDriver.ORACLE)
            {
                // built in native support for oracle and teradata
                if (args.dbType == "oracle")
                {
                    if (string.IsNullOrEmpty(args.port))
                    {
                        args.port = "1521";
                    }
                    this.MyDriver = DBExecutor.UseDriver.ORACLE;
                    this.ConnectString = String.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={4})))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={3})));User Id = {1}; Password = {2}; ",
                        args.server, args.loginName, args.loginpassword, args.database, args.port);

                    return this.ConnectString;
                }
            }

            if (useDriverType == UseDriver.DEFAULT || useDriverType == UseDriver.TERADATA)
            {
                // teradata - we have own driver
                if (args.dbType == "teradata")
                {
                    this.MyDriver = DBExecutor.UseDriver.TERADATA;
                    TdConnectionStringBuilder builder = new TdConnectionStringBuilder();
                    builder.SessionCharacterSet = "UTF8";
                    builder.DataSource = args.server;
                    builder.UserId = args.loginName;
                    builder.Password = args.loginpassword;
                    this.ConnectString = builder.ToString();
                    return this.ConnectString;
                }
            }

            if (useDriverType == UseDriver.DEFAULT || useDriverType == UseDriver.POSTGRESQL)
            {
                // greenplum, redhift - we have own driver
                if (args.dbType == "greenplum" || args.dbType == "redshift" || args.dbType == "postgres")
                {
                    this.MyDriver = DBExecutor.UseDriver.POSTGRESQL;
                    NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
                    builder.Host = args.server;
                    builder.Encoding = "UTF8";

                    if (!string.IsNullOrEmpty(args.database))
                        builder.Database = args.database;                    

                    if (!string.IsNullOrEmpty(args.port))
                    {
                        if (int.TryParse(args.port, out int portNum) == false)
                        {
                            throw new ArgumentException("Could not parse port number");
                        }
                        else
                        {
                            builder.Port = portNum;
                        }
                    }

                    switch (args.auth_type)
                    {
                        case "win_auth":
                            builder.IntegratedSecurity = true;
                            builder.Username = args.loginName;
                            Logger.Log("Using Integrated Security: username may be required.");
                            Logger.Log("Connection string: " + builder.ToString());
                            break;
                        case "sql_auth":
                            // logging
                            builder.Username = "**user***";
                            builder.Password = "**passw**";
                            Logger.Log("Using passw");
                            Logger.Log("Connection string: " + builder.ToString());
                            builder.Password = args.loginpassword;
                            builder.Username = args.loginName;
                            break;
                        case "dsn_auth":
                        default: break;
                    }

                    this.ConnectString = builder.ToString();
                    return this.ConnectString;
                }
            }

            if (useDriverType == UseDriver.DEFAULT || useDriverType == UseDriver.SNOWFLAKE)
            {
                // snowflake - we have own driver
                if (args.dbType == "snowflake")
                {
                    this.MyDriver = DBExecutor.UseDriver.SNOWFLAKE;
                    if (String.IsNullOrEmpty(args.loginName) || String.IsNullOrEmpty(args.loginpassword) ||
                        String.IsNullOrEmpty(args.account))
                        throw new ArgumentException("Login, password and client has to be filled in!");
                    ConnectString = String.Format("account={0};user={1};password={2}", args.account, args.loginName, args.loginpassword);
                    ConnectString += !String.IsNullOrEmpty(args.database) ? String.Format(";db={0}", args.database) : "";
                    ConnectString += !String.IsNullOrEmpty(args.server) ? String.Format(";host={0}", args.server) : "";
                    ConnectString += !String.IsNullOrEmpty(args.role) ? String.Format(";role={0}", args.role) : "";
                    ConnectString += !String.IsNullOrEmpty(args.warehouse) ? String.Format(";warehouse={0}", args.warehouse) : "";

                    Regex rgx = new Regex("password=[^;]*");
                    Logger.Log(String.Format("Connection string: {0}", rgx.Replace(ConnectString, "password=*****")));

                    return this.ConnectString;
                }
            }

            // the only solution is find an ODBC driver
            if (string.IsNullOrEmpty(args.dsnName))
            {
                // I wish to go through driver
                string driverName = string.Empty;

                if (args.driverName == string.Empty)
                {
                    List<string> drivers = ODBCUtils.GetSystemDriverList();
                    switch (args.dbType)
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
                    driverName = args.driverName;
                }

                if (string.IsNullOrEmpty(driverName))
                {
                    throw new Exception("No ODBC driver found, please install and try again");
                }
                else
                {
                    ret += "Driver={" + driverName + "};";
                }

                if (string.IsNullOrEmpty(args.port))
                {
                    ret += "Server=" + args.server + ";";
                }
                else
                {
                    ret += "Server=" + args.server + ":" + args.port + ";";
                }

                if (!string.IsNullOrEmpty(args.database))
                {
                    ret += "Database=" + args.database + ";";
                }
            }
            else
            {
                // I wish to go through named DSN
                ret = "DSN=" + args.dsnName + ";";
            }

            // add properties both for DSN or driver
            switch (args.auth_type)
            {
                case "win_auth":
                    ret += "Authentication=Windows Authentication;";
                    break;
                case "sql_auth":
                    ret += "UID=" + args.loginName + ";";
                    ret += "PWD=" + args.loginpassword + ";";
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
            else if (this.MyDriver == UseDriver.SNOWFLAKE)
            {
                SnowflakeDbConnection connection = new SnowflakeDbConnection();
                connection.ConnectionString = ConnectString;
                connection.Open();
                this.SnowflakeConnection = connection;
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
            else if (this.MyDriver == UseDriver.SNOWFLAKE)
            {
                this.SnowflakeConnection.Close();
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
            else if (this.MyDriver == UseDriver.SNOWFLAKE)
            {
                this.RunSnowflake(result, cmd);
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

        private void RunSnowflake(List<SQLResult> result, string cmd)
        {

            IDbCommand toGo = this.SnowflakeConnection.CreateCommand();
            toGo.CommandTimeout = 3600 * 12;
            toGo.CommandText = cmd;

            IDataReader reader = toGo.ExecuteReader();
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

            reader.Close();
        }
    }


}

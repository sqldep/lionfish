using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Options;
using SQLDepLib;

namespace SQLDepCmd
{
    class Program
    {
        static int Main(string[] args)
        {
            string dbType = string.Empty;
            string hostName = string.Empty;
            string auth_type = "sql_auth";
            string server = string.Empty;
            string port = string.Empty;
            string database = string.Empty;
            string loginName = string.Empty;
            string loginpassword = string.Empty;
            string customSqlSetName = string.Empty;
            string exportFileName = string.Empty;
            string sMyKey = string.Empty;
            Guid myKey;

            var p = new OptionSet() {
                { "db|dbType=", "database type MsSQL(mssql)/Oracle(oracle)", v => dbType = v },
                { "h|hostName=",  "hostname", v => hostName = v },
                { "a|auth=",  "authorization SQL(default: sql_auth)/Windows (win_auth)", v => { if (v != null) auth_type = v; } },
                { "s|server=",  "server", v => server = v },
                { "p|port=",  "port", v => { if ( v != null) port = v; } },
                { "d|database=",  "database", v => database = v },
                { "u|user=",  "loginName", v => { if ( v != null) loginName = v; } },
                { "pwd|password=",  "loginpassword", v => { if (v != null) loginpassword = v; } },
                { "n|name=",  "name", v => customSqlSetName = v },
                { "f|file=",  "file", v => exportFileName = v },
                { "k|key=",  "key (Guid)", v => sMyKey = v },
            };

            try
            {
                p.Parse(args);

                myKey = Guid.Parse(sMyKey);

                DBExecutor dbExecutor = new DBExecutor();
                string connectString = dbExecutor.BuildConnectionString(dbType, auth_type, server, port, database, loginName, loginpassword);
                dbExecutor.ConnectString = connectString;
                dbExecutor.Hostname = hostName;
                dbExecutor.Connect();

                Executor executor = new Executor(dbExecutor);
                executor.Run(customSqlSetName, myKey, dbType, exportFileName);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

            return 0;
        }
    }
}

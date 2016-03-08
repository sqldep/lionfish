using System;
using System.Collections.Generic;
using System.IO;
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
            string auth_type = "sql_auth";
            string server = string.Empty;
            string port = string.Empty;
            string database = string.Empty;
            string loginName = string.Empty;
            string loginpassword = string.Empty;
            string customSqlSetName = string.Empty;
            string exportFileName = string.Empty;
            string sMyKey = string.Empty;
            string sendFile = string.Empty;
            string help = string.Empty;
            string driverName = string.Empty;
            Guid myKey;

            var p = new OptionSet() {
                { "dbType=", "database type MsSQL(mssql)/Oracle(oracle)", v => dbType = v },
                { "a|auth=",  "authorization SQL(default: sql_auth)/Windows (win_auth)", v => { if (v != null) auth_type = v; } },
                { "s|server=",  "server", v => server = v },
                { "p|port=",  "port", v => { if ( v != null) port = v; } },
                { "d|database=",  "database (SID for Oracle)", v => database = v },
                { "u|user=",  "loginName", v => { if ( v != null) loginName = v; } },
                { "pwd|password=",  "loginpassword", v => { if (v != null) loginpassword = v; } },
                { "n|name=",  "name of export", v => customSqlSetName = v },
                { "f|file=",  "output file", v => exportFileName = v },
                { "k|key=",  "api key (Guid)", v => sMyKey = v },
                { "h|help",  "show help", v => help = "set" },
                { "driver",  "driver name", v => driverName = v },
                { "send=",  "SEND or SENDONLY, default do not send", v => sendFile = v.ToUpper() },
            };

            try
            {
                p.Parse(args);
                
                if (help.Equals("set") || (args.Length == 0))
                {
                    ShowHelp(p);
                }
                else
                {
                    myKey = Guid.Parse(sMyKey);

                    DBExecutor dbExecutor = new DBExecutor();

                    bool runDb = (sendFile != "SENDONLY");
                    bool sendIt = (sendFile == "SEND" || sendFile == "SENDONLY");

                    string connectString = dbExecutor.BuildConnectionString(dbType, auth_type, server, port, database, loginName, loginpassword, driverName, DBExecutor.UseDriver.DEFAULT);
                    dbExecutor.ConnectString = connectString;
                    if (runDb)
                    {
                        dbExecutor.Connect();
                    }

                    Executor executor = ExecutorFactory.CreateExecutor(dbExecutor, dbType);

                    if (runDb)
                    {
                        executor.Run(customSqlSetName, myKey, dbType, exportFileName);
                    }

                    if (sendIt)
                    {
                        List<string> sendFiles = new List<string>();

                        FileAttributes fileattr;
                        foreach (var item in exportFileName.Split(','))
                        {
                            fileattr = File.GetAttributes(item);

                            if ((fileattr & FileAttributes.Directory) == FileAttributes.Directory)
                            {
                                // add whole directory content
                                foreach (string fileName in Directory.EnumerateFiles(item, "*.*"))
                                {
                                    // skip inner directories
                                    fileattr = File.GetAttributes(fileName);

                                    if ((fileattr & FileAttributes.Directory) == 0)
                                    {
                                        sendFiles.Add(fileName);
                                    }
                                }
                            }
                            else
                            {
                                sendFiles.Add(item);
                            }
                        }

                        executor.SendFiles(sendFiles, sMyKey);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
            return 0; // standard success
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: greet [OPTIONS]+ message");
            Console.WriteLine("Greet a list of individuals with an optional message.");
            Console.WriteLine("If no message is specified, a generic greeting is used.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}

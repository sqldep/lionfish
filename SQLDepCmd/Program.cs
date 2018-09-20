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
            Arguments arguments = new Arguments();

            var p = new OptionSet() {
                { "dbType=", "database type: mssql|oracle|greenplum|postgres|redshift|snowflake", v => arguments.dbType = v },
                { "a|auth=",  "authorization SQL(default: sql_auth)/ Windows(win_auth)", v => { if (v != null) arguments.auth_type = v; } },
                { "s|server=",  "server", v => arguments.server = v },
                { "acc|account=",  "account", v => arguments.account = v },
                { "p|port=",  "port", v => { if ( v != null) arguments.port = v; } },
                { "d|database=",  "database (SID for Oracle)", v => arguments.database = v },
                { "u|user=",  "loginName", v => { if ( v != null) arguments.loginName = v; } },
                { "pwd|password=",  "loginpassword", v => { if (v != null) arguments.loginpassword = v; } },
                { "n|name=",  "batch name for the web dashboard", v => arguments.customSqlSetName = v },
                { "f|file=",  "output file", v => arguments.exportFileName = v },
                { "k|key=",  "api key (Guid)", v => arguments.sMyKey = v },
                { "h|help",  "show help", v => arguments.help = "set" },
                { "driver",  "driver name", v => arguments.driverName = v },
                { "send=",  "SEND or SENDONLY, default do not send", v => arguments.sendFile = v.ToUpper() },
                { "use-filesystem",  "Use this option to use FS. file_system.conf must be configured!", v => arguments.useFS = true},
            };

            try
            {
                p.Parse(args);
                
                if (arguments.help.Equals("set") || (args.Length == 0))
                {
                    ShowHelp(p);
                }
                else
                {

                    arguments.myKey = Guid.Parse(arguments.sMyKey);

                    DBExecutor dbExecutor = new DBExecutor();

                    bool runDb = (arguments.sendFile != "SENDONLY");
                    bool sendIt = (arguments.sendFile == "SEND" || arguments.sendFile == "SENDONLY");
                    string connectString = dbExecutor.BuildConnectionString(arguments, DBExecutor.UseDriver.DEFAULT);
                    dbExecutor.ConnectString = connectString;

                    Executor executor = ExecutorFactory.CreateExecutor(dbExecutor, arguments.dbType);

                    if (runDb)
                    {
                        executor.Run(arguments.customSqlSetName, arguments.myKey, arguments.dbType, arguments.exportFileName, arguments.useFS);
                    }

                    if (sendIt)
                    {
                        List<string> sendFiles = new List<string>();

                        FileAttributes fileattr;
                        foreach (var item in arguments.exportFileName.Split(','))
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

                        executor.SendFiles(sendFiles, arguments.sMyKey);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e.Message);
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

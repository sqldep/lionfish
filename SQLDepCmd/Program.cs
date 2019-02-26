using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Mono.Options;
using SQLDepLib;

namespace SQLDepCmd
{
    class Program
    {
        static int Main(string[] args)
        {
            Arguments a = new Arguments();
            bool useGUIConfig = false;

            var p = new OptionSet() {
                { "dbType=", "database type: mssql|oracle|greenplum|postgres|redshift|snowflake", v => a.dbType = v },
                { "a|auth=",  "authorization SQL(default: sql_auth)/ Windows(win_auth)", v => { if (v != null) a.auth_type = v; } },
                { "s|server=",  "server", v => a.server = v },
                { "p|port=",  "port", v => { if ( v != null) a.port = v; } },
                { "d|database=",  "database (SID for Oracle)", v => a.database = v },
                { "u|user=",  "loginName", v => { if ( v != null) a.loginName = v; } },
                { "pwd|password=",  "loginpassword", v => { if (v != null) a.loginpassword = v; } },
                { "n|name=",  "batch name for the web dashboard", v => a.customSqlSetName = v },
                { "f|file=",  "output file - ZIP file", v => a.exportFileName = v },
                { "k|key=",  "api key (Guid)", v => a.sMyKey = v },
                { "h|help",  "show help", v => a.help = true },
                { "driver=",  "driver name", v => a.driverName = v },

                { "send=",  "SEND or SENDONLY, default do not send", v => a.sendFile = v.ToUpper() },
                { "warehouse=",  "(Snowflake only) Warehouse", v => a.warehouse = v },
                { "account=",  "(Snowflake only) Account", v => a.account = v },
                { "role=",  "(Snowflake only) Role", v => a.role = v },

                { "gui-config",  "Use configuration saved in GUI version.", v => useGUIConfig = true},
                { "gui-config-filename=",  "Custom UI configuration file.", v =>  UIConfig.ConfigFilename = v},

                { "fs_path=",  "(FS conf) Path to folder with sql queries.", v => { a.fs_path = v; a.fs_useFs = true;} },
                { "fs_mask=",  "(FS conf) File mask.", v => a.fs_mask = v},
                { "fs_schema=",  "(FS conf) Default schema in FS queries.", v => a.fs_default_schema = v},
                { "fs_database=",  "(FS conf) Default database in FS queries.", v => a.fs_default_db = v},

                { "ex_SAP_path=",  "(external conf) Path for SAP folder.", v => { a.ext_SAPPath = v; a.ext_useSAP = true;}},
                { "ex_INFA_path=",  "(external conf) Path for INFA folder.", v => { a.ext_InformaticaPath = v; a.ext_useInformatica = true;}},
                { "ex_SSIS_path=",  "(external conf) Path for SSIS folder.", v => { a.ext_SSISPath = v; a.ext_useSSIS = true;}},
                { "ex_SAP_mask=",  "(external conf) Mask for SAP files.", v => a.ext_SAPMask = v},
                { "ex_INFA_mask=",  "(external conf) Mask for INFA files.", v => a.ext_INFAMask = v},
                { "ex_SSIS_mask=",  "(external conf) Mask for SSIS files.", v => a.ext_SSISMask = v},
            };

            try
            {
                p.Parse(args);

                if (useGUIConfig)
                {
                    Logger.Log(String.Format("Using UI config file: {0}", UIConfig.ConfigFilename));
                    if (String.IsNullOrEmpty(a.exportFileName))
                    {
                        throw new ArgumentException("--file argument needs to be set when using GUI config.");
                    }

                    Arguments old = a;
                    a = UIConfig.GetArguments();
                    a.sendFile = old.sendFile;
                    a.exportFileName = old.exportFileName;
                }
                
                if (a.help || (args.Length == 0))
                {
                    ShowHelp(p);
                    return 0;
                }

                a.myKey = Guid.Parse(a.sMyKey);

                DBExecutor dbExecutor = new DBExecutor();

                bool runDb = (a.sendFile != "SENDONLY");
                bool sendIt = (a.sendFile == "SEND" || a.sendFile == "SENDONLY");
                string connectString = dbExecutor.BuildConnectionString(a, DBExecutor.UseDriver.DEFAULT);
                dbExecutor.ConnectString = connectString;

                Executor executor = ExecutorFactory.CreateExecutor(dbExecutor, a.dbType);

                if (runDb)
                {
                    executor.Run(a);
                }

                if (sendIt)
                {
                    List<string> sendFiles = new List<string>();

                    FileAttributes fileattr;
                    foreach (var item in a.exportFileName.Split(','))
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

                    executor.SendFiles(sendFiles, a.sMyKey);
                }
                
            }
            catch (Exception e)
            {
               
                string exMessage = e.Message;
                if (e.InnerException != null)
                {
                    exMessage += "\nInner exception: " + e.InnerException.Message;
                }
                Logger.Exception(exMessage);
                return 1;
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

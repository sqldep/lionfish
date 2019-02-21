using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLDepLib;

namespace SQLDepLib
{
    public class Arguments
    {
        public string dbType { get; set; }
        public string auth_type { get; set; }
        public string server { get; set; }
        public string port { get; set; }
        public string database { get; set; }
        public string loginName { get; set; }
        public string loginpassword { get; set; }
        public string customSqlSetName { get; set; }
        public string sMyKey { get; set; }
        public string sendFile { get; set; }
        public string help { get; set; }
        public string driverName { get; set; }
        public string dsnName { get; set; }
        public bool useFS { get; set; }
        public string account { get; set; }
        public string exportFileName { get; set; }
        public string warehouse { get; set; }
        public string role { get; set; }
        public Guid myKey { get; set; }

        // external files
        public string ext_SAPPath { get; set; }
        public string ext_SSISPath { get; set; }
        public string ext_InformaticaPath { get; set; }
        public bool ext_useSAP { get; set; }
        public bool ext_useSSIS { get; set; }
        public bool ext_useInformatica { get; set; }
        public string ext_SAPMask { get; set; }
        public string ext_INFAMask { get; set; }
        public string ext_SSISMask { get; set; }

        public Arguments()
        {
            dbType = string.Empty;
            auth_type = "sql_auth";
            server = string.Empty;
            port = string.Empty;
            database = string.Empty;
            loginName = string.Empty;
            loginpassword = string.Empty;
            customSqlSetName = string.Empty;
            exportFileName = string.Empty;
            sMyKey = string.Empty;
            sendFile = string.Empty;
            help = string.Empty;
            driverName = string.Empty;
            dsnName = string.Empty;
            account = string.Empty; // used in snowflake connection
            useFS = false;
            warehouse = string.Empty;
            role = string.Empty;
            ext_SSISMask = ext_INFAMask = ext_SAPMask = 
                ext_InformaticaPath = ext_SAPPath = ext_SSISPath = string.Empty;
            ext_useInformatica = ext_useSAP = ext_useSSIS = false;
        }


        public bool Validate()
        {
            if (sendFile != "SENDONLY")
            {
                // check dbType
                string[] allDbTypes = { "mssql", "oracle", "greenplum", "postgres", "redshift" };
                if (!allDbTypes.Contains(dbType))
                {
                    Logger.Log("dbType is not valid! It must be one of: {\"mssql\", \"oracle\", \"greenplum\", \"postgres\", \"redshift\"}");
                    return false;
                }
            }

            return true;
        }
    }
}

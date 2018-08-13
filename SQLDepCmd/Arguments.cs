using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLDepLib;

namespace SQLDepCmd
{
    public class Arguments
    {
        public string dbtype { get; set; }
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
        public bool useFS { get; set; }
        public Guid myKey { get; set; }

        public Arguments()
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
            bool useFS = false;
        }


        public bool Validate()
        {
            if (sendFile != "SENDONLY")
            {
                // check dbType
                string[] allDbTypes = { "mssql", "oracle", "greenplum", "postgres", "redshift" };
                if (!allDbTypes.Contains(dbtype))
                {
                    Logger.Log("dbType is not valid! It must be one of: {\"mssql\", \"oracle\", \"greenplum\", \"postgres\", \"redshift\"}");
                    return false;
                }
            }

            return true;
        }
    }
}

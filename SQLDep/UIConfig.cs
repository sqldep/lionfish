using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SQLDep
{
    class UIConfig
    {
        // klice se musi lisit
        public const string SQL_DIALECT = "sql_dialect";
        public const string SERVER_NAME = "server_name";
        public const string SERVER_PORT = "1523";
        public const string DATA_SET_NAME = "data_set_name";
        public const string SQLDEP_KEY = "SQL_dep_key";
        public const string LOGIN_NAME = "login_name";
        public const string LOGIN_PASSWORD = "login_password";
        public const string AUTH_TYPE = "auth_type";
        public const string DATABASE_NAME = "database_name";
        public const string DRIVER_NAME = "driver_name";

        public const string DRIVER_NAME_NATIVE = "Native Driver";

        static public string Get (string key, string defaultValue)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings[key] == null)
            {
                return defaultValue;
            }
            else
            {
                return config.AppSettings.Settings[key].Value;
            }
        }
        static public void Set (string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}

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
        public const string DB_CONN = "db_connection";
        public const string DATA_SET_NAME = "data_set_name";
        public const string SQLDEP_KEY = "SQL_dep_key";

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

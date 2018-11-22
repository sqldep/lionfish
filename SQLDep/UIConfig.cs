using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using SQLDepLib;
using static System.Net.Mime.MediaTypeNames;
using System.Xml;

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
        public const string FS_PATH = "fs_path";
        public const string FS_MASK = "fs_mask";
        public const string FS_DEFAULT_SCHEMA = "fs_default_schema";
        public const string FS_DEFAULT_DB = "fs_default_db";
        public const string DRIVER_NAME_NATIVE = "Native Driver";
        public const string SNOWFLAKE_WAREHOUSE = "Warehouse";
        public const string SNOWFLAKE_ACCOUNT = "Account";
        public const string SNOWFLAKE_ROLE= "Role";

        private const string ConfigFilename = "UIConfig.xml";

        public static string Get (string key, string defaultValue)
        {
            ExeConfigurationFileMap configFile = new ExeConfigurationFileMap
            {
                ExeConfigFilename = ConfigFilename
            };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);

            if (config.AppSettings.Settings[key] == null)
            {
                return defaultValue;
            }
            else
            {
                if (key == LOGIN_PASSWORD)
                {
                    return Base64Decode(config.AppSettings.Settings[key].Value);
                }

                return config.AppSettings.Settings[key].Value;
            }
        }
        public static void Set (string key, string value)
        {
            ExeConfigurationFileMap configFile = new ExeConfigurationFileMap
            {
                ExeConfigFilename = ConfigFilename
            };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);
            if (key == LOGIN_PASSWORD)
            {
                config.AppSettings.Settings.Add(key, Base64Encode(value));
            }
            else
            {
                config.AppSettings.Settings.Add(key, value);
            }

            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}

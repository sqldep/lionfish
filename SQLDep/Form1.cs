using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;

namespace SQLDep
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeValues();
        }


        private void InitializeValues()
        {
            string sqlDialect = UIConfig.Get(UIConfig.SQL_DIALECT, "mssql");
            this.InitDatabaseValues(sqlDialect);
            this.textBoxUserName.Text = UIConfig.Get(UIConfig.DATA_SET_NAME, "My Data Set Name");
            this.textBoxKey.Text = UIConfig.Get(UIConfig.SQLDEP_KEY, "12345678-1234-1234-1234-123456789012");
        }

        private void InitDatabaseValues(string sqlDialect)
        {
            switch (sqlDialect)
            {
                case "oracle":
                    {
                        this.comboBoxDatabase.SelectedIndex = 0;
                        this.textBoxConnectionString.Text = UIConfig.Get(UIConfig.DB_CONN + sqlDialect, "Driver={Oracle};Server=SERVER;Database=master;UID=USER;PWD=PASSWORD");
                        break;
                    }
                case "mssql":
                    {
                        this.comboBoxDatabase.SelectedIndex = 1;
                        this.textBoxConnectionString.Text = UIConfig.Get(UIConfig.DB_CONN + sqlDialect, "Driver={SQL Server};Server=SERVER;Database=master;UID=USER;PWD=PASSWORD");
                        break;
                    }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // co to vybral ?
                int dbType = this.comboBoxDatabase.SelectedIndex;
                if (dbType < 0)
                {
                    throw new Exception("Please select database!");
                }

                string conn = this.textBoxConnectionString.Text.ToString();
                string myName = this.textBoxUserName.Text.ToString();
                string myKey = this.textBoxKey.Text.ToString();
                string sqlDialect;
                switch(dbType)
                {
                    case 0:
                        {
                            sqlDialect = "oracle"; break;
                        }
                    case 1:
                        {
                            sqlDialect = "mssql"; break;
                        }
                    default: throw new Exception("Undefined db type, please select your database type from combobox!"); 
                }

                // uloz nastaveni uzivatele
                UIConfig.Set(UIConfig.SQL_DIALECT, sqlDialect);
                UIConfig.Set(UIConfig.DB_CONN + sqlDialect, this.textBoxConnectionString.Text.ToString());
                UIConfig.Set(UIConfig.DATA_SET_NAME, this.textBoxUserName.Text.ToString());
                UIConfig.Set(UIConfig.SQLDEP_KEY, this.textBoxKey.Text.ToString());

                // na test lze pouzit: "356d0c42-8717-495d-ad6b-339cd6e530fb"
                // go!
                //Guid myGuid;
                //if (!Guid.TryParse(myKey, out myGuid))
                //{
                //    throw new Exception("Invalid key, it is not possible to handle as quide!");
                //}

                List<string> failedDbs = new List<string>();
                Executor executor = new Executor();

                string exportFileName;
                executor.Run(conn, dbType, myName, myKey, sqlDialect, out exportFileName);

                DialogResult answer = MessageBox.Show("Send data to SQLDep?", "Please confirm data sending.", MessageBoxButtons.YesNo);

                if (answer == DialogResult.Yes)
                {
                    executor.SendStructure();
                    MessageBox.Show("Completed succesfully. Data were sent!");
                }
                else
                {
                    MessageBox.Show("Completed succesfully. Data are saved on disk! " + exportFileName);
                }

                executor = null; // zahod vysledky
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                MessageBox.Show(msg);
            }
        }

        private void comboBoxDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            int dbType = this.comboBoxDatabase.SelectedIndex;
            switch (dbType)
            {
                case 0:
                    {
                        this.InitDatabaseValues("oracle"); break;
                    }
                case 1:
                    {
                        this.InitDatabaseValues("mssql"); break;
                    }
            }
        }
    }
}

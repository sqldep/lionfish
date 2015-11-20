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


#if DEBUG
                {
                // jen pro ucely testovani
                    sqlDialect = "mssql";
                    //conn = @"Driver ={SQL Server}; Server = mmnag1\sql2014; Initial Catalog = Nemocnice_vyvoj; UID = dplaner; PWD = ";
                    myKey = "63b95df9-da06-4612-9ad7-e763d4e1ea12";
                }
#endif
                // na test lze pouzit: "356d0c42-8717-495d-ad6b-339cd6e530fb"

                // go!
                //Guid myGuid;
                //if (!Guid.TryParse(myKey, out myGuid))
                //{
                //    throw new Exception("Invalid key, it is not possible to handle as quide!");
                //}

                List<string> failedDbs = new List<string>();
                new Executor().Run(conn, dbType, myName, myKey, sqlDialect, failedDbs);

                if (failedDbs.Count > 0)
                {
                    string msg = failedDbs.Count + " databases were not performed properly. Verify your connection. Some data were sent.";
                    MessageBox.Show(msg);
                }
                else
                {
                    MessageBox.Show("Completed succesfully. Thank you!");
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                MessageBox.Show(msg);
            }
        }

    }
}

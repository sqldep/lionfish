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
using System.Data.Odbc;
using System.Threading;
using SQLDepLib;

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
            this.comboBoxDatabase.SelectedIndex = this.GetDatabaseTypeIdx(UIConfig.Get(UIConfig.SQL_DIALECT, "mssql"));
            this.comboBoxAuthType.SelectedIndex = this.GetAuthTypeIdx(UIConfig.Get(UIConfig.AUTH_TYPE, "sql_auth"));
            this.textBoxServerName.Text = UIConfig.Get(UIConfig.SERVER_NAME, "");
            this.textBoxPort.Text = UIConfig.Get(UIConfig.SERVER_PORT, "");
            this.textBoxLoginName.Text = UIConfig.Get(UIConfig.LOGIN_NAME, "");
            this.textBoxLoginPassword.Text = UIConfig.Get(UIConfig.LOGIN_PASSWORD, "");
            this.textBoxUserName.Text = UIConfig.Get(UIConfig.DATA_SET_NAME, "My Data Set Name");
            this.textBoxDatabaseName.Text = UIConfig.Get(UIConfig.DATABASE_NAME, "master");
            this.textBoxKey.Text = UIConfig.Get(UIConfig.SQLDEP_KEY, "");
            this.buttonRun.Enabled = false;
            this.EnableAuthSettings();
            //
            CheckForIllegalCrossThreadCalls = false;
        }

        public void EnableAuthSettings()
        {
            if ( this.GetAuthTypeName(this.comboBoxAuthType.SelectedIndex) == "sql_auth")
            {
                this.textBoxLoginName.Enabled = true;
                this.textBoxLoginPassword.Enabled = true;
            }
            else
            {
                this.textBoxLoginName.Enabled = false;
                this.textBoxLoginPassword.Enabled = false;
            }
        }

        private int GetDatabaseTypeIdx (string sqlDialect)
        {
            if (sqlDialect == "teradata")
            {
                return 2;
            }
            else if (sqlDialect == "oracle")
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private string GetDatabaseTypeName(int idx)
        {
            if (idx == 0)
            {
                return "oracle";
            }
            else if (idx == 2)
            {
                return "teradata";
            }
            else
            {
                return "mssql";
            }
        }

        private int GetAuthTypeIdx(string authType)
        {
            if (authType == "sql_auth")
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private string GetAuthTypeName(int idx)
        {
            if (idx == 0)
            {
                return "sql_auth";
            }
            else
            {
                return "win_auth";
            }
        }

        private string BuildConnectionString (DBExecutor dbExecutor)
        {
            return dbExecutor.BuildConnectionString(this.GetDatabaseTypeName(this.comboBoxDatabase.SelectedIndex),
                                                this.GetAuthTypeName(this.comboBoxAuthType.SelectedIndex),
                                                this.textBoxServerName.Text,
                                                this.textBoxPort.Text, 
                                                this.textBoxDatabaseName.Text,
                                                this.textBoxLoginName.Text,
                                                this.textBoxLoginPassword.Text);
        }

        private void SaveDialogSettings ()
        {
            UIConfig.Set(UIConfig.AUTH_TYPE, this.GetAuthTypeName(this.comboBoxAuthType.SelectedIndex));
            UIConfig.Set(UIConfig.SQL_DIALECT, this.GetDatabaseTypeName(this.comboBoxDatabase.SelectedIndex));
            UIConfig.Set(UIConfig.DATA_SET_NAME, this.textBoxUserName.Text.ToString());
            UIConfig.Set(UIConfig.SQLDEP_KEY, this.textBoxKey.Text.ToString());
            UIConfig.Set(UIConfig.SERVER_NAME, this.textBoxServerName.Text.ToString());
            UIConfig.Set(UIConfig.SERVER_PORT, this.textBoxPort.Text.ToString());
            UIConfig.Set(UIConfig.LOGIN_NAME, this.textBoxLoginName.Text.ToString());
            UIConfig.Set(UIConfig.LOGIN_PASSWORD, this.textBoxLoginPassword.Text.ToString());
            UIConfig.Set(UIConfig.DATABASE_NAME, this.textBoxDatabaseName.Text.ToString());
        }

        public AsyncExecutor AsyncExecutor { get; set; }
        public Thread AsyncExecutorThread { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            // only one execution at time is allowed, do nothing now
            if (this.AsyncExecutor != null)
            {
                return;  
            }

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            try
            {
                this.SaveDialogSettings();
                DBExecutor dbExecutor = new DBExecutor();
                this.BuildConnectionString(dbExecutor);

                string myName = this.textBoxUserName.Text.ToString();
                Guid myKey;
                if (!Guid.TryParse(this.textBoxKey.Text.ToString(), out myKey))
                {
                    throw new Exception("Invalid or missing API key! Get one at https://www.sqldep.com/browser/upload/api");
                }

                string sqlDialect = this.GetDatabaseTypeName(this.comboBoxDatabase.SelectedIndex);


                List<string> failedDbs = new List<string>();
                Executor executor = new Executor(dbExecutor);

                string exportFileName = fbd.SelectedPath + "\\DBexport_" + executor.runId + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".json";

                this.AsyncExecutor = new AsyncExecutor(myName, myKey, sqlDialect, exportFileName, executor);
                this.AsyncExecutorThread = new Thread(AsyncExecutor.Run);
                this.AsyncExecutorThread.Start();
                new Thread(this.ShowProgress).Start();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                MessageBox.Show(msg);
            }
        }

        public void ShowProgress ()
        {
            this.buttonRun.Enabled = false;
            this.buttonTestConnection.Enabled = false;
            this.textBoxDatabaseName.Enabled = false;
            this.textBoxKey.Enabled = false;
            this.textBoxPort.Enabled = false;
            this.textBoxServerName.Enabled = false;
            this.textBoxUserName.Enabled = false;
            this.comboBoxDatabase.Enabled = false;
            this.comboBoxAuthType.Enabled = false;
            this.textBoxLoginName.Enabled = false;
            this.textBoxLoginPassword.Enabled = false;

            string form1Text = this.Text;
            this.Text = form1Text + " - Running - Please Wait ... ";

            string workingOn = string.Empty;
            double done = 0;
            while (this.AsyncExecutorThread.IsAlive)
            {
                Thread.Sleep(100);
                done = this.AsyncExecutor.MyExecutor.ProgressInfo.GetPercentDone(out workingOn);
                this.progressBarCalc.Value = (int)done;
                this.Text = form1Text + " - Running " + workingOn;
            }
            this.progressBarCalc.Value = 0;
            this.buttonRun.Enabled = true;
            this.buttonTestConnection.Enabled = true;
            this.textBoxDatabaseName.Enabled = true;
            this.textBoxKey.Enabled = true;
            this.textBoxPort.Enabled = true;
            this.textBoxServerName.Enabled = true;
            this.textBoxUserName.Enabled = true;
            this.comboBoxDatabase.Enabled = true;
            this.comboBoxAuthType.Enabled = true;
            this.textBoxLoginName.Enabled = true;
            this.textBoxLoginPassword.Enabled = true;

            this.Text = form1Text;
        //
            this.AsyncExecutor = null;
            this.AsyncExecutorThread = null;

        }


        private void buttonSendFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Multiselect = true;
            fdlg.ShowDialog();
            List<string> result = fdlg.FileNames.ToList();

            if (result.Count > 0)
            {
                string form1Text = this.Text;

                this.Text = form1Text + " - sending...";
                try
                {
                    new Executor(new DBExecutor()).SendFiles(result, this.textBoxKey.Text.ToString());
                    MessageBox.Show("Files sent successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Files were not sent!"+ ex.ToString());
                }
                this.Text = form1Text;

            }
        }

        private void comboBoxAuthType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.EnableAuthSettings();
        }

        private void buttonTestConnection_Click(object sender, EventArgs e)
        {
            DBExecutor dbExecutor = new DBExecutor();
            string connection = this.BuildConnectionString(dbExecutor);

            try
            {

                dbExecutor.Connect();
                dbExecutor.Close();
                this.buttonRun.Enabled = true;
                this.SaveDialogSettings();
                MessageBox.Show("Database connected!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database not connected! \n\nConnection string:\n" + connection + "\n\nError: " + ex.ToString());
            }
        }
    }
}

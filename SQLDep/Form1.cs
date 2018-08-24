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
            this.textBoxDefaultDatabase.Text = UIConfig.Get(UIConfig.FS_DEFAULT_DB, "");
            this.textBoxDefautSchema.Text = UIConfig.Get(UIConfig.FS_DEFAULT_SCHEMA, "");
            this.textBoxRootDirectory.Text = UIConfig.Get(UIConfig.FS_PATH, "");
            this.textBoxFileMask.Text = UIConfig.Get(UIConfig.FS_MASK, "*.sql");
            this.buttonRun.Enabled = false;
            this.buttonCreateAndSendFiles.Enabled = false;
            this.InitializeDSNNames(string.Empty);
            this.InitializeDrivers(UIConfig.Get(UIConfig.DRIVER_NAME, ""));

            this.EnableAuthSettings();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void InitializeDSNNames (string defaultDSNName)
        {
            string sqlDialect = this.GetDatabaseTypeName(this.comboBoxDatabase.SelectedIndex);

            List<ComboBoxDSNItem> comboItems = new List<ComboBoxDSNItem>();
            List<string> dsnNames = ODBCUtils.GetDSNNames();

            comboItems.Add(new ComboBoxDSNItem() { Text = "", IsDSN = false });

            // add ODBC drivers 
            foreach (var item in dsnNames)
            {
                comboItems.Add(new ComboBoxDSNItem() { Text = item, IsDSN = true });
            }

            this.comboBoxDSNName.Items.Clear();

            foreach (var item in comboItems)
            {
                this.comboBoxDSNName.Items.Add(item);
            }

            comboBoxDSNName.DisplayMember = "Text";
            comboBoxDSNName.ValueMember = "IsDSN";

            comboBoxDSNName.SelectedIndex = 0;

            // preselect driver
            int idx = 0;
            foreach (ComboBoxDSNItem item in comboBoxDSNName.Items)
            {
                if (defaultDSNName == item.Text)
                {
                    comboBoxDSNName.SelectedIndex = idx;
                }

                idx++;
            }

            if (comboBoxDSNName.SelectedIndex < 0)
            {
                comboBoxDSNName.SelectedIndex = 0;
            }
        }

        private void InitializeDrivers(string defaultDriverName)
        {
            string sqlDialect = this.GetDatabaseTypeName(this.comboBoxDatabase.SelectedIndex);
            ComboBoxDSNItem dsnItem = this.GetSelectedDSNName();
            this.comboBoxDriverName.Items.Clear();

            if (dsnItem.IsDSN)
            {
                this.comboBoxDriverName.Enabled = false;
                this.textBoxDatabaseName.Enabled = false;
                this.textBoxPort.Enabled = false;
                this.textBoxServerName.Enabled = false;
            }
            else
            {
                this.comboBoxDriverName.Enabled = true;
                this.textBoxDatabaseName.Enabled = true;
                this.textBoxPort.Enabled = true;
                this.textBoxServerName.Enabled = true;
            }

            List<ComboBoxDriverItem> comboItems = new List<ComboBoxDriverItem>();
            List<string> odbcDrivers = null;

            switch (sqlDialect)
            {
                case "oracle":
                    comboItems.Add(new ComboBoxDriverItem() { Text = UIConfig.DRIVER_NAME_NATIVE, UseDriverType = DBExecutor.UseDriver.ORACLE });
                    odbcDrivers = ODBCUtils.GetSystemDriverList().Where(x => x.IndexOf("oracle", StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
                    break;
                case "teradata":
                    comboItems.Add(new ComboBoxDriverItem() { Text = UIConfig.DRIVER_NAME_NATIVE, UseDriverType = DBExecutor.UseDriver.TERADATA });
                    odbcDrivers = ODBCUtils.GetSystemDriverList().Where(x => x.IndexOf("teradata", StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
                    break;
                case "mssql":
                    odbcDrivers = ODBCUtils.GetSystemDriverList().Where(x => x.IndexOf("SQL", StringComparison.InvariantCultureIgnoreCase) >=0).ToList();
                    break;
                case "greenplum":
                case "postgres":
                case "redshift":
                    comboItems.Add(new ComboBoxDriverItem() { Text = UIConfig.DRIVER_NAME_NATIVE, UseDriverType = DBExecutor.UseDriver.POSTGRESQL });
                    odbcDrivers = ODBCUtils.GetSystemDriverList().Where(x => x.IndexOf("postgres", StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
                    break;
                default:
                    odbcDrivers = new List<string>();
                    break;
            }

            // add ODBC drivers 
            foreach (var item in odbcDrivers)
            {
                comboItems.Add(new ComboBoxDriverItem() { Text = item, UseDriverType = DBExecutor.UseDriver.ODBC });
            }

            foreach (var item in comboItems)
            {
                this.comboBoxDriverName.Items.Add(item);
            }

            comboBoxDriverName.DisplayMember = "Text";
            comboBoxDriverName.ValueMember = "Value";

            comboBoxDriverName.SelectedIndex = -1;

            // preselect driver
            int idx = 0;
            foreach(ComboBoxDriverItem item in comboBoxDriverName.Items)
            {
                if(defaultDriverName == item.Text)
                {
                    if (idx != comboBoxDriverName.SelectedIndex)
                    {
                        comboBoxDriverName.SelectedIndex = idx;
                    }
                    else break;
                }

                idx++;
            }

            if(comboBoxDriverName.SelectedIndex < 0)
            {
                comboBoxDriverName.SelectedIndex = 0;
            }
        }

        public void EnableAuthSettings()
        {
            if ( this.GetAuthTypeName(this.comboBoxAuthType.SelectedIndex) == "sql_auth")
            {
                this.textBoxLoginName.Enabled = true;
                this.textBoxLoginPassword.Enabled = true;
            }
            else if (this.GetAuthTypeName(this.comboBoxAuthType.SelectedIndex) == "win_auth")
            {
                this.textBoxLoginName.Enabled = false;
                this.textBoxLoginPassword.Enabled = false;
            }
            else
            {
                this.textBoxLoginName.Enabled = false;
                this.textBoxLoginPassword.Enabled = false;
            }
        }

        public void EnableFileSystem()
        {
            if (this.textBoxRootDirectory.Enabled)
            {
                this.textBoxRootDirectory.Enabled = false;
                this.textBoxDefautSchema.Enabled = false;
                this.textBoxDefaultDatabase.Enabled = false;
                this.textBoxFileMask.Enabled = false;
                this.ButtonBrowse.Enabled = false;
            }
            else
            {
                this.textBoxRootDirectory.Enabled = true;
                this.textBoxDefautSchema.Enabled = true;
                this.textBoxDefaultDatabase.Enabled = true;
                this.textBoxFileMask.Enabled = true;
                this.ButtonBrowse.Enabled = true;
            }
        }

        private int GetDatabaseTypeIdx (string sqlDialect)
        {
            switch (sqlDialect)
            {
                case "teradata": return 2;
                case "oracle": return 0;
                default: return 1;
            }
        }

        private string GetDatabaseTypeName(int idx)
        {
            switch (idx)
            {
                case 0: return "oracle";
                case 1: return "mssql";
                case 2: return "teradata";
                case 3: return "greenplum";
                case 4: return "redshift";
                case 5: return "postgres";
                default: return "mssql";
            }
        }

        private string GetDriverName(out DBExecutor.UseDriver value)
        {
            int idx = this.comboBoxDriverName.SelectedIndex;

            if (idx >= 0 && idx < this.comboBoxDriverName.Items.Count)
            {
                SQLDep.ComboBoxDriverItem item = (SQLDep.ComboBoxDriverItem) this.comboBoxDriverName.Items[idx];
                value = item.UseDriverType;
                return item.Text;
            }
            else
            {
                value = DBExecutor.UseDriver.DEFAULT;
                return string.Empty;
            }
        }

        private ComboBoxDSNItem GetSelectedDSNName()
        {
            int idx = this.comboBoxDSNName.SelectedIndex;

            if (idx >= 0 && idx < this.comboBoxDSNName.Items.Count)
            {
                return (ComboBoxDSNItem)this.comboBoxDSNName.Items[idx];
            }
            else
            {
                return new ComboBoxDSNItem() { Text = string.Empty, IsDSN = false };
            }
        }

        private int GetAuthTypeIdx(string authType)
        {
            if (authType == "sql_auth")
            {
                return 0;
            }
            else if (authType == "win_auth")
            {
                return 1;
            }
            else return 2;
        }

        private string GetAuthTypeName(int idx)
        {
            if (idx == 0)
            {
                return "sql_auth";
            }
            else if (idx == 1)
            {
                return "win_auth";
            }
            else
            {
                return "dsn_auth";
            }
        }

        private string BuildConnectionString (DBExecutor dbExecutor)
        {
            DBExecutor.UseDriver useDriverType;
            string driverName = this.GetDriverName(out useDriverType);

            ComboBoxDSNItem dsnItem = this.GetSelectedDSNName();
            string dsn = dsnItem.IsDSN ? dsnItem.Text : string.Empty;
            return dbExecutor.BuildConnectionString(this.GetDatabaseTypeName(this.comboBoxDatabase.SelectedIndex),
                                                dsn,
                                                this.GetAuthTypeName(this.comboBoxAuthType.SelectedIndex),
                                                this.textBoxServerName.Text,
                                                this.textBoxPort.Text, 
                                                this.textBoxDatabaseName.Text,
                                                this.textBoxLoginName.Text,
                                                this.textBoxLoginPassword.Text,
                                                driverName, useDriverType);
        }

        private void SaveDialogSettings ()
        {
            DBExecutor.UseDriver useDriverType;
            UIConfig.Set(UIConfig.AUTH_TYPE, this.GetAuthTypeName(this.comboBoxAuthType.SelectedIndex));
            UIConfig.Set(UIConfig.SQL_DIALECT, this.GetDatabaseTypeName(this.comboBoxDatabase.SelectedIndex));
            UIConfig.Set(UIConfig.DATA_SET_NAME, this.textBoxUserName.Text);
            UIConfig.Set(UIConfig.SQLDEP_KEY, this.textBoxKey.Text);
            UIConfig.Set(UIConfig.SERVER_NAME, this.textBoxServerName.Text);
            UIConfig.Set(UIConfig.SERVER_PORT, this.textBoxPort.Text);
            UIConfig.Set(UIConfig.LOGIN_NAME, this.textBoxLoginName.Text);
            UIConfig.Set(UIConfig.LOGIN_PASSWORD, this.textBoxLoginPassword.Text);
            UIConfig.Set(UIConfig.DATABASE_NAME, this.textBoxDatabaseName.Text);
            UIConfig.Set(UIConfig.DRIVER_NAME, this.GetDriverName(out useDriverType));
            UIConfig.Set(UIConfig.FS_PATH, this.textBoxRootDirectory.Text);
            UIConfig.Set(UIConfig.FS_MASK, this.textBoxFileMask.Text);
            UIConfig.Set(UIConfig.FS_DEFAULT_SCHEMA, this.textBoxDefautSchema.Text);
            UIConfig.Set(UIConfig.FS_DEFAULT_DB, this.textBoxDefaultDatabase.Text);
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

            createJson();

        }

        private void createJson(bool sendFiles = false)
        {
            string saveFolder;
            if (sendFiles)
            {
                saveFolder = Path.GetTempPath();
            }
            else
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                DialogResult result = fbd.ShowDialog();

                // export cancelled - do nothing
                if (result != DialogResult.OK)
                    return;

                saveFolder = fbd.SelectedPath;
            }

            try
            {
                this.SaveDialogSettings();
                DBExecutor dbExecutor = new DBExecutor();
                this.BuildConnectionString(dbExecutor);

                // create filesystem configuration file, Executor will use this
                if (checkboxUseFS.Checked)
                    CreateFileSystemCfgFile();

                string myName = this.textBoxUserName.Text;
                Guid myKey;
                if (!Guid.TryParse(this.textBoxKey.Text, out myKey))
                {
                    throw new SQLDepException("Invalid or missing API key. Get one in your dashboard on SQLdep website.");
                }

                string sqlDialect = this.GetDatabaseTypeName(this.comboBoxDatabase.SelectedIndex);

                List<string> failedDbs = new List<string>();
                Executor executor = ExecutorFactory.CreateExecutor(dbExecutor, sqlDialect);
                string filename = "DBexport_" + executor.runId + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".json";
                string exportFileName = Path.Combine(saveFolder, filename);

                this.AsyncExecutor = new AsyncExecutor(myName, myKey, sqlDialect, exportFileName, executor, checkboxUseFS.Checked, sendFiles);
                this.AsyncExecutorThread = new Thread(AsyncExecutor.Run);
                this.AsyncExecutorThread.Start();
                new Thread(this.ShowProgress).Start();
            }
            catch (SQLDepException ex)
            {
                // known errors - do not show details about stack
                string msg = ex.Message;
                MessageBox.Show(msg);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                MessageBox.Show(msg);
            }
        }

        private void CreateFileSystemCfgFile()
        {
            FileSystemData fsData = new FileSystemData();
            fsData.ConfFile.DefaultDatabase = this.textBoxDefaultDatabase.Text;
            fsData.ConfFile.DefaultSchema = this.textBoxDefautSchema.Text;
            fsData.ConfFile.InputDir = this.textBoxRootDirectory.Text;
            fsData.ConfFile.FileMask = this.textBoxFileMask.Text;
            fsData.Save();
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
                this.Text = form1Text + " - " + workingOn;
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
            this.AsyncExecutor = null;
            this.AsyncExecutorThread = null;

        }


        private void buttonCreateAndSendFiles_Click(object sender, EventArgs e)
        {
            createJson(true);
        }

        private void comboBoxAuthType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.EnableAuthSettings();
        }

        private void buttonTestConnection_Click(object sender, EventArgs e)
        {
            DBExecutor dbExecutor = new DBExecutor();
            this.BuildConnectionString(dbExecutor);
            try
            {
                dbExecutor.Connect();
                dbExecutor.Close();
                this.buttonRun.Enabled = true;
                this.buttonCreateAndSendFiles.Enabled = true;
                this.SaveDialogSettings();
                MessageBox.Show("Database connected!");
            }
            catch (Exception ex)
            {
                this.buttonRun.Enabled = false;
                this.buttonCreateAndSendFiles.Enabled = false;
                MessageBox.Show("Database not connected! \nError: " + ex.Message);
            }
        }

        private void comboBoxDriverName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonRun.Enabled = false;
            this.buttonCreateAndSendFiles.Enabled = false;
        }

        private void comboBoxDSNName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.InitializeDrivers(UIConfig.Get(UIConfig.DRIVER_NAME, ""));
            this.buttonRun.Enabled = false;
            this.buttonCreateAndSendFiles.Enabled = false;
        }

        private void comboBoxDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.InitializeDSNNames(string.Empty);
            this.InitializeDrivers(UIConfig.Get(UIConfig.DRIVER_NAME, ""));
            this.buttonRun.Enabled = false;
            this.buttonCreateAndSendFiles.Enabled = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.EnableFileSystem();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var FD = new FolderBrowserDialog();
            if (FD.ShowDialog() == DialogResult.OK)
            {
                textBoxRootDirectory.Text = FD.SelectedPath;
            }
        }

        private void buttonSendOnly_Click(object sender, EventArgs e)
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
                    ExecutorFactory.CreateExecutor(new DBExecutor(), string.Empty).SendFiles(result, this.textBoxKey.Text);
                    MessageBox.Show("Files sent successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Files were not sent! " + ex.Message);
                }
                this.Text = form1Text;
            }
        }
    }
}

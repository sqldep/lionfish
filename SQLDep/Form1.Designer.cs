namespace SQLDep
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.comboBoxDatabase = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonRun = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxKey = new System.Windows.Forms.TextBox();
            this.textBoxBatchName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.AuthenticationLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxAuthType = new System.Windows.Forms.ComboBox();
            this.textBoxLoginPassword = new System.Windows.Forms.TextBox();
            this.textBoxServerName = new System.Windows.Forms.TextBox();
            this.textBoxLoginName = new System.Windows.Forms.TextBox();
            this.buttonTestConnection = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxDatabaseName = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBarCalc = new System.Windows.Forms.ProgressBar();
            this.buttonCreateAndSendFiles = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxDriverName = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBoxDSNName = new System.Windows.Forms.ComboBox();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageBasic = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelRole = new System.Windows.Forms.Label();
            this.labelWarehouse = new System.Windows.Forms.Label();
            this.textBoxRole = new System.Windows.Forms.TextBox();
            this.textBoxWarehouse = new System.Windows.Forms.TextBox();
            this.TextBoxAccount = new System.Windows.Forms.TextBox();
            this.buttonSendOnly = new System.Windows.Forms.Button();
            this.tabPageAdvanced = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.Filesystem = new System.Windows.Forms.TabPage();
            this.ButtonBrowse = new System.Windows.Forms.Button();
            this.textBoxRootDirectory = new System.Windows.Forms.TextBox();
            this.textBoxFileMask = new System.Windows.Forms.TextBox();
            this.textBoxDefautSchema = new System.Windows.Forms.TextBox();
            this.textBoxDefaultDatabase = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.checkboxUseFS = new System.Windows.Forms.CheckBox();
            this.tabExternalFiles = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabInformatica = new System.Windows.Forms.TabPage();
            this.buttonInfaDirBrowse = new System.Windows.Forms.Button();
            this.textInformaticaRootDir = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.checkBoxInformaticaEnable = new System.Windows.Forms.CheckBox();
            this.tabSSIS = new System.Windows.Forms.TabPage();
            this.buttonSSASBrowse = new System.Windows.Forms.Button();
            this.textSSISDir = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.checkBoxSSISEnable = new System.Windows.Forms.CheckBox();
            this.tabSAP = new System.Windows.Forms.TabPage();
            this.buttonSAPBrowse = new System.Windows.Forms.Button();
            this.textSAPDir = new System.Windows.Forms.TextBox();
            this.checkBoxSAPEnable = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.textBoxSAPMask = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.textBoxSSISMask = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.textBoxInfaMask = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.tabControlMain.SuspendLayout();
            this.tabPageBasic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPageAdvanced.SuspendLayout();
            this.Filesystem.SuspendLayout();
            this.tabExternalFiles.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabInformatica.SuspendLayout();
            this.tabSSIS.SuspendLayout();
            this.tabSAP.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxDatabase
            // 
            this.comboBoxDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDatabase.FormattingEnabled = true;
            this.comboBoxDatabase.Items.AddRange(new object[] {
            "Oracle",
            "MsSQL",
            "Teradata",
            "Greenplum",
            "Amazon Redshift",
            "PostgreSQL",
            "Snowflake"});
            this.comboBoxDatabase.Location = new System.Drawing.Point(103, 16);
            this.comboBoxDatabase.Name = "comboBoxDatabase";
            this.comboBoxDatabase.Size = new System.Drawing.Size(234, 21);
            this.comboBoxDatabase.TabIndex = 0;
            this.comboBoxDatabase.SelectedIndexChanged += new System.EventHandler(this.comboBoxDatabase_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Database Vendor";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(354, 190);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(111, 32);
            this.buttonRun.TabIndex = 10;
            this.buttonRun.Text = "Extract to file";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 279);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "API Key";
            // 
            // textBoxKey
            // 
            this.textBoxKey.Location = new System.Drawing.Point(103, 276);
            this.textBoxKey.Name = "textBoxKey";
            this.textBoxKey.Size = new System.Drawing.Size(234, 20);
            this.textBoxKey.TabIndex = 8;
            // 
            // textBoxBatchName
            // 
            this.textBoxBatchName.Location = new System.Drawing.Point(103, 302);
            this.textBoxBatchName.Name = "textBoxBatchName";
            this.textBoxBatchName.Size = new System.Drawing.Size(234, 20);
            this.textBoxBatchName.TabIndex = 9;
            this.textBoxBatchName.Text = "Your set name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 305);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Hostname";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AuthenticationLabel
            // 
            this.AuthenticationLabel.AutoSize = true;
            this.AuthenticationLabel.Location = new System.Drawing.Point(12, 103);
            this.AuthenticationLabel.Name = "AuthenticationLabel";
            this.AuthenticationLabel.Size = new System.Drawing.Size(75, 13);
            this.AuthenticationLabel.TabIndex = 1;
            this.AuthenticationLabel.Text = "Authentication";
            this.AuthenticationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(46, 132);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "User name";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(46, 160);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Password";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxAuthType
            // 
            this.comboBoxAuthType.FormattingEnabled = true;
            this.comboBoxAuthType.Items.AddRange(new object[] {
            "Server Authentication",
            "Windows Authentication ",
            "Authentication DSN"});
            this.comboBoxAuthType.Location = new System.Drawing.Point(103, 102);
            this.comboBoxAuthType.Name = "comboBoxAuthType";
            this.comboBoxAuthType.Size = new System.Drawing.Size(234, 21);
            this.comboBoxAuthType.TabIndex = 4;
            this.comboBoxAuthType.SelectedIndexChanged += new System.EventHandler(this.comboBoxAuthType_SelectedIndexChanged);
            // 
            // textBoxLoginPassword
            // 
            this.textBoxLoginPassword.Location = new System.Drawing.Point(128, 156);
            this.textBoxLoginPassword.Name = "textBoxLoginPassword";
            this.textBoxLoginPassword.PasswordChar = '*';
            this.textBoxLoginPassword.Size = new System.Drawing.Size(209, 20);
            this.textBoxLoginPassword.TabIndex = 6;
            // 
            // textBoxServerName
            // 
            this.textBoxServerName.Location = new System.Drawing.Point(103, 50);
            this.textBoxServerName.Name = "textBoxServerName";
            this.textBoxServerName.Size = new System.Drawing.Size(234, 20);
            this.textBoxServerName.TabIndex = 2;
            // 
            // textBoxLoginName
            // 
            this.textBoxLoginName.Location = new System.Drawing.Point(128, 128);
            this.textBoxLoginName.Name = "textBoxLoginName";
            this.textBoxLoginName.Size = new System.Drawing.Size(209, 20);
            this.textBoxLoginName.TabIndex = 5;
            // 
            // buttonTestConnection
            // 
            this.buttonTestConnection.Location = new System.Drawing.Point(354, 16);
            this.buttonTestConnection.Name = "buttonTestConnection";
            this.buttonTestConnection.Size = new System.Drawing.Size(111, 32);
            this.buttonTestConnection.TabIndex = 7;
            this.buttonTestConnection.Text = "Test Connection";
            this.buttonTestConnection.UseVisualStyleBackColor = true;
            this.buttonTestConnection.Click += new System.EventHandler(this.buttonTestConnection_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 186);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Database (SID)";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxDatabaseName
            // 
            this.textBoxDatabaseName.Location = new System.Drawing.Point(103, 183);
            this.textBoxDatabaseName.Name = "textBoxDatabaseName";
            this.textBoxDatabaseName.Size = new System.Drawing.Size(234, 20);
            this.textBoxDatabaseName.TabIndex = 7;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(103, 76);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(234, 20);
            this.textBoxPort.TabIndex = 3;
            this.textBoxPort.Text = "1523";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Port";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBarCalc
            // 
            this.progressBarCalc.Location = new System.Drawing.Point(6, 381);
            this.progressBarCalc.Name = "progressBarCalc";
            this.progressBarCalc.Size = new System.Drawing.Size(480, 29);
            this.progressBarCalc.TabIndex = 11;
            // 
            // buttonCreateAndSendFiles
            // 
            this.buttonCreateAndSendFiles.Location = new System.Drawing.Point(354, 290);
            this.buttonCreateAndSendFiles.Name = "buttonCreateAndSendFiles";
            this.buttonCreateAndSendFiles.Size = new System.Drawing.Size(111, 32);
            this.buttonCreateAndSendFiles.TabIndex = 10;
            this.buttonCreateAndSendFiles.Text = "Extract && Send";
            this.buttonCreateAndSendFiles.UseVisualStyleBackColor = true;
            this.buttonCreateAndSendFiles.Click += new System.EventHandler(this.buttonCreateAndSendFiles_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "DriverName";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxDriverName
            // 
            this.comboBoxDriverName.FormattingEnabled = true;
            this.comboBoxDriverName.Location = new System.Drawing.Point(104, 50);
            this.comboBoxDriverName.Name = "comboBoxDriverName";
            this.comboBoxDriverName.Size = new System.Drawing.Size(234, 21);
            this.comboBoxDriverName.TabIndex = 1;
            this.comboBoxDriverName.SelectedIndexChanged += new System.EventHandler(this.comboBoxDriverName_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "DSN Name";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxDSNName
            // 
            this.comboBoxDSNName.FormattingEnabled = true;
            this.comboBoxDSNName.Location = new System.Drawing.Point(104, 20);
            this.comboBoxDSNName.Name = "comboBoxDSNName";
            this.comboBoxDSNName.Size = new System.Drawing.Size(234, 21);
            this.comboBoxDSNName.TabIndex = 1;
            this.comboBoxDSNName.SelectedIndexChanged += new System.EventHandler(this.comboBoxDSNName_SelectedIndexChanged);
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageBasic);
            this.tabControlMain.Controls.Add(this.tabPageAdvanced);
            this.tabControlMain.Controls.Add(this.Filesystem);
            this.tabControlMain.Controls.Add(this.tabExternalFiles);
            this.tabControlMain.Location = new System.Drawing.Point(6, 12);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(484, 363);
            this.tabControlMain.TabIndex = 12;
            // 
            // tabPageBasic
            // 
            this.tabPageBasic.Controls.Add(this.pictureBox1);
            this.tabPageBasic.Controls.Add(this.labelRole);
            this.tabPageBasic.Controls.Add(this.labelWarehouse);
            this.tabPageBasic.Controls.Add(this.textBoxRole);
            this.tabPageBasic.Controls.Add(this.textBoxWarehouse);
            this.tabPageBasic.Controls.Add(this.TextBoxAccount);
            this.tabPageBasic.Controls.Add(this.buttonSendOnly);
            this.tabPageBasic.Controls.Add(this.comboBoxDatabase);
            this.tabPageBasic.Controls.Add(this.label1);
            this.tabPageBasic.Controls.Add(this.textBoxPort);
            this.tabPageBasic.Controls.Add(this.label5);
            this.tabPageBasic.Controls.Add(this.label2);
            this.tabPageBasic.Controls.Add(this.AuthenticationLabel);
            this.tabPageBasic.Controls.Add(this.textBoxDatabaseName);
            this.tabPageBasic.Controls.Add(this.label7);
            this.tabPageBasic.Controls.Add(this.textBoxLoginName);
            this.tabPageBasic.Controls.Add(this.label8);
            this.tabPageBasic.Controls.Add(this.textBoxServerName);
            this.tabPageBasic.Controls.Add(this.label9);
            this.tabPageBasic.Controls.Add(this.textBoxLoginPassword);
            this.tabPageBasic.Controls.Add(this.buttonRun);
            this.tabPageBasic.Controls.Add(this.comboBoxAuthType);
            this.tabPageBasic.Controls.Add(this.buttonCreateAndSendFiles);
            this.tabPageBasic.Controls.Add(this.label4);
            this.tabPageBasic.Controls.Add(this.buttonTestConnection);
            this.tabPageBasic.Controls.Add(this.label3);
            this.tabPageBasic.Controls.Add(this.textBoxKey);
            this.tabPageBasic.Controls.Add(this.textBoxBatchName);
            this.tabPageBasic.Location = new System.Drawing.Point(4, 22);
            this.tabPageBasic.Name = "tabPageBasic";
            this.tabPageBasic.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBasic.Size = new System.Drawing.Size(476, 337);
            this.tabPageBasic.TabIndex = 0;
            this.tabPageBasic.Text = "Basic";
            this.tabPageBasic.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SQLDep.Properties.Resources.icons8_help_26;
            this.pictureBox1.Location = new System.Drawing.Point(64, 276);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(18, 18);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBox1, "Visit https://app.sqldep.com/queryflow/upload/api/ to obtain your API key.");
            // 
            // labelRole
            // 
            this.labelRole.AutoSize = true;
            this.labelRole.Location = new System.Drawing.Point(12, 238);
            this.labelRole.Name = "labelRole";
            this.labelRole.Size = new System.Drawing.Size(29, 13);
            this.labelRole.TabIndex = 16;
            this.labelRole.Text = "Role";
            this.labelRole.Visible = false;
            // 
            // labelWarehouse
            // 
            this.labelWarehouse.AutoSize = true;
            this.labelWarehouse.Location = new System.Drawing.Point(13, 212);
            this.labelWarehouse.Name = "labelWarehouse";
            this.labelWarehouse.Size = new System.Drawing.Size(62, 13);
            this.labelWarehouse.TabIndex = 15;
            this.labelWarehouse.Text = "Warehouse";
            this.labelWarehouse.Visible = false;
            // 
            // textBoxRole
            // 
            this.textBoxRole.Location = new System.Drawing.Point(103, 235);
            this.textBoxRole.Name = "textBoxRole";
            this.textBoxRole.Size = new System.Drawing.Size(234, 20);
            this.textBoxRole.TabIndex = 14;
            this.textBoxRole.Visible = false;
            // 
            // textBoxWarehouse
            // 
            this.textBoxWarehouse.Location = new System.Drawing.Point(103, 209);
            this.textBoxWarehouse.Name = "textBoxWarehouse";
            this.textBoxWarehouse.Size = new System.Drawing.Size(234, 20);
            this.textBoxWarehouse.TabIndex = 13;
            this.textBoxWarehouse.Visible = false;
            // 
            // TextBoxAccount
            // 
            this.TextBoxAccount.Location = new System.Drawing.Point(103, 103);
            this.TextBoxAccount.Name = "TextBoxAccount";
            this.TextBoxAccount.Size = new System.Drawing.Size(234, 20);
            this.TextBoxAccount.TabIndex = 12;
            this.TextBoxAccount.Visible = false;
            // 
            // buttonSendOnly
            // 
            this.buttonSendOnly.Location = new System.Drawing.Point(354, 228);
            this.buttonSendOnly.Name = "buttonSendOnly";
            this.buttonSendOnly.Size = new System.Drawing.Size(111, 32);
            this.buttonSendOnly.TabIndex = 11;
            this.buttonSendOnly.Text = "Send file(s)";
            this.buttonSendOnly.UseVisualStyleBackColor = true;
            this.buttonSendOnly.Click += new System.EventHandler(this.buttonSendOnly_Click);
            // 
            // tabPageAdvanced
            // 
            this.tabPageAdvanced.Controls.Add(this.label12);
            this.tabPageAdvanced.Controls.Add(this.comboBoxDSNName);
            this.tabPageAdvanced.Controls.Add(this.comboBoxDriverName);
            this.tabPageAdvanced.Controls.Add(this.label10);
            this.tabPageAdvanced.Controls.Add(this.label11);
            this.tabPageAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tabPageAdvanced.Name = "tabPageAdvanced";
            this.tabPageAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAdvanced.Size = new System.Drawing.Size(476, 337);
            this.tabPageAdvanced.TabIndex = 1;
            this.tabPageAdvanced.Text = "Advanced";
            this.tabPageAdvanced.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(13, 108);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(336, 100);
            this.label12.TabIndex = 2;
            this.label12.Text = "Setting these values is optional. Fill them out only if the basic setting does no" +
    "t work. For example if you want to use a different driver or a preconfigured DSN" +
    " connection.";
            // 
            // Filesystem
            // 
            this.Filesystem.Controls.Add(this.ButtonBrowse);
            this.Filesystem.Controls.Add(this.textBoxRootDirectory);
            this.Filesystem.Controls.Add(this.textBoxFileMask);
            this.Filesystem.Controls.Add(this.textBoxDefautSchema);
            this.Filesystem.Controls.Add(this.textBoxDefaultDatabase);
            this.Filesystem.Controls.Add(this.label16);
            this.Filesystem.Controls.Add(this.label15);
            this.Filesystem.Controls.Add(this.label14);
            this.Filesystem.Controls.Add(this.label13);
            this.Filesystem.Controls.Add(this.checkboxUseFS);
            this.Filesystem.Location = new System.Drawing.Point(4, 22);
            this.Filesystem.Name = "Filesystem";
            this.Filesystem.Padding = new System.Windows.Forms.Padding(3);
            this.Filesystem.Size = new System.Drawing.Size(476, 337);
            this.Filesystem.TabIndex = 2;
            this.Filesystem.Text = "File system";
            this.Filesystem.UseVisualStyleBackColor = true;
            // 
            // ButtonBrowse
            // 
            this.ButtonBrowse.Enabled = false;
            this.ButtonBrowse.Location = new System.Drawing.Point(327, 50);
            this.ButtonBrowse.Name = "ButtonBrowse";
            this.ButtonBrowse.Size = new System.Drawing.Size(75, 23);
            this.ButtonBrowse.TabIndex = 6;
            this.ButtonBrowse.Text = "Browse";
            this.ButtonBrowse.UseVisualStyleBackColor = true;
            this.ButtonBrowse.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // textBoxRootDirectory
            // 
            this.textBoxRootDirectory.Enabled = false;
            this.textBoxRootDirectory.Location = new System.Drawing.Point(125, 52);
            this.textBoxRootDirectory.Name = "textBoxRootDirectory";
            this.textBoxRootDirectory.Size = new System.Drawing.Size(196, 20);
            this.textBoxRootDirectory.TabIndex = 5;
            // 
            // textBoxFileMask
            // 
            this.textBoxFileMask.Enabled = false;
            this.textBoxFileMask.Location = new System.Drawing.Point(125, 77);
            this.textBoxFileMask.Name = "textBoxFileMask";
            this.textBoxFileMask.Size = new System.Drawing.Size(138, 20);
            this.textBoxFileMask.TabIndex = 7;
            // 
            // textBoxDefautSchema
            // 
            this.textBoxDefautSchema.Enabled = false;
            this.textBoxDefautSchema.Location = new System.Drawing.Point(125, 102);
            this.textBoxDefautSchema.Name = "textBoxDefautSchema";
            this.textBoxDefautSchema.Size = new System.Drawing.Size(138, 20);
            this.textBoxDefautSchema.TabIndex = 8;
            // 
            // textBoxDefaultDatabase
            // 
            this.textBoxDefaultDatabase.Enabled = false;
            this.textBoxDefaultDatabase.Location = new System.Drawing.Point(125, 127);
            this.textBoxDefaultDatabase.Name = "textBoxDefaultDatabase";
            this.textBoxDefaultDatabase.Size = new System.Drawing.Size(138, 20);
            this.textBoxDefaultDatabase.TabIndex = 9;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(30, 130);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(88, 13);
            this.label16.TabIndex = 4;
            this.label16.Text = "Default database";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(30, 105);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(81, 13);
            this.label15.TabIndex = 3;
            this.label15.Text = "Default schema";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(30, 80);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(51, 13);
            this.label14.TabIndex = 2;
            this.label14.Text = "File mask";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(30, 55);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Root directory";
            // 
            // checkboxUseFS
            // 
            this.checkboxUseFS.AutoSize = true;
            this.checkboxUseFS.Location = new System.Drawing.Point(13, 23);
            this.checkboxUseFS.Name = "checkboxUseFS";
            this.checkboxUseFS.Size = new System.Drawing.Size(207, 17);
            this.checkboxUseFS.TabIndex = 0;
            this.checkboxUseFS.Text = "Use file system as a query data source";
            this.checkboxUseFS.UseVisualStyleBackColor = true;
            this.checkboxUseFS.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tabExternalFiles
            // 
            this.tabExternalFiles.Controls.Add(this.tabControl1);
            this.tabExternalFiles.Controls.Add(this.label6);
            this.tabExternalFiles.Location = new System.Drawing.Point(4, 22);
            this.tabExternalFiles.Name = "tabExternalFiles";
            this.tabExternalFiles.Size = new System.Drawing.Size(476, 337);
            this.tabExternalFiles.TabIndex = 3;
            this.tabExternalFiles.Text = "External files";
            this.tabExternalFiles.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabInformatica);
            this.tabControl1.Controls.Add(this.tabSSIS);
            this.tabControl1.Controls.Add(this.tabSAP);
            this.tabControl1.Location = new System.Drawing.Point(17, 40);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(440, 285);
            this.tabControl1.TabIndex = 4;
            // 
            // tabInformatica
            // 
            this.tabInformatica.Controls.Add(this.textBoxInfaMask);
            this.tabInformatica.Controls.Add(this.label22);
            this.tabInformatica.Controls.Add(this.buttonInfaDirBrowse);
            this.tabInformatica.Controls.Add(this.textInformaticaRootDir);
            this.tabInformatica.Controls.Add(this.label18);
            this.tabInformatica.Controls.Add(this.checkBoxInformaticaEnable);
            this.tabInformatica.Location = new System.Drawing.Point(4, 25);
            this.tabInformatica.Name = "tabInformatica";
            this.tabInformatica.Padding = new System.Windows.Forms.Padding(3);
            this.tabInformatica.Size = new System.Drawing.Size(432, 256);
            this.tabInformatica.TabIndex = 0;
            this.tabInformatica.Text = "Informatica";
            this.tabInformatica.UseVisualStyleBackColor = true;
            // 
            // buttonInfaDirBrowse
            // 
            this.buttonInfaDirBrowse.Location = new System.Drawing.Point(211, 52);
            this.buttonInfaDirBrowse.Name = "buttonInfaDirBrowse";
            this.buttonInfaDirBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonInfaDirBrowse.TabIndex = 9;
            this.buttonInfaDirBrowse.Text = "Browse";
            this.buttonInfaDirBrowse.UseVisualStyleBackColor = true;
            this.buttonInfaDirBrowse.Click += new System.EventHandler(this.buttonInfaDirBrowse_Click);
            // 
            // textInformaticaRootDir
            // 
            this.textInformaticaRootDir.Location = new System.Drawing.Point(9, 54);
            this.textInformaticaRootDir.Name = "textInformaticaRootDir";
            this.textInformaticaRootDir.Size = new System.Drawing.Size(196, 20);
            this.textInformaticaRootDir.TabIndex = 8;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 38);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(126, 13);
            this.label18.TabIndex = 7;
            this.label18.Text = "Informatica data directory";
            // 
            // checkBoxInformaticaEnable
            // 
            this.checkBoxInformaticaEnable.AutoSize = true;
            this.checkBoxInformaticaEnable.Location = new System.Drawing.Point(9, 6);
            this.checkBoxInformaticaEnable.Name = "checkBoxInformaticaEnable";
            this.checkBoxInformaticaEnable.Size = new System.Drawing.Size(59, 17);
            this.checkBoxInformaticaEnable.TabIndex = 1;
            this.checkBoxInformaticaEnable.Text = "Enable";
            this.checkBoxInformaticaEnable.UseVisualStyleBackColor = true;
            this.checkBoxInformaticaEnable.CheckedChanged += new System.EventHandler(this.checkBoxInformaticaEnable_CheckedChanged);
            // 
            // tabSSIS
            // 
            this.tabSSIS.Controls.Add(this.textBoxSSISMask);
            this.tabSSIS.Controls.Add(this.label21);
            this.tabSSIS.Controls.Add(this.buttonSSASBrowse);
            this.tabSSIS.Controls.Add(this.textSSISDir);
            this.tabSSIS.Controls.Add(this.label19);
            this.tabSSIS.Controls.Add(this.checkBoxSSISEnable);
            this.tabSSIS.Location = new System.Drawing.Point(4, 25);
            this.tabSSIS.Name = "tabSSIS";
            this.tabSSIS.Padding = new System.Windows.Forms.Padding(3);
            this.tabSSIS.Size = new System.Drawing.Size(432, 256);
            this.tabSSIS.TabIndex = 1;
            this.tabSSIS.Text = "SSIS";
            this.tabSSIS.UseVisualStyleBackColor = true;
            // 
            // buttonSSASBrowse
            // 
            this.buttonSSASBrowse.Location = new System.Drawing.Point(212, 54);
            this.buttonSSASBrowse.Name = "buttonSSASBrowse";
            this.buttonSSASBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonSSASBrowse.TabIndex = 12;
            this.buttonSSASBrowse.Text = "Browse";
            this.buttonSSASBrowse.UseVisualStyleBackColor = true;
            this.buttonSSASBrowse.Click += new System.EventHandler(this.buttonSSASBrowse_Click);
            // 
            // textSSISDir
            // 
            this.textSSISDir.Location = new System.Drawing.Point(10, 56);
            this.textSSISDir.Name = "textSSISDir";
            this.textSSISDir.Size = new System.Drawing.Size(196, 20);
            this.textSSISDir.TabIndex = 11;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(7, 40);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(78, 13);
            this.label19.TabIndex = 10;
            this.label19.Text = "SSAS directory";
            // 
            // checkBoxSSISEnable
            // 
            this.checkBoxSSISEnable.AutoSize = true;
            this.checkBoxSSISEnable.Location = new System.Drawing.Point(10, 6);
            this.checkBoxSSISEnable.Name = "checkBoxSSISEnable";
            this.checkBoxSSISEnable.Size = new System.Drawing.Size(59, 17);
            this.checkBoxSSISEnable.TabIndex = 2;
            this.checkBoxSSISEnable.Text = "Enable";
            this.checkBoxSSISEnable.UseVisualStyleBackColor = true;
            this.checkBoxSSISEnable.CheckedChanged += new System.EventHandler(this.checkBoxSSISEnable_CheckedChanged);
            // 
            // tabSAP
            // 
            this.tabSAP.Controls.Add(this.textBoxSAPMask);
            this.tabSAP.Controls.Add(this.label17);
            this.tabSAP.Controls.Add(this.buttonSAPBrowse);
            this.tabSAP.Controls.Add(this.textSAPDir);
            this.tabSAP.Controls.Add(this.checkBoxSAPEnable);
            this.tabSAP.Controls.Add(this.label20);
            this.tabSAP.Location = new System.Drawing.Point(4, 25);
            this.tabSAP.Name = "tabSAP";
            this.tabSAP.Size = new System.Drawing.Size(432, 256);
            this.tabSAP.TabIndex = 2;
            this.tabSAP.Text = "SAP";
            this.tabSAP.UseVisualStyleBackColor = true;
            // 
            // buttonSAPBrowse
            // 
            this.buttonSAPBrowse.Location = new System.Drawing.Point(213, 55);
            this.buttonSAPBrowse.Name = "buttonSAPBrowse";
            this.buttonSAPBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonSAPBrowse.TabIndex = 15;
            this.buttonSAPBrowse.Text = "Browse";
            this.buttonSAPBrowse.UseVisualStyleBackColor = true;
            this.buttonSAPBrowse.Click += new System.EventHandler(this.buttonSAPBrowse_Click);
            // 
            // textSAPDir
            // 
            this.textSAPDir.Location = new System.Drawing.Point(11, 57);
            this.textSAPDir.Name = "textSAPDir";
            this.textSAPDir.Size = new System.Drawing.Size(196, 20);
            this.textSAPDir.TabIndex = 14;
            // 
            // checkBoxSAPEnable
            // 
            this.checkBoxSAPEnable.AutoSize = true;
            this.checkBoxSAPEnable.Location = new System.Drawing.Point(11, 6);
            this.checkBoxSAPEnable.Name = "checkBoxSAPEnable";
            this.checkBoxSAPEnable.Size = new System.Drawing.Size(59, 17);
            this.checkBoxSAPEnable.TabIndex = 3;
            this.checkBoxSAPEnable.Text = "Enable";
            this.checkBoxSAPEnable.UseVisualStyleBackColor = true;
            this.checkBoxSAPEnable.CheckedChanged += new System.EventHandler(this.checkBoxSAPEnable_CheckedChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(8, 41);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(74, 13);
            this.label20.TabIndex = 13;
            this.label20.Text = "SAP  directory";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(211, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Configure external sources you wish to use:";
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "API key";
            // 
            // textBoxSAPMask
            // 
            this.textBoxSAPMask.Location = new System.Drawing.Point(11, 100);
            this.textBoxSAPMask.Name = "textBoxSAPMask";
            this.textBoxSAPMask.Size = new System.Drawing.Size(95, 20);
            this.textBoxSAPMask.TabIndex = 17;
            this.textBoxSAPMask.Text = "*";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 84);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(51, 13);
            this.label17.TabIndex = 16;
            this.label17.Text = "File mask";
            // 
            // textBoxSSISMask
            // 
            this.textBoxSSISMask.Location = new System.Drawing.Point(10, 97);
            this.textBoxSSISMask.Name = "textBoxSSISMask";
            this.textBoxSSISMask.Size = new System.Drawing.Size(95, 20);
            this.textBoxSSISMask.TabIndex = 19;
            this.textBoxSSISMask.Text = "*";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(7, 81);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(51, 13);
            this.label21.TabIndex = 18;
            this.label21.Text = "File mask";
            // 
            // textBoxInfaMask
            // 
            this.textBoxInfaMask.Location = new System.Drawing.Point(9, 96);
            this.textBoxInfaMask.Name = "textBoxInfaMask";
            this.textBoxInfaMask.Size = new System.Drawing.Size(95, 20);
            this.textBoxInfaMask.TabIndex = 19;
            this.textBoxInfaMask.Text = "*";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 80);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(51, 13);
            this.label22.TabIndex = 18;
            this.label22.Text = "File mask";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 422);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.progressBarCalc);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "SQLdep v1.7.0-beta";
            this.tabControlMain.ResumeLayout(false);
            this.tabPageBasic.ResumeLayout(false);
            this.tabPageBasic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPageAdvanced.ResumeLayout(false);
            this.tabPageAdvanced.PerformLayout();
            this.Filesystem.ResumeLayout(false);
            this.Filesystem.PerformLayout();
            this.tabExternalFiles.ResumeLayout(false);
            this.tabExternalFiles.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabInformatica.ResumeLayout(false);
            this.tabInformatica.PerformLayout();
            this.tabSSIS.ResumeLayout(false);
            this.tabSSIS.PerformLayout();
            this.tabSAP.ResumeLayout(false);
            this.tabSAP.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxDatabase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxKey;
        private System.Windows.Forms.TextBox textBoxBatchName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label AuthenticationLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxAuthType;
        private System.Windows.Forms.TextBox textBoxLoginPassword;
        private System.Windows.Forms.TextBox textBoxServerName;
        private System.Windows.Forms.TextBox textBoxLoginName;
        private System.Windows.Forms.Button buttonTestConnection;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxDatabaseName;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBarCalc;
        private System.Windows.Forms.Button buttonCreateAndSendFiles;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboBoxDriverName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBoxDSNName;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageBasic;
        private System.Windows.Forms.TabPage tabPageAdvanced;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TabPage Filesystem;
        private System.Windows.Forms.CheckBox checkboxUseFS;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBoxRootDirectory;
        private System.Windows.Forms.TextBox textBoxFileMask;
        private System.Windows.Forms.TextBox textBoxDefautSchema;
        private System.Windows.Forms.TextBox textBoxDefaultDatabase;
        private System.Windows.Forms.Button ButtonBrowse;
        private System.Windows.Forms.Button buttonSendOnly;
        private System.Windows.Forms.TextBox TextBoxAccount;
        private System.Windows.Forms.Label labelWarehouse;
        private System.Windows.Forms.TextBox textBoxRole;
        private System.Windows.Forms.TextBox textBoxWarehouse;
        private System.Windows.Forms.Label labelRole;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage tabExternalFiles;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxInformaticaEnable;
        private System.Windows.Forms.CheckBox checkBoxSAPEnable;
        private System.Windows.Forms.CheckBox checkBoxSSISEnable;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabInformatica;
        private System.Windows.Forms.TabPage tabSSIS;
        private System.Windows.Forms.TabPage tabSAP;
        private System.Windows.Forms.Button buttonInfaDirBrowse;
        private System.Windows.Forms.TextBox textInformaticaRootDir;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button buttonSSASBrowse;
        private System.Windows.Forms.TextBox textSSISDir;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button buttonSAPBrowse;
        private System.Windows.Forms.TextBox textSAPDir;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox textBoxSAPMask;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textBoxInfaMask;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox textBoxSSISMask;
        private System.Windows.Forms.Label label21;
    }
}


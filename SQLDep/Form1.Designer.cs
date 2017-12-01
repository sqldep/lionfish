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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.comboBoxDatabase = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonRun = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxKey = new System.Windows.Forms.TextBox();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
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
            this.buttonSendFiles = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxDriverName = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBoxDSNName = new System.Windows.Forms.ComboBox();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageBasic = new System.Windows.Forms.TabPage();
            this.tabPageAdvanced = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.tabControlMain.SuspendLayout();
            this.tabPageBasic.SuspendLayout();
            this.tabPageAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxDatabase
            // 
            this.comboBoxDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDatabase.FormattingEnabled = true;
            this.comboBoxDatabase.Items.AddRange(new object[] {
            "Oracle",
            "MsSQL",
            "Teradata"});
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
            this.buttonRun.Location = new System.Drawing.Point(355, 213);
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
            this.label3.Location = new System.Drawing.Point(13, 227);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "API Key";
            // 
            // textBoxKey
            // 
            this.textBoxKey.Location = new System.Drawing.Point(103, 224);
            this.textBoxKey.Name = "textBoxKey";
            this.textBoxKey.Size = new System.Drawing.Size(234, 20);
            this.textBoxKey.TabIndex = 8;
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(103, 250);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(234, 20);
            this.textBoxUserName.TabIndex = 9;
            this.textBoxUserName.Text = "Your set name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 253);
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Authentication";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.buttonTestConnection.Location = new System.Drawing.Point(355, 176);
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
            this.progressBarCalc.Location = new System.Drawing.Point(6, 340);
            this.progressBarCalc.Name = "progressBarCalc";
            this.progressBarCalc.Size = new System.Drawing.Size(480, 29);
            this.progressBarCalc.TabIndex = 11;
            // 
            // buttonSendFiles
            // 
            this.buttonSendFiles.Location = new System.Drawing.Point(354, 250);
            this.buttonSendFiles.Name = "buttonSendFiles";
            this.buttonSendFiles.Size = new System.Drawing.Size(111, 32);
            this.buttonSendFiles.TabIndex = 10;
            this.buttonSendFiles.Text = "Send file(s)";
            this.buttonSendFiles.UseVisualStyleBackColor = true;
            this.buttonSendFiles.Click += new System.EventHandler(this.buttonSendFiles_Click);
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
            this.tabControlMain.Location = new System.Drawing.Point(6, 12);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(484, 322);
            this.tabControlMain.TabIndex = 12;
            // 
            // tabPageBasic
            // 
            this.tabPageBasic.Controls.Add(this.comboBoxDatabase);
            this.tabPageBasic.Controls.Add(this.label1);
            this.tabPageBasic.Controls.Add(this.textBoxPort);
            this.tabPageBasic.Controls.Add(this.label5);
            this.tabPageBasic.Controls.Add(this.label2);
            this.tabPageBasic.Controls.Add(this.label6);
            this.tabPageBasic.Controls.Add(this.textBoxDatabaseName);
            this.tabPageBasic.Controls.Add(this.label7);
            this.tabPageBasic.Controls.Add(this.textBoxLoginName);
            this.tabPageBasic.Controls.Add(this.label8);
            this.tabPageBasic.Controls.Add(this.textBoxServerName);
            this.tabPageBasic.Controls.Add(this.label9);
            this.tabPageBasic.Controls.Add(this.textBoxLoginPassword);
            this.tabPageBasic.Controls.Add(this.buttonRun);
            this.tabPageBasic.Controls.Add(this.comboBoxAuthType);
            this.tabPageBasic.Controls.Add(this.buttonSendFiles);
            this.tabPageBasic.Controls.Add(this.label4);
            this.tabPageBasic.Controls.Add(this.buttonTestConnection);
            this.tabPageBasic.Controls.Add(this.label3);
            this.tabPageBasic.Controls.Add(this.textBoxKey);
            this.tabPageBasic.Controls.Add(this.textBoxUserName);
            this.tabPageBasic.Location = new System.Drawing.Point(4, 22);
            this.tabPageBasic.Name = "tabPageBasic";
            this.tabPageBasic.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBasic.Size = new System.Drawing.Size(476, 296);
            this.tabPageBasic.TabIndex = 0;
            this.tabPageBasic.Text = "Basic";
            this.tabPageBasic.UseVisualStyleBackColor = true;
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
            this.tabPageAdvanced.Size = new System.Drawing.Size(476, 296);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 377);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.progressBarCalc);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "SQLdep v1.5.13";
            this.tabControlMain.ResumeLayout(false);
            this.tabPageBasic.ResumeLayout(false);
            this.tabPageBasic.PerformLayout();
            this.tabPageAdvanced.ResumeLayout(false);
            this.tabPageAdvanced.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxDatabase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxKey;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
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
        private System.Windows.Forms.Button buttonSendFiles;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboBoxDriverName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBoxDSNName;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageBasic;
        private System.Windows.Forms.TabPage tabPageAdvanced;
        private System.Windows.Forms.Label label12;
    }
}


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
            this.SuspendLayout();
            // 
            // comboBoxDatabase
            // 
            this.comboBoxDatabase.FormattingEnabled = true;
            this.comboBoxDatabase.Items.AddRange(new object[] {
            "Oracle",
            "MsSQL"});
            this.comboBoxDatabase.Location = new System.Drawing.Point(98, 14);
            this.comboBoxDatabase.Name = "comboBoxDatabase";
            this.comboBoxDatabase.Size = new System.Drawing.Size(234, 21);
            this.comboBoxDatabase.TabIndex = 0;
            this.comboBoxDatabase.SelectedIndexChanged += new System.EventHandler(this.comboBoxDatabase_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Database type";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(350, 197);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(111, 32);
            this.buttonRun.TabIndex = 2;
            this.buttonRun.Text = "Extract to file";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Key";
            // 
            // textBoxKey
            // 
            this.textBoxKey.Location = new System.Drawing.Point(98, 178);
            this.textBoxKey.Name = "textBoxKey";
            this.textBoxKey.Size = new System.Drawing.Size(234, 20);
            this.textBoxKey.TabIndex = 6;
            this.textBoxKey.Text = "12345678-1234-1234-1234-123456789012";
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(98, 204);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(234, 20);
            this.textBoxUserName.TabIndex = 7;
            this.textBoxUserName.Text = "Your set name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 207);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Server name";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Authentication";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(41, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "User name";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(41, 127);
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
            "SQL Server Authentication",
            "Windows Authentication "});
            this.comboBoxAuthType.Location = new System.Drawing.Point(98, 69);
            this.comboBoxAuthType.Name = "comboBoxAuthType";
            this.comboBoxAuthType.Size = new System.Drawing.Size(234, 21);
            this.comboBoxAuthType.TabIndex = 2;
            this.comboBoxAuthType.SelectedIndexChanged += new System.EventHandler(this.comboBoxAuthType_SelectedIndexChanged);
            // 
            // textBoxLoginPassword
            // 
            this.textBoxLoginPassword.Location = new System.Drawing.Point(123, 123);
            this.textBoxLoginPassword.Name = "textBoxLoginPassword";
            this.textBoxLoginPassword.PasswordChar = '*';
            this.textBoxLoginPassword.Size = new System.Drawing.Size(209, 20);
            this.textBoxLoginPassword.TabIndex = 4;
            // 
            // textBoxServerName
            // 
            this.textBoxServerName.Location = new System.Drawing.Point(98, 42);
            this.textBoxServerName.Name = "textBoxServerName";
            this.textBoxServerName.Size = new System.Drawing.Size(234, 20);
            this.textBoxServerName.TabIndex = 1;
            // 
            // textBoxLoginName
            // 
            this.textBoxLoginName.Location = new System.Drawing.Point(123, 95);
            this.textBoxLoginName.Name = "textBoxLoginName";
            this.textBoxLoginName.Size = new System.Drawing.Size(209, 20);
            this.textBoxLoginName.TabIndex = 3;
            // 
            // buttonTestConnection
            // 
            this.buttonTestConnection.Location = new System.Drawing.Point(350, 142);
            this.buttonTestConnection.Name = "buttonTestConnection";
            this.buttonTestConnection.Size = new System.Drawing.Size(111, 32);
            this.buttonTestConnection.TabIndex = 2;
            this.buttonTestConnection.Text = "Test Connection";
            this.buttonTestConnection.UseVisualStyleBackColor = true;
            this.buttonTestConnection.Click += new System.EventHandler(this.buttonTestConnection_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 153);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Database";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxDatabaseName
            // 
            this.textBoxDatabaseName.Location = new System.Drawing.Point(98, 150);
            this.textBoxDatabaseName.Name = "textBoxDatabaseName";
            this.textBoxDatabaseName.Size = new System.Drawing.Size(234, 20);
            this.textBoxDatabaseName.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 237);
            this.Controls.Add(this.textBoxDatabaseName);
            this.Controls.Add(this.textBoxLoginName);
            this.Controls.Add(this.textBoxServerName);
            this.Controls.Add(this.textBoxLoginPassword);
            this.Controls.Add(this.comboBoxAuthType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxUserName);
            this.Controls.Add(this.textBoxKey);
            this.Controls.Add(this.buttonTestConnection);
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxDatabase);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "SQLdep";
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}


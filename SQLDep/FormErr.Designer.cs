namespace SQLDep
{
    partial class FormErr
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
            this.buttonViewDetails = new System.Windows.Forms.Button();
            this.buttonSendReport = new System.Windows.Forms.Button();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.labelErr = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonViewDetails
            // 
            this.buttonViewDetails.Location = new System.Drawing.Point(40, 54);
            this.buttonViewDetails.Name = "buttonViewDetails";
            this.buttonViewDetails.Size = new System.Drawing.Size(127, 33);
            this.buttonViewDetails.TabIndex = 0;
            this.buttonViewDetails.Text = "view details";
            this.buttonViewDetails.UseVisualStyleBackColor = true;
            this.buttonViewDetails.Click += new System.EventHandler(this.buttonViewDetails_Click);
            // 
            // buttonSendReport
            // 
            this.buttonSendReport.Location = new System.Drawing.Point(173, 54);
            this.buttonSendReport.Name = "buttonSendReport";
            this.buttonSendReport.Size = new System.Drawing.Size(127, 33);
            this.buttonSendReport.TabIndex = 0;
            this.buttonSendReport.Text = "send report";
            this.buttonSendReport.UseVisualStyleBackColor = true;
            // 
            // buttonQuit
            // 
            this.buttonQuit.Location = new System.Drawing.Point(310, 54);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(127, 33);
            this.buttonQuit.TabIndex = 0;
            this.buttonQuit.Text = "quit";
            this.buttonQuit.UseVisualStyleBackColor = true;
            this.buttonQuit.Click += new System.EventHandler(this.buttonQuit_Click);
            // 
            // labelErr
            // 
            this.labelErr.AutoSize = true;
            this.labelErr.Location = new System.Drawing.Point(12, 9);
            this.labelErr.Name = "labelErr";
            this.labelErr.Size = new System.Drawing.Size(42, 13);
            this.labelErr.TabIndex = 1;
            this.labelErr.Text = "labelErr";
            // 
            // FormErr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 106);
            this.Controls.Add(this.labelErr);
            this.Controls.Add(this.buttonQuit);
            this.Controls.Add(this.buttonSendReport);
            this.Controls.Add(this.buttonViewDetails);
            this.Name = "FormErr";
            this.Text = "FormErr";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonViewDetails;
        private System.Windows.Forms.Button buttonSendReport;
        private System.Windows.Forms.Button buttonQuit;
        private System.Windows.Forms.Label labelErr;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSMSAddin
{
    public partial class AskUserForm : Form
    {
        public string UserId { get;  set;}

        public bool StoreAble { get; set; }

        public AskUserForm(string userAccountId = null)
        {
            InitializeComponent();
            this.textBoxUserId.Text = userAccountId != null ? userAccountId : string.Empty;
            this.checkBoxRemember.Checked = true;
            this.Text = "Enter your credentials";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.UserId = this.textBoxUserId.Text;
            this.StoreAble = this.checkBoxRemember.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

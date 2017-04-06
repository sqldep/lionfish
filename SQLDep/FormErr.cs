using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLDep
{
    public partial class FormErr : Form
    {
        public FormErr(Exception ex)
        {
            this.Error = ex;
            InitializeComponent();
            this.labelErr.Text = ex.Message;
        }

        public Exception Error { get; internal set; }

        private void buttonViewDetails_Click(object sender, EventArgs e)
        {
            string msg = this.Error.Message + this.Error.StackTrace;
            MessageBox.Show(msg);
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SQLDep
{
    public class AsyncExecutor
    {
        public AsyncExecutor(string myName, Guid myKey, string sqlDialect, string exportFileName, Executor executor)
        {
            this.MyName = myName;
            this.MyKey = myKey;
            this.SqlDialect = sqlDialect;
            this.MyExecutor = executor;
            this.ExportFileName = exportFileName;
        }

        private string MyName { get; set; }
        private Guid MyKey { get; set; }

        private string SqlDialect { get; set; }

        private string ExportFileName { get; set; }

        public Executor MyExecutor { get; set; }

        public bool IsRunning { get; set; }

        public void Run()
        {
            this.IsRunning = true;
            try
            {
                this.MyExecutor.Run(this.MyName, this.MyKey, this.SqlDialect, this.ExportFileName);
                DialogResult answer = MessageBox.Show("Send data to SQLdep?", "Please confirm data sending.", MessageBoxButtons.YesNo);

                if (answer == DialogResult.Yes)
                {

                    this.MyExecutor.SendStructure();
                    MessageBox.Show("Completed succesfully. Check your dashboard at sqldep.com");
                }
                else
                {
                    MessageBox.Show("Completed succesfully. Data are saved on disk! " + this.ExportFileName);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                MessageBox.Show(msg);
            }
            this.IsRunning = false;
        }
    }
}

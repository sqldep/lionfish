using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SQLDepLib
{
    public class AsyncExecutor
    {
        public AsyncExecutor(string myName, Guid myKey, string sqlDialect, string exportFileName, Executor executor, bool useFs)
        {
            this.MyName = myName;
            this.MyKey = myKey;
            this.SqlDialect = sqlDialect;
            this.MyExecutor = executor;
            this.ExportFileName = exportFileName;
            this.UseFs = useFs;
        }

        private string MyName { get; set; }
        private Guid MyKey { get; set; }

        private string SqlDialect { get; set; }

        private string ExportFileName { get; set; }

        public Executor MyExecutor { get; set; }

        public bool IsRunning { get; set; }
        public bool UseFs { get; set; }

        public void Run()
        {
            this.IsRunning = true;
            try
            {
                this.MyExecutor.Run(this.MyName, this.MyKey, this.SqlDialect, this.ExportFileName, this.UseFs);
                MessageBox.Show("Completed succesfully. Data are saved on disk! " + this.ExportFileName);

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

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
        public AsyncExecutor(string myName, Guid myKey, string sqlDialect, string exportFileName, Executor executor, bool useFs, bool send)
        {
            this.MyName = myName;
            this.MyKey = myKey;
            this.SqlDialect = sqlDialect;
            this.MyExecutor = executor;
            this.ExportFileName = exportFileName;
            this.UseFs = useFs;
            this.Send = send;
        }

        private string MyName { get; set; }
        private Guid MyKey { get; set; }

        private string SqlDialect { get; set; }

        private string ExportFileName { get; set; }

        public Executor MyExecutor { get; set; }

        public bool IsRunning { get; set; }
        public bool UseFs { get; set; }
        public bool Send { get; set; }

        public void Run()
        {
            this.IsRunning = true;
            try
            {
                this.MyExecutor.Run(this.MyName, this.MyKey, this.SqlDialect, this.ExportFileName, this.UseFs);
                if (Send)
                {
                    List<string> files = new List<string>();
                    files.Add(ExportFileName);
                    try
                    {
                        MyExecutor.SendFiles(files, MyKey.ToString());
                        MessageBox.Show("File sent successfully. (" + ExportFileName + ")");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("File was not sent successfully: " + e.Message);
                    }

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

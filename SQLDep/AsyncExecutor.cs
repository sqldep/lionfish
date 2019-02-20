using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SQLDep;

namespace SQLDepLib
{
    public class AsyncExecutor
    {
        public AsyncExecutor(Arguments args, Executor executor, bool sendFile, Form1 form)
        {
            this.Args = args;
            this.MyExecutor = executor;
            this.Send = sendFile;
            this.Form = form;
        }
        private Arguments Args { get; set; }
        public Executor MyExecutor { get; set; }

        public bool IsRunning { get; set; }
        private bool Send { get; set; }
        private Form1 Form { get; set; }

        public void Run()
        {
            this.IsRunning = true;
            try
            {
                this.MyExecutor.Run(Args);
                if (Send)
                {
                    try
                    {
                        MyExecutor.SendFile(Args.exportFileName, Args.myKey.ToString());
                        MessageBox.Show("File sent successfully. (" + Args.exportFileName + ")");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("File was not sent successfully: " + e.Message);
                    }

                }
                else
                {
                    MessageBox.Show("Completed succesfully. Data are saved on disk! " + Args.exportFileName);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Logger.Log(msg);
                MessageBox.Show(msg);
            }
            this.IsRunning = false;

            Form.AsyncExecutor = null;
            Form.AsyncExecutorThread = null;
        }
    }
}

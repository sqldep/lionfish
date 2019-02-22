using System;
using System.IO;

namespace SQLDepLib
{
    public static class Logger
    {
        public static void Log(string msg, string filename = "SQLdepLog.txt")
        {
            {
                msg = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\t" + msg ;
                Console.WriteLine(msg);
                StreamWriter wr = File.AppendText(filename);
                wr.WriteLine(msg);
                wr.Close();
            }
        }

        public static void Exception(string msg, string filename = "SQLdepLog.txt")
        {
            {
                msg = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\t" + "Exception: " + msg;
                Console.WriteLine(msg);
                StreamWriter wr = File.AppendText(filename);
                wr.WriteLine(msg);
                wr.Close();
            }
        }
    }
}

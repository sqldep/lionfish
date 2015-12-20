using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDepLib
{
    class PercInfo
    {
        public PercInfo ()
        {
            this.Ratio = 0;
            this.Done = 0;
            this.WorkingOn = string.Empty;

        }
        public double Ratio { get; set; }

        public double Done { get; set; }

        public string WorkingOn { get; set; }
    }
    public class ProgressInfo
    {
        private double lastProgress = 0;
        private LinkedList<PercInfo> PercInfos { get; set; }

        public ProgressInfo()
        {
            this.PercInfos = new LinkedList<PercInfo>();
        }

        public double GetPercentDone (out string workingOn)
        {
            double done = 0;
            lock (this)
            {
                double ratio = 1;
                string retWorkingOn = string.Empty;
                foreach (var item in this.PercInfos)
                {
                    done += ratio * item.Done;
                    ratio = item.Ratio;
                    if (item.WorkingOn.Length > 0)
                    {
                        if (retWorkingOn.Length > 0)
                        {
                            retWorkingOn += " ";
                        }
                        retWorkingOn += item;
                    }
                }

                if (lastProgress > done)
                {
                    // bad implementation
                    int ii = 0;
                }

                this.lastProgress = done;
                if (retWorkingOn.Length > 0)
                {
                    retWorkingOn += " ";
                }
                retWorkingOn += string.Format("{0:0.00} %", done);
                workingOn = retWorkingOn;
            }
            return done;
        }

        public void CreateProgress ()
        {
            lock(this)
            {
                this.PercInfos.AddLast(new PercInfo());
            }
        }

        public void RemoveProgress ()
        {
            lock (this)
            {
                PercInfo lastRemoved = this.PercInfos.Last();
                this.PercInfos.RemoveLast();

                PercInfo last = this.PercInfos.Last();
                if (last != null)
                {
                    last.Done += last.Ratio * lastRemoved.Done;
                }
            }
        }

        // sum ratios in one function should be 1
        public void SetProgressRatio(double ratio, string workingOn)
        {
            lock (this)
            {

                PercInfo last = this.PercInfos.Last();
                if (last != null)
                {
                    last.Ratio = ratio;
                    last.WorkingOn = workingOn;
                }
            }
        }

        // set progress from 0-100
        public void SetProgressDone(double done, string workingOn)
        {
            lock(this)
            {
                PercInfo last = this.PercInfos.Last();
                if (last != null)
                {
                    last.Done = done;
                    last.WorkingOn = workingOn;
                }
            }
        }
    }
}

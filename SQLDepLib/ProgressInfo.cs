using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDepLib
{

    public class ProgressInfo
    {

        public double Done { get; set; }
        public string WorkingOn { get; set; }

        public ProgressInfo()
        {
            Done = 0f;
            WorkingOn = String.Empty;
        }

        public double GetPercentDone (out string workingOn)
        {
            workingOn = WorkingOn;
            workingOn += string.Format(": {0:0.00} %", Done);
            
            return Done;
        }

        // sum ratios in one function should be 1
        public void SetProgressPercent(double percent, string workingOn)
        {
            Done = percent;
            WorkingOn = workingOn;
        }

        public void RemoveProgress()
        {
            Done = 0f;
            WorkingOn = String.Empty;
        }
    }
}

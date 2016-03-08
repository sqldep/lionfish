using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLDepLib;

namespace SQLDep
{
    public class ComboBoxDriverItem
    {
        public string Text { get; set; }
        public DBExecutor.UseDriver UseDriverType { get; set; }
    }
}

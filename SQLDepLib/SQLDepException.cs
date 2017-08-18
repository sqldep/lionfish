using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDepLib
{
    public class SQLDepException : Exception
    {
        public SQLDepException(string message) : base(message) { }
    }
}

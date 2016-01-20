using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDepLib
{
    public class ExecutorFactory
    {
        public static Executor CreateExecutor(DBExecutor dbExecutor, string sqlDialect)
        {
            if (sqlDialect == "teradata")
            {
                return new TeradataExecutor(dbExecutor);
            }
            else
            {
                return new Executor(dbExecutor);
            }
        }
    }
}

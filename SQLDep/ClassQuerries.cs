using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDep
{
    public class SQLCompleteStructure
    {
        public List<SQLQuerry> queries { get; set; }

        public List<SQLDBLink> dblinks { get; set; }

        public SQLDatabaseModel databaseModel { get; set; }
        public string userAccountId { get; set; }

        public string dialect { get; set; }

        public string customSqlSetName { get; set; }
        public string createdBy { get; set; }
        public string exportTime { get; set; }
        public string exportId { get; set; }
        public string instanceName { get; set; }

    }
}

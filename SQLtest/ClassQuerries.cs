using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLtest
{
    public class ClassCompleteStructure
    {
        public List<SQLQuerry> queries { get; set; }

        public SQLDatabaseModel databaseModel { get; set; }
        public string userAccountId { get; set; }

        public string dialect { get; set; }

        public string customSqlSetName { get; set; }
    }
}

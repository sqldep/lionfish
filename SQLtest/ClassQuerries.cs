using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLtest
{
    public class ClassQuerries
    {
        public List<SQLData> queries { get; set; }
        public string userAccountId { get; set; }

        public string dialect { get; set; }

        public string customSqlSetName { get; set; }
    }
}

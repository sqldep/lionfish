using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLtest
{
    public class SQLResult
    {
        public string Column0 { get; set; }
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
    }


    public class SQLQuerry
    {
        public string schema { get; set; }
        public string database { get; set; }
        public string groupName { get; set; }
        public string sourceCode { get; set; }
        public string name { get; set; }
    }

    public class StrReplace
    {
        public string SearchText { get; set; }
        public string ReplaceText { get; set; }
    }

    public class SQLDatabaseModel
    {
        public List<SQLDatabaseModelItem> databases { get; set; }
    }

    public class SQLDatabaseModelItem
    {
        public string name { get; set; }

        public List<SQLTableModelItem> tables { get; set; }
        public List<SQLSynonymModelItem> synonyms { get; set; }
    }

    public class SQLTableModelItem
    {
        public string schema { get; set; }
        public string name { get; set; }
        public bool isView { get; set; }

        public List<SQLColumnModelItem> columns { get; set; }


    }

    public class SQLColumnModelItem
    {
        public string name { get; set; }
        public bool comment { get; set; }
        public string dataType { get; set; }
    }

    public class SQLSynonymModelItem
    {
        public string schema { get; set; }
        public string name { get; set; }
        public string sourceName { get; set; }
        public string sourceSchema { get; set; }
        public string sourceDbLinkName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDepLib
{
    public class TeradataExecutor : Executor
    {
        public TeradataExecutor(DBExecutor dbExecutor) : base(dbExecutor)
        {

        }



        public enum ItemType
            {
            TABLE,
            VIEW,
            PROCEDURE,
            MACRO
        }

        public SQLQuerry GetQuerry (string dbName, string name, ItemType type)
        {
            string sql = string.Empty;
            name = name.Trim();
            switch (type)
            {
                case ItemType.PROCEDURE:
                    sql = string.Format("SHOW PROCEDURE {0}.{1}", dbName, name);
                    break;
                case ItemType.TABLE:
                    sql = string.Format("SHOW TABLE {0}.{1}", dbName, name);
                    break;
                case ItemType.VIEW:
                    sql = string.Format("SHOW VIEW {0}.{1}", dbName, name);
                    break;
                case ItemType.MACRO:
                    sql = string.Format("SHOW MACRO {0}.{1}", dbName, name);
                    break;
            }

            SQLQuerry querryItem = new SQLQuerry()
            {
                sourceCode = string.Empty,
                name = name,
                groupName = string.Empty,
                database = "default",
                schema = dbName  // workaround for Teradata parser
            };

            List<SQLResult> result = new List<SQLResult>();
            DBExecutor.RunSql(result, sql);
            string sourceCodeLine = string.Empty;
            foreach (var item in result)
            {
                // teradata ma zvlastni oddelovac radku
                sourceCodeLine = item.Column0.Replace('\r', '\n');
                querryItem.sourceCode += sourceCodeLine;
            }

            return querryItem;
        }

        public override SQLCompleteStructure Run(string sqlDialect, bool useFS)
        {
            // The following SELECTS map to JSON (see example.json)
            SQLCompleteStructure ret = new SQLCompleteStructure();
            Logger.Log("Getting list of databases");
            List<string> dbNames = this.GetTeradataDbNames(sqlDialect);
            Logger.Log("List of databases has " + dbNames.Count + " items.");

            Logger.Log("Getting list of querries");
            if (!useFS)
            {
                ret.queries = this.GetTeradataQuerries(sqlDialect, dbNames);
                Logger.Log("List of querries has " + ret.queries.Count + " items.");
            }
            else
            {
                ret.queries = new List<SQLQuerry>();
            }

            ret.databaseModel = new SQLDatabaseModel();
            ret.databaseModel.databases = this.GetTeradataDatabaseModels(sqlDialect, dbNames);

            return ret;
        }

        private List<SQLQuerry> GetTeradataQuerries(string sqlDialect, List<string> dbNames)
        {
            List<SQLQuerry> ret = new List<SQLQuerry>();

            bool firstSqlCommands = true;
            foreach (var dbName in dbNames)
            {
                // this.ProgressInfo.SetProgressDone((double)100 * ++iiDbCounter / dbNames.Count, dbName);
                {
                    // let us run this script two times, first run returns list of procedures, the second loads its definition
                    List<string> procedures = new List<string>();
                    // sql commands with replace
                    List<StrReplace> replaces = new List<StrReplace>();
                    StrReplace itemForReplace = new StrReplace()
                    {
                        SearchText = "##DBNAME##",
                        ReplaceText = dbName
                    };
                    replaces.Add(itemForReplace);

                    // load script with replaces for the given database/procedure
                    List<string> sqls = this.GetSQLCommands(sqlDialect, Purpose.TABLES, firstSqlCommands, replaces);
                    firstSqlCommands = false;

                    // first command is list of procedures and views
                    List<SQLResult> result = new List<SQLResult>();
                    DBExecutor.RunSql(result, sqls.ElementAt(2));

                    foreach (var item in result)
                    {
                        string procedureOrViewName = item.Column2;

                        ItemType itemType = ItemType.PROCEDURE;

                        switch (item.Column3.Trim())
                        {
                            case "T":
                                itemType = ItemType.TABLE;
                                break;
                            case "V":
                                itemType = ItemType.VIEW;
                                break;
                            case "P":
                                itemType = ItemType.PROCEDURE;
                                break;
                            case "M":
                                itemType = ItemType.MACRO;
                                break;
                            default: throw new Exception("NYI");
                        }

                        try
                        {
            
                            SQLQuerry querryItem = this.GetQuerry(dbName, procedureOrViewName, itemType);
                            ret.Add(querryItem);
                        }
                        catch(Exception ex)
                        {
                            Logger.Log("Ignored error " + ex.Message);
                        }
                    }
                }
            }
            return ret;
        }

        private List<string> GetTeradataDbNames(string sqlDialect)
        {
            List<string> sqls = this.GetSQLCommands(sqlDialect, Purpose.DATABASES, true, null);

            List<SQLResult> result = new List<SQLResult>();

            foreach (var item in sqls)
            {
                DBExecutor.RunSql(result, item);
            }

            List<string> ret = new List<string>();
            foreach (var item in result)
            {
                // trimm outer spaces
                string dbName = item.Column0.Trim();
                ret.Add(dbName);
            }

            return ret;
        }

        private List<SQLDatabaseModelItem> GetTeradataDatabaseModels(string sqlDialect, List<string> dbNames)
        {
            List<SQLDatabaseModelItem> modelItems = new List<SQLDatabaseModelItem>();

            SQLDatabaseModelItem modelItem = new SQLDatabaseModelItem();
            modelItem.name = "default";
            modelItem.tables = new List<SQLTableModelItem>();
            bool firstSqlCommands = true;
            foreach (var dbName in dbNames)
            {

                // this.ProgressInfo.SetProgressDone((double)100 * ++iiCounter / dbNames.Count, dbName);

                List<StrReplace> replaces = new List<StrReplace>();
                StrReplace itemForReplace = new StrReplace()
                {
                    SearchText = "##DBNAME##",
                    ReplaceText = dbName
                };
                replaces.Add(itemForReplace);
                List<string> sqls = this.GetSQLCommands(sqlDialect, Purpose.TABLES, firstSqlCommands, replaces);
                firstSqlCommands = false;

                // tabulky a viecka
                List<SQLResult> tablesAndViews = new List<SQLResult>();
                DBExecutor.RunSql(tablesAndViews, sqls.ElementAt(0));

                // sloupecky
                List<SQLResult> allColumns = new List<SQLResult>();
                DBExecutor.RunSql(allColumns, sqls.ElementAt(1));

                foreach (var tableOrView in tablesAndViews)
                {
                    string tableOrViewName = tableOrView.Column3.Trim();
                    List<SQLResult> columns = allColumns.Where(x => x.Column1.Trim() == tableOrViewName).ToList();

                    try
                    {
                        bool isView = false;
                        /*  
                        
                        zatim neukladame create tabulek

                        SQLQuerry structure = null;
                        if (tableOrView.Column1 == "T")
                        {
                            // table
                            structure = this.GetQuerry(dbName, tableOrViewName, ItemType.TABLE);
                        }
                        else if (tableOrView.Column1 == "V")
                        {
                            structure = this.GetQuerry(dbName, tableOrViewName, ItemType.VIEW);
                            isView = true;
                        }
                        else
                        {
                            throw new Exception("NYI");
                        }
                        */

                        SQLTableModelItem tableModelItem = new SQLTableModelItem()
                        {
                            database = string.Empty,
                            schema = dbName,  // because Teradata parser treats DB as schema
                            name = tableOrViewName,
                            isView = (isView) ? "true" : "false",
                            columns = new List<SQLColumnModelItem>()
                        };

                        modelItem.tables.Add(tableModelItem);

                        // columns
                        foreach (var column in columns)
                        {
                            SQLColumnModelItem columnModelItem = new SQLColumnModelItem()
                            {
                                name = column.Column2,
                                dataType = column.Column3,
                                comment = "" // item.Column6
                            };
                            tableModelItem.columns.Add(columnModelItem);
                        }
                    }
                    catch(Exception ex)
                    {
                        Logger.Log("Ignored error for table/view " + tableOrViewName + ", error:" + ex.Message);// ignore
                    }
                }

                Logger.Log("Tables #[" + modelItem.tables.Count + "] in database" + dbName + " processed.");
            }
            modelItems.Add(modelItem);

            return modelItems;
        }
    }
}

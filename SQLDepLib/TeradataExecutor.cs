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
            }

            SQLQuerry querryItem = new SQLQuerry()
            {
                sourceCode = string.Empty,
                name = name,
                groupName = string.Empty,
                database = dbName,
                schema = string.Empty
            };

            List<SQLResult> result = new List<SQLResult>();
            DBExecutor.RunSql(result, sql);
            foreach (var item in result)
            {
                querryItem.sourceCode += item.Column0;
            }

            return querryItem;
        }







        /*


                      DBExecutor.RunSql(result, sqls.ElementAt(0));
                            foreach (var item in result)
                            {
                                // copy result to list to be processed
                                procedures.Add(item.Column2);
                            }
                            iiRun++;
                        }
                        else if (iiRun == 1)
                        {
                            sqls.RemoveRange(0, 1);

                            // procedure definition
                            List<SQLResult> result = new List<SQLResult>();

                            try
                            {
                                foreach (var item in sqls)
                                {
                                    // pobezi nam show prikaz, vrati to data?
                                    DBExecutor.RunSql(result, item);
                                }
                            }
                            catch (Exception)
                            {
                                // todo nektere procedury nelze cist
                                continue;
                            }

                            // TODO: zkontrolovat co to vraci a to dat do vysledku

                            // result ma 3 sloupce a je treba je spojit mezerou. Vysledny string je sourceCode v JSONu.
                            // prvni sloupec je REPLACE PROCEDURE
                            // druhy sloupec je nazev procedury/tabulky
                            // treti sloupec je zdrojak
                            SQLQuerry querryItem = new SQLQuerry()
                            {
                                sourceCode = string.Empty,
                                name = currentProcedure.Trim(),
                                groupName = string.Empty,
                                database = dbName,
                                schema = string.Empty
                            };
                            foreach (var item in result)
                            {
                                querryItem.sourceCode += item.Column0;
                            }

                            ret.Add(querryItem);
                        }

        */






        public override SQLCompleteStructure Run(string sqlDialect)
        {
            this.ProgressInfo.CreateProgress();

            // The following SELECTS map to JSON (see example.json)
            SQLCompleteStructure ret = new SQLCompleteStructure();
            this.Log("Getting list of databases");
            List<string> dbNames = this.GetTeradataDbNames(sqlDialect);
            this.Log("List of databases has " + dbNames.Count + " items.");

            this.Log("Getting list of querries");
            this.ProgressInfo.SetProgressRatio(0.45, "querries");
            ret.queries = this.GetTeradataQuerries(sqlDialect, dbNames);
            this.Log("List of querries has " + ret.queries.Count + " items.");


            this.ProgressInfo.SetProgressRatio(0.55, "DB model");
            ret.databaseModel = new SQLDatabaseModel();
            ret.databaseModel.databases = this.GetTeradataDatabaseModels(sqlDialect, dbNames);


            this.ProgressInfo.RemoveProgress();
            return ret;
        }

        private List<SQLQuerry> GetTeradataQuerries(string sqlDialect, List<string> dbNames)
        {
            List<SQLQuerry> ret = new List<SQLQuerry>();

            this.ProgressInfo.CreateProgress();
            int iiDbCounter = 0;

            foreach (var dbName in dbNames)
            {
                this.ProgressInfo.SetProgressDone((double)100 * ++iiDbCounter / dbNames.Count, dbName);
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
                    List<string> sqls = this.GetSQLCommands(sqlDialect, "tables", replaces);

                    // first command is list of procedures
                    List<SQLResult> result = new List<SQLResult>();
                    DBExecutor.RunSql(result, sqls.ElementAt(2));
                    foreach (var item in result)
                    {
                        // copy result to list to be processed
                        procedures.Add(item.Column2);
                    }

                    foreach (var item in procedures)
                    {
                        try
                        {
                            SQLQuerry querryItem = this.GetQuerry(dbName, item, ItemType.PROCEDURE);
                            ret.Add(querryItem);
                        }
                        catch(Exception ex)
                        {
                            this.Log("Ignored error " + ex.Message);// ignore
                        }
                    }
                }
            }
            this.ProgressInfo.RemoveProgress();
            return ret;
        }

        private List<string> GetTeradataDbNames(string sqlDialect)
        {
            List<string> sqls = this.GetSQLCommands(sqlDialect, "databases", null);

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
                if (dbName == "DBC")
                {
                    continue;
                }

                ret.Add(dbName);
            }

            return ret;
        }

        private List<SQLDatabaseModelItem> GetTeradataDatabaseModels(string sqlDialect, List<string> dbNames)
        {
            List<SQLDatabaseModelItem> modelItems = new List<SQLDatabaseModelItem>();
            this.ProgressInfo.CreateProgress();

            int iiCounter = 0;
            foreach (var dbName in dbNames)
            {

                this.ProgressInfo.SetProgressDone((double)100 * ++iiCounter / dbNames.Count, dbName);
                SQLDatabaseModelItem modelItem = new SQLDatabaseModelItem();
                modelItem.tables = new List<SQLTableModelItem>();

                List<StrReplace> replaces = new List<StrReplace>();
                StrReplace itemForReplace = new StrReplace()
                {
                    SearchText = "##DBNAME##",
                    ReplaceText = dbName
                };
                replaces.Add(itemForReplace);
                List<string> sqls = this.GetSQLCommands(sqlDialect, "tables", replaces);

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
                            database = dbName,
                            schema = string.Empty,
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
                        this.Log("Ignored error for table/view " + tableOrViewName + ", error:" + ex.Message);// ignore
                    }
                }
                modelItems.Add(modelItem);

                this.Log("Tables #[" + modelItem.tables.Count + "] in database" + dbName + " processed.");
            }
            return modelItems;
        }
    }
}
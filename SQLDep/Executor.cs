using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace SQLDep
{
    public class Executor
    {

        public Executor(DBExecutor dbExecutor)
        {
            this.DBExecutor = dbExecutor;
            this.runId = Guid.NewGuid().ToString();
        }

        public string runId { get; set; }

        private DBExecutor DBExecutor { get; set; }

        public string LogFileName { get; set; }

        private void Log(string msg)
        {
            StreamWriter wr = File.AppendText(LogFileName);
            wr.Write(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            wr.Write("\t");
            wr.WriteLine(msg);
            wr.Close();
        }

        private string myJson = string.Empty;

        public void Run(string customSqlSetName, Guid myKey, string sqlDialect, string exportFileName)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                this.LogFileName = "SQLdepLog.txt";
                // pripoj se do databaze
                this.Log("Before database open.");
                DBExecutor.Connect();
                this.Log("Database open.");
                SQLCompleteStructure dbStructure = this.Run(sqlDialect);

                int totalTablesCount = 0;
                foreach (var item in dbStructure.databaseModel.databases)
                {
                    totalTablesCount += item.tables.Count;
                }

                if (dbStructure.queries.Count == 0 && totalTablesCount == 0)
                {
                    throw new Exception("None data collected. Missinq any querry or table.");
                }

                dbStructure.dialect = sqlDialect;
                dbStructure.userAccountId = myKey.ToString();
                dbStructure.customSqlSetName = customSqlSetName;
                sw.Stop();
                dbStructure.exportTime = sw.ElapsedMilliseconds.ToString();
                myJson = this.SaveStructureToFile(dbStructure, exportFileName);
                DBExecutor.Close();
            }
            catch (Exception ex)
            {
                this.Log("Error " + ex.Message + "\n" + ex.StackTrace);
                throw;
            }
        }

        public void SendStructure()
        {
            try
            {
                this.SendStructure(this.myJson);
            }
            catch (Exception ex)
            {
                this.Log("Error " + ex.Message + "\n" + ex.StackTrace);
                throw;
            }
        }

        private SQLCompleteStructure Run(string sqlDialect)
        {
            // The following SELECTS map to JSON (see example.json)
            SQLCompleteStructure ret = new SQLCompleteStructure();
            this.Log("Getting list of databases");
            List<string> dbNames = this.GetDbNames(sqlDialect);
            this.Log("List of databases has " + dbNames.Count + " items.");

            // 2. SELECT
            // DDL ("queries" in JSON): procedures and views
            // - sourceCode: `select * from table1` (string, required)
            // - name (string, optional) - for visualization purposes - use your internal name if available
            // - groupName (string, optional) - for visualization purposes - use your internal name if available
            // - database (string, optional)
            // - schema (string, optional)
            //
            // Expect columns in this order: SourceCode, Name, GroupName, Database, Schema


            // 3. SELECT
            // DbDef: ("databaseModel" in JSON) details on table and view columns
            // ## Table (object)
            //
            // - schema: `ETL_SCHEMA` (string, required) - name of the schema for the table/view
            // - name: ACCOUNT (string, required) - name of the table/view, avoid using duplicate names (only the first occurrence may be processed)
            // - isView: false (boolean, optional, default) - true => view, otherwise => table
            // - columns (array[Column]) - columns are processed sequentially, provide them in the same order as in your table/view
            //
            // ## Column (object)
            //
            // - name: `ACC_ID` (string, required) - name of the column, avoid using duplicate names (only the first occurrence may be processed)
            // - dataType: `NUMBER(10)` (string, optional) - column data type
            // - comment: `Unique account identifier` (string, optional) - column comment if available
            //
            // Expect columns in this order: Database, Schema, TableName, IsView, ColumnName, DataType, Comment, ColOrder


            // 4. SELECT
            // synonyms
            // ## Synonym (object)
            //
            // - schema: `DW_SCHEMA` (string, required) - name of the synonym schema
            // - name: ACCOUNTS (string, required) - synonym name, avoid using duplicate names (only the first occurrence may be processed)
            // - sourceName: ACCOUNTS (string, required) - table/view name
            // - sourceSchema: `ETL_SCHEMA` (string, optional) - name of the schema for source table/view
            // - sourceDbLinkName (string, optional) - database link for source table/view
            //
            // Expect columns in this order: Database, Schema, Name, SoourceName, SourceSchema, SourceDbLinkName


            // 5. SELECT
            // ## DBLink (object)
            //
            // - owner : (string, required)
            // - name : (string, required)
            // - userName : (string, required)
            // - host : (string, required)
            //
            // Expect columns in this order: Owner, Name, UserName, Host

            this.Log("Getting list of querries");
            if (sqlDialect == "oracle")
            {
                this.Log("Using Oracle dialect");
                ret.queries = this.GetOracleQuerries(sqlDialect, dbNames);
            }
            else
            {
                ret.queries = this.GetQuerries(sqlDialect, dbNames);
            }
            this.Log("List of querries has " + ret.queries.Count + " items.");

            ret.databaseModel = new SQLDatabaseModel();
            ret.databaseModel.databases = this.GetDatabaseModels(sqlDialect, dbNames);

            this.Log("Getting list of dblinks");
            ret.dblinks = this.GetDBLinks(sqlDialect);
            this.Log("List of dblinks has " + ret.dblinks.Count + " items.");
            return ret;
        }

        private List<string> GetDbNames(string sqlDialect)
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

                ret.Add(item.Column0);
            }

            return ret;
        }

        private List<SQLQuerry> GetOracleQuerries(string sqlDialect, List<string> dbNames)
        {
            List<SQLQuerry> ret = new List<SQLQuerry>();

            foreach (var dbName in dbNames)
            {
                try
                {
                    // sql commands
                    List<StrReplace> replaces = new List<StrReplace>();
                    StrReplace itemForReplace = new StrReplace()
                    {
                        SearchText = "##DBNAME##",
                        ReplaceText = dbName
                    };
                    replaces.Add(itemForReplace);

                    List<string> sqls = this.GetSQLCommands(sqlDialect, "queries", replaces);

                    // vem prvni select, procedury spojime dle cisla radku

                    List<SQLResult> firstBlock = new List<SQLResult>();
                    DBExecutor.RunSql(firstBlock, sqls.FirstOrDefault());

                    StringBuilder wholeCode = new StringBuilder(512 * 1024);
                    string queryName = string.Empty;
                    string querySchema = string.Empty;
                    string queryDatabase = string.Empty;
                    string queryGroup = string.Empty;
                    int counter = 0;
                    int query_counter = 0;

                    foreach (var item in firstBlock)
                    {
                        if (item.Column5.Equals("1")) { // dump previous wholeCode

                            if (counter > 0)
                            {
                                SQLQuerry querryItem = new SQLQuerry()
                                {
                                    sourceCode = wholeCode.ToString(),
                                    name = queryName,
                                    groupName = queryGroup,
                                    database = queryDatabase,
                                    schema = querySchema
                                };

                                ret.Add(querryItem);
                                this.Log("Query done " + query_counter);
                                query_counter++;
                            }

                            wholeCode.Length = 0;
                            wholeCode.Append(item.Column0);
                            queryName = item.Column1;
                            queryGroup = item.Column2;
                            querySchema = item.Column4;
                            queryDatabase = item.Column3;
                        }
                        else
                        {
                            wholeCode.Append(item.Column0);
                            queryName = item.Column1;
                            querySchema = item.Column4;
                            queryDatabase = item.Column3;
                            queryGroup = item.Column2;
                        }
                        counter++;
                    }

                        

                    sqls.RemoveAt(0);
                    List<SQLResult> secondBlock = new List<SQLResult>();
                    DBExecutor.RunQuerySql(secondBlock, sqls.FirstOrDefault());

                    foreach (var item in secondBlock)
                    {
                        SQLQuerry querryItem = new SQLQuerry()
                        {
                            sourceCode = "CREATE OR REPLACE FORCE VIEW " + item.Column2 + " AS " +  item.Column0,
                            name = item.Column1,
                            groupName = item.Column2,
                            database = item.Column3,
                            schema = item.Column4
                        };

                        ret.Add(querryItem);
                    }

                    sqls.RemoveAt(0);
                    List<SQLResult> thirdBlock = new List<SQLResult>();
                    DBExecutor.RunQuerySql(thirdBlock, sqls.FirstOrDefault());

                    foreach (var item in thirdBlock)
                    {
                        SQLQuerry querryItem = new SQLQuerry()
                        {
                            sourceCode = "CREATE MATERIALIZED VIEW " + item.Column2 + " AS " + item.Column0,
                            name = item.Column1,
                            groupName = item.Column2,
                            database = item.Column3,
                            schema = item.Column4
                        };

                        ret.Add(querryItem);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return ret;
        }

        private List<SQLQuerry> GetQuerries(string sqlDialect, List<string> dbNames)
        {
            List<SQLQuerry> ret = new List<SQLQuerry>();

            int count = 0;
            foreach (var dbName in dbNames)
            {
                try
                {
                    // sql commands
                    List<StrReplace> replaces = new List<StrReplace>();
                    StrReplace itemForReplace = new StrReplace()
                    {
                        SearchText = "##DBNAME##",
                        ReplaceText = dbName
                    };
                    replaces.Add(itemForReplace);

                    List<string> sqls = this.GetSQLCommands(sqlDialect, "queries", replaces);

                    List<SQLResult> result = new List<SQLResult>();
                    foreach (var item in sqls)
                    {
                        DBExecutor.RunSql(result, item);
                    }

                    foreach (var item in result)
                    {
                        SQLQuerry querryItem = new SQLQuerry()
                        {
                            sourceCode = item.Column0,
                            name = item.Column1,
                            groupName = item.Column2,
                            database = item.Column3,
                            schema = item.Column4
                        };


                        ret.Add(querryItem);
                        count++;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return ret;
        }

        private List<SQLDBLink> GetDBLinks (string sqlDialect)
        {
            List<SQLDBLink> ret = new List<SQLDBLink>();

            int count = 0;
            try
            {
                // sql commands

                List<string> sqls = this.GetSQLCommands(sqlDialect, "dblinks", null);

                List<SQLResult> result = new List<SQLResult>();
                foreach (var item in sqls)
                {
                    DBExecutor.RunSql(result, item);
                }

                foreach (var item in result)
                {
                    SQLDBLink dblinkItem = new SQLDBLink()
                    {
                        owner = item.Column0,
                        db_link = item.Column1,
                        username = item.Column2,
                        host = item.Column3,
                    };

                    ret.Add(dblinkItem);
                    count++;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return ret;
        }

        private List<string> GetSQLCommands(string sqlDialect, string purpose, List<StrReplace> list)
        {

            string sqlCommands = System.IO.File.ReadAllText("./sql/" + sqlDialect + "/" + purpose + "/cmd.sql");

            if (list != null)
            {
                foreach (var item in list)
                {
                    sqlCommands = sqlCommands.Replace(item.SearchText, item.ReplaceText);
                }
            }

            string[] separator = { "--split" };
            List<string> sqlCommandsList = sqlCommands.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();

            return sqlCommandsList;
        }

        private string SaveStructureToFile(SQLCompleteStructure querries, string logJSONName)
        {
            querries.createdBy = "SQLdep v0.9";
            querries.exportId = this.runId;
            querries.instanceName = this.DBExecutor.Hostname;

            var jsonSerialiser = new JavaScriptSerializer();
            jsonSerialiser.MaxJsonLength = Int32.MaxValue;
            var json = jsonSerialiser.Serialize(querries);

            // post data
            //string URI = " https://dev-jessie.sqldep.com/api/rest/sqlset/create/";
            //string myParameters = json;
            //using (WebClient wc = new WebClient())
            //{
            //    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
            //    string HtmlResult = wc.UploadString(URI, myParameters);
            //}

            StreamWriter wr = File.AppendText(logJSONName);
            wr.Write(json);
            wr.Close();

            this.Log("Result data saved in " + logJSONName);
            return json;
        }
    

        private void SendStructure(string json)
        {

            // post data
            //string URI = " https://dev-jessie.sqldep.com/api/rest/sqlset/create/";
            //string myParameters = json;
            //using (WebClient wc = new WebClient())
            //{
            //    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
            //    string HtmlResult = wc.UploadString(URI, myParameters);
            //}

            this.Log("Before sending data.");
            var baseAddress = "https://sqldep.com/api/rest/sqlset/create/";

            var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));

            string proxy = WebRequest.DefaultWebProxy.GetProxy(http.Address).AbsoluteUri;
            if (proxy != string.Empty)
            {
                this.Log("Proxy URL: " + proxy);
                http.Proxy = WebRequest.DefaultWebProxy;
            }

            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";
            string parsedContent = json;
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            var response = http.GetResponse();
            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();

            if (content.IndexOf("success") < 0)
            {
                throw new Exception("Unexpected returned message " + content);
            }
            this.Log("Data sent.");
        }

 

        private List<SQLDatabaseModelItem> GetDatabaseModels(string sqlDialect, List<string> dbNames)
        {
            List<SQLDatabaseModelItem> ret = new List<SQLDatabaseModelItem>();

            int tableCount = 0;
            int synonymsCount = 0;

            foreach (var dbName in dbNames)
            {
                try
                {
                    SQLDatabaseModelItem modelItem = new SQLDatabaseModelItem();
                    modelItem.name = dbName;

                    this.Log("Getting tables in database" + dbName + ".");

                    // sql commands
                    List<StrReplace> replaces = new List<StrReplace>();
                    StrReplace itemForReplace = new StrReplace()
                    {
                        SearchText = "##DBNAME##",
                        ReplaceText = dbName
                    };
                    replaces.Add(itemForReplace);

                    // tabulky a viecka se sloupci dohromady
                    modelItem.tables = new List<SQLTableModelItem>();
                    List<string> sqlsTablesWithColumns = this.GetSQLCommands(sqlDialect, "tables", replaces);
                    List<SQLResult> tablesWithColumns = new List<SQLResult>();
                    foreach (var item in sqlsTablesWithColumns)
                    {
                        DBExecutor.RunSql(tablesWithColumns, item);
                    }

                    foreach (var item in tablesWithColumns)
                    {
                        string tableName = item.Column2;

                        SQLTableModelItem tableModelItem = modelItem.tables.Find(x => x.name == tableName);

                        if (tableModelItem == null)
                        {
                            tableModelItem = new SQLTableModelItem()
                            {
                                database = item.Column0,
                                schema = item.Column1,
                                name = item.Column2,
                                isView =  item.Column3,
                                columns = new List<SQLColumnModelItem>()
                            };
                            modelItem.tables.Add(tableModelItem);
                            tableCount++;
                        }

                        SQLColumnModelItem columnModelItem = new SQLColumnModelItem()
                        {
                            name = item.Column4,
                            dataType = item.Column5,
                            comment = "" // item.Column6
                        };
                        tableModelItem.columns.Add(columnModelItem);
                    }
                    this.Log("Tables #["+ modelItem.tables.Count + "] in database" + dbName + " processed.");


                    // synonyms
                    this.Log("Getting synonyms in database" + dbName + ".");

                    modelItem.synonyms = new List<SQLSynonymModelItem>();
                    List<string> sqlsSynonyms = this.GetSQLCommands(sqlDialect, "synonyms", replaces);
                    List<SQLResult> synonyms = new List<SQLResult>();
                    foreach (var item in sqlsSynonyms)
                    {
                        DBExecutor.RunSql(synonyms, item);
                    }
                    foreach (var item in synonyms)
                    {
                        SQLSynonymModelItem synonymModelItem = new SQLSynonymModelItem()
                        {
                            database = item.Column0,
                            schema = item.Column1,
                            name = item.Column2,
                            sourceName = item.Column4,
                            sourceSchema = item.Column3,
                            sourceDbLinkName = item.Column5
                        };
                        modelItem.synonyms.Add(synonymModelItem);
                        synonymsCount++;
                    }
                    ret.Add(modelItem);
                    this.Log("Synonyms #["+ sqlsSynonyms .Count + "] in database" + dbName + "processed.");
                }
                catch (Exception ex)
                {

                    if (ex.Message.IndexOf("offline") >= 0)
                    {
                        ;//knonw error - databse is offline, ignore
                    }
                    else
                    {
                        this.Log("Error " + ex.Message + "\n" + ex.StackTrace);
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            return ret;
        }
    }
}

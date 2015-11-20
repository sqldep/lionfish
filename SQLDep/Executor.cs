using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace SQLDep
{
    class Executor
    {
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

        public void Run(string sqlConnection, int dbType, string customSqlSetName, string myKey, string sqlDialect, out string exportFileName)
        {
            try
            {
                this.LogFileName = "SQLDepLog.txt";
                string logJSONName = "Export_" + customSqlSetName + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".txt";
                exportFileName = logJSONName;
                // pripoj se do databaze
                this.Log("Before database open.");
                OdbcConnection connection = new OdbcConnection(sqlConnection);
                connection.Open();
                this.Log("Database open.");
                SQLCompleteStructure dbStructure = this.Run(connection, sqlDialect);

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
                dbStructure.userAccountId = myKey;
                dbStructure.customSqlSetName = customSqlSetName;
                myJson = this.SaveStructureToFile(dbStructure, logJSONName);
            }
            catch (Exception ex)
            {
                this.Log("Error " + ex.Message);
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
                this.Log("Error " + ex.Message);
                throw;
            }
        }

        private SQLCompleteStructure Run(OdbcConnection connection, string sqlDialect)
        {
            // The following SELECTS map to JSON (see example.json)
            SQLCompleteStructure ret = new SQLCompleteStructure();
            this.Log("Getting list of databases");
            List<string> dbNames = this.GetDbNames(connection, sqlDialect);
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
            ret.queries = this.GetQuerries(connection, sqlDialect, dbNames);
            this.Log("List of querries has " + ret.queries.Count + " items.");

            ret.databaseModel = new SQLDatabaseModel();
            ret.databaseModel.databases = this.GetDatabaseModels(connection, sqlDialect, dbNames);

            this.Log("Getting list of dblinks");
            ret.dblinks = this.GetDBLinks(connection, sqlDialect);
            this.Log("List of dblinks has " + ret.dblinks.Count + " items.");
            return ret;
        }

        private List<string> GetDbNames(OdbcConnection connection, string sqlDialect)
        {
            List<string> sqls = this.GetSQLCommands(sqlDialect, "databases", null);

            List<SQLResult> result = new List<SQLResult>();

            foreach (var item in sqls)
            {
                this.RunSql(connection, result, item);
            }

            List<string> ret = new List<string>();
            foreach (var item in result)
            {
#if DEBUG
                if(item.Column0.IndexOf("Nemocnice") < 0)
                {
                    continue;
                }
#endif

                ret.Add(item.Column0);
            }

            return ret;
        }

        private List<SQLQuerry> GetQuerries(OdbcConnection connection, string sqlDialect, List<string> dbNames)
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
                        this.RunSql(connection, result, item);
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

#if DEBUG
                        {
                            // for debug purposes make short ouputs
                            querryItem.sourceCode = querryItem.sourceCode.Substring(0, 25);
                            if (count > 3) break;
                        }
#endif

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


        private List<SQLDBLink> GetDBLinks (OdbcConnection connection, string sqlDialect)
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
                    this.RunSql(connection, result, item);
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
            var jsonSerialiser = new JavaScriptSerializer();
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

        private void RunSql(OdbcConnection connection, List<SQLResult> result, string cmd)
        {

            OdbcCommand toGo = connection.CreateCommand();
            toGo.CommandText = cmd;

            OdbcDataReader reader = toGo.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int nCol = reader.VisibleFieldCount;

                    SQLResult newItem = new SQLResult();

                    if (nCol > 0)
                        newItem.Column0 = reader.GetString(0);
                    if (nCol > 1)
                        newItem.Column1 = reader.GetString(1);
                    if (nCol > 2)
                        newItem.Column2 = reader.GetString(2);
                    if (nCol > 3)
                        newItem.Column3 = reader.GetString(3);
                    if (nCol > 4)
                        newItem.Column4 = reader.GetString(4);
                    if (nCol > 5)
                        newItem.Column5 = reader.GetString(5);
                    if (nCol > 6)
                        newItem.Column6 = reader.GetString(6);

                    result.Add(newItem);

                }
            }

            reader.Close();
            toGo.Dispose();
        }
 

        private List<SQLDatabaseModelItem> GetDatabaseModels(OdbcConnection connection, string sqlDialect, List<string> dbNames)
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
                        this.RunSql(connection, tablesWithColumns, item);
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
                            comment = item.Column6
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
                        this.RunSql(connection, synonyms, item);
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
                        this.Log("Error " + ex.Message);
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            return ret;
        }
    }
}

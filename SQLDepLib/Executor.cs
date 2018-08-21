using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Ionic.Zip;

namespace SQLDepLib
{
    public class Executor
    {

        public Executor(DBExecutor dbExecutor)
        {
            this.DBExecutor = dbExecutor;
            this.runId = Guid.NewGuid().ToString();
            this.ProgressInfo = new ProgressInfo();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public string runId { get; set; }

        public DBExecutor DBExecutor { get; private set; }

        public ProgressInfo ProgressInfo { get; private set; }

        private string myJson = string.Empty;

        public void Run(string customSqlSetName, Guid myKey, string sqlDialect, string exportFileName, bool useFs)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                // coult be useful
                Logger.Log(Environment.Is64BitOperatingSystem ? "64bit system" : "32bit system");

                DBExecutor.Connect();

                this.ProgressInfo.SetProgressPercent(10, "Collecting data from DB.");

                // this will fill some dbStructure fields, such as queries and tables...
                SQLCompleteStructure dbStructure = this.Run(sqlDialect, useFs);

                // append queries from FS
                if (useFs)
                {
                    this.ProgressInfo.SetProgressPercent(85, "Collecting data from FileSystem.");
                    this.GetQueriesFromFS(dbStructure);
                }

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
                this.ProgressInfo.SetProgressPercent(95, "Saving collected data to file.");
                myJson = this.SaveStructureToFile(dbStructure, exportFileName);
                DBExecutor.Close();
            }
            catch (Exception ex)
            {
                Logger.Log("Error " + ex.Message);
                throw;
            }
            finally
            {
                this.ProgressInfo.RemoveProgress();
            }

        }

        /// <summary>
        /// Gets all files that are matched with fileMask and appends them to dbStructure as queries.
        /// </summary>
        /// <param name="dbStructure"></param>
        private void GetQueriesFromFS(SQLCompleteStructure dbStructure)
        {
            Logger.Log("Getting data from Filesystem.");
            FileSystemData fsData = new FileSystemData();
            fsData.Load();

            string[] allFiles = Directory.GetFiles(fsData.ConfFile.InputDir, fsData.ConfFile.FileMask, SearchOption.AllDirectories);

            foreach (var path in allFiles)
            {
                string fileString = File.ReadAllText(path);
                SQLQuery newQuery = new SQLQuery();
                newQuery.name = Path.GetFileNameWithoutExtension(path);
                newQuery.sourceCode = fileString;
                newQuery.schema = fsData.ConfFile.DefaultSchema;
                newQuery.database = fsData.ConfFile.DefaultDatabase;
                dbStructure.queries.Add(newQuery);
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
                Logger.Log("Error " + ex.Message);
                throw;
            }
        }

        public void SendFiles(List<string> files, String apiKey)
        {
            var ms = new MemoryStream();
            {
                ZipFile zip = new ZipFile();

                foreach (var fileName in files)
                {
                    zip.AddFile(fileName, "");
                }

                // store to memory stream
                ms.Seek(0, SeekOrigin.Begin);
                zip.Save(ms);
                zip.Dispose();
            }

            Logger.Log("Before sending zipped data.");
            var baseAddress = "https://sqldep.com/api/batch/zip/";

            var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));

            string proxy = WebRequest.DefaultWebProxy.GetProxy(http.Address).AbsoluteUri;
            if (proxy != string.Empty)
            {
                Logger.Log("Proxy URL: " + proxy);
                http.Proxy = WebRequest.DefaultWebProxy;

                // experinemt with proxy setting
                http.Proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            }

            http.Accept = "application/json";
            http.ContentType = "application/zip";
            http.Headers["Authorization"] = "Token " + apiKey;
            http.Method = "POST";

            // vyssi verze protokolu
            http.ProtocolVersion = HttpVersion.Version11;

            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = ms.ToArray();
            http.ReadWriteTimeout = 1800 * 1000;
            http.Timeout = 1800 * 1000;

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
            Logger.Log("Data sent.");
        }

        public virtual SQLCompleteStructure Run(string sqlDialect, bool useFS)
        {            
            // The following SELECTS map to JSON (see example.json)
            SQLCompleteStructure ret = new SQLCompleteStructure();
            Logger.Log("Getting list of databases");
            List<string> dbNames = this.GetDbNames(sqlDialect);
            Logger.Log("List of databases has " + dbNames.Count + " items.");

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

            Logger.Log("Getting list of querries");
            this.ProgressInfo.SetProgressPercent(15, "Collecting queries.");
            if (!useFS)
            {
                if (sqlDialect == "oracle")
                {
                    Logger.Log("Using Oracle dialect");
                    ret.queries = this.GetOracleQuerries(sqlDialect, dbNames);
                }
                else
                {
                    ret.queries = this.GetQuerries(sqlDialect, dbNames);
                }
                Logger.Log("List of querries has " + ret.queries.Count + " items.");
            }
            else
            {
                ret.queries = new List<SQLQuery>();
            }
            this.ProgressInfo.SetProgressPercent(60, "Collecting database model.");
            ret.databaseModel = new SQLDatabaseModel();
            ret.databaseModel.databases = this.GetDatabaseModels(sqlDialect, dbNames);

            if (sqlDialect != "greenplum" && sqlDialect != "redshift" && sqlDialect != "postgres")
            {
                this.ProgressInfo.SetProgressPercent(62, "Colleting DB links.");
                Logger.Log("Getting list of dblinks");
                ret.dblinks = this.GetDBLinks(sqlDialect);
                Logger.Log("List of dblinks has " + ret.dblinks.Count + " items.");
            }

            return ret;
        }

        private List<string> GetDbNames(string sqlDialect)
        {
            List<string> sqls = this.GetSQLCommands(sqlDialect, Purpose.DATABASES, true, null);

            List<SQLResult> result = new List<SQLResult>();

            foreach (var item in sqls)
            {
                Logger.Log(string.Format("GetDbNames processing cmd {0}", item));
                DBExecutor.RunSql(result, item);
            }

            List<string> ret = new List<string>();
            foreach (var item in result)
            {
                Logger.Log(string.Format("GetDBNames - adding {0}", item));
                ret.Add(item.Column0);
            }

            Logger.Log(string.Format("GetDBNames - count {0}", ret != null ? (ret.Count).ToString() : "null"));

            return ret;
        }

        private List<SQLQuery> GetOracleQuerries(string sqlDialect, List<string> dbNames)
        {
            List<SQLQuery> ret = new List<SQLQuery>();

            bool firstSqlCommands = true;
            foreach (var dbName in dbNames)
            {
                //this.ProgressInfo.SetProgressDone((double)100* ++iiDbCounter / dbNames.Count, dbName);

                // sql commands
                List<StrReplace> replaces = new List<StrReplace>();
                StrReplace itemForReplace = new StrReplace()
                {
                    SearchText = "##DBNAME##",
                    ReplaceText = dbName
                };
                replaces.Add(itemForReplace);

                List<string> sqls = this.GetSQLCommands(sqlDialect, Purpose.QUERIES, firstSqlCommands, replaces);
                firstSqlCommands = false;

                try
                {
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
                        this.ProgressInfo.SetProgressPercent(15 + 30 * (counter / firstBlock.Count), "Collecting queries.");
                        if (item.Column5.Equals("1")) { // dump previous wholeCode

                            if (counter > 0)
                            {
                                SQLQuery queryItem = new SQLQuery()
                                {
                                    sourceCode = wholeCode.ToString(),
                                    name = queryName,
                                    groupName = queryGroup,
                                    database = queryDatabase,
                                    schema = querySchema
                                };

                                ret.Add(queryItem);
                                Logger.Log("Query done " + query_counter);
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
                        SQLQuery queryItem = new SQLQuery()
                        {
                            sourceCode = "CREATE OR REPLACE FORCE VIEW " + item.Column2 + " (" + item.Column5 + ") AS " +  item.Column0,
                            name = item.Column1,
                            groupName = item.Column2,
                            database = item.Column3,
                            schema = item.Column4
                        };

                        ret.Add(queryItem);
                    }

                    sqls.RemoveAt(0);
                    List<SQLResult> thirdBlock = new List<SQLResult>();
                    DBExecutor.RunQuerySql(thirdBlock, sqls.FirstOrDefault());

                    foreach (var item in thirdBlock)
                    {
                        SQLQuery queryItem = new SQLQuery()
                        {
                            sourceCode = "CREATE MATERIALIZED VIEW " + item.Column2 + " (" + item.Column5 + ") AS " + item.Column0,
                            name = item.Column1,
                            groupName = item.Column2,
                            database = item.Column3,
                            schema = item.Column4
                        };

                        ret.Add(queryItem);
                    }

                    sqls.RemoveAt(0);

                    // There could be custom code left
                    while (sqls.Count != 0)
                    {
                        String sql = sqls.FirstOrDefault();
                        List<SQLResult> customBlock = new List<SQLResult>();
                        DBExecutor.RunQuerySql(customBlock, sql);

                        foreach (var item in customBlock)
                        {
                            SQLQuery queryItem = new SQLQuery()
                            {
                                sourceCode = item.Column0,
                                name = item.Column1,
                                groupName = item.Column2,
                                database = item.Column3,
                                schema = item.Column4
                            };

                            ret.Add(queryItem);
                        }
                        sqls.RemoveAt(0);
                    }
                }
                catch (Oracle.ManagedDataAccess.Client.OracleException oe)
                {
                    Logger.Log("Last executed SQL dump:\n" + sqls.FirstOrDefault());
                    throw oe;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return ret;
        }

        private List<SQLQuery> GetQuerries(string sqlDialect, List<string> dbNames)
        {
            List<SQLQuery> ret = new List<SQLQuery>();

            int count = 0;
            bool firstSqlCommands = true;
            foreach (var dbName in dbNames)
            {
                //this.ProgressInfo.SetProgressDone((double)100 * ++iiDbCounter / dbNames.Count, dbName);
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

                    List<string> sqls = this.GetSQLCommands(sqlDialect, Purpose.QUERIES, firstSqlCommands, replaces);
                    firstSqlCommands = false;

                    List<SQLResult> result = new List<SQLResult>();
                    foreach (var item in sqls)
                    {
                        DBExecutor.RunSql(result, item);
                    }

                    foreach (var item in result)
                    {
                        this.ProgressInfo.SetProgressPercent(15 + 40 * (count / result.Count), "Collecting queries.");
                        SQLQuery queryItem = new SQLQuery()
                        {
                            sourceCode = item.Column0,
                            name = item.Column1,
                            groupName = item.Column2,
                            database = item.Column3,
                            schema = item.Column4
                        };


                        ret.Add(queryItem);
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

                List<string> sqls = this.GetSQLCommands(sqlDialect, Purpose.DBLINKS, true, null);

                List<SQLResult> result = new List<SQLResult>();

                foreach (var item in sqls)
                {
                    //this.ProgressInfo.SetProgressDone((double)100 * ++iiItem / sqls.Count, item);
                    DBExecutor.RunSql(result, item);
                }

                foreach (var item in result)
                {
                    this.ProgressInfo.SetProgressPercent(62 + 20*(count/result.Count), String.Format("Colleting DB links({}/{}).", count, result.Count));

                    SQLDBLink dblinkItem = new SQLDBLink()
                    {
                        owner = item.Column0,
                        name = item.Column1,
                        userName = item.Column2,
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

        public enum Purpose
        {
            QUERIES = 1,
            DATABASES = 2,
            DBLINKS = 3,
            TABLES = 4,
            SYNONYMS = 5,
        }

        protected List<string> GetSQLCommands(string sqlDialect, Purpose enumPurpose, bool isFirstOfThisType, List<StrReplace> list)
        {
            string purpose = enumPurpose.ToString().ToLower();

            // tyto jsou povinne
            string sqlCommands = System.IO.File.ReadAllText("./sql/" + sqlDialect + "/" + purpose + "/cmd.sql");

            //  Ted se sekce Queries v JSONu plni tak, ze se nacte napriklad definice procedury a ta se vlozi do Queries.
            //  Potrebujeme uzivatelum dat moznost, aby meli moznost vlozit vlastni SQL prikaz z jine tabulku a nejen databazoveho katalogu. A to by se ulozilo do Queries.
            // Navrhuju to resit tak, ze by vznikl v kazdem adresari s dialektemadresar custom, cili / sql / teradata / custom
            // Tam by se lionfish vzdy koukl a pokud by tam existoval nejaky fajl, tak by ho spustil.
            // Uvnitr by byl takovyhle nejaky dotaz, ktery by rovnou vracel vsechny fieldy, ktere jsou treba vyplnit v Queries.
            // SELECT
            //  sql_query_stmt as sourceCode,
            //  sql_name as name, --can be left empty
            //  sql_group_name as groupName, --can be empty
            //  'my-database-name' as databaseName
            //  'SCOTT' as schemaName
            // FROM
            //  ETL_LOG

            if (enumPurpose == Purpose.QUERIES)
            {
                if (isFirstOfThisType)
                {
                    string customPath = "./sql/" + sqlDialect + "/custom";

                    if (Directory.Exists(customPath))
                    {
                        Logger.Log(string.Format("Checked custom sql files in {0} - directory exists.", customPath));
                        foreach (var file in Directory.GetFiles(customPath))
                        {
                            if (file.EndsWith("sql", StringComparison.InvariantCultureIgnoreCase))
                            {
                                string customSqlCommands = System.IO.File.ReadAllText(file);

                                // add to standard
                                sqlCommands += "\n--split\n";
                                sqlCommands += customSqlCommands;

                                // log it
                                Logger.Log(string.Format("ConfFile {0} in custom directory succesfully added ({1} characters read).", file, customSqlCommands.Length));
                            }
                            else
                            {
                                Logger.Log(string.Format("ConfFile {0} in custom directory ignored - expected extension sql.", customPath));
                            }
                        }
                    }
                    else
                    {
                        Logger.Log(string.Format("Checked custom sql files in {0} - directory does not exist.", customPath));
                    }
                }
            }

            // a dale jedeme jako posledne
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
            querries.createdBy = "SQLdep v1.5.15";
            querries.exportId = this.runId;
            querries.physicalInstance = this.DBExecutor.Server;

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

            StreamWriter wr = File.CreateText(logJSONName);
            wr.Write(json);
            wr.Close();

            Logger.Log("Result data saved in " + logJSONName);
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

            Logger.Log("Before sending data.");
            var baseAddress = "https://sqldep.com/api/rest/sqlset/create/";

            var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));

            string proxy = WebRequest.DefaultWebProxy.GetProxy(http.Address).AbsoluteUri;
            if (proxy != string.Empty)
            {
                Logger.Log("Proxy URL: " + proxy);
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
            Logger.Log("Data sent.");
        }

        private List<SQLDatabaseModelItem> GetDatabaseModels(string sqlDialect, List<string> dbNames)
        {
            List<SQLDatabaseModelItem> ret = new List<SQLDatabaseModelItem>();

            int tableCount = 0;
            int synonymsCount = 0;

            bool firstSqlCommands = true;
            bool firstSqlCommands2 = true;
            foreach (var dbName in dbNames)
            {
                try
                {
                    SQLDatabaseModelItem modelItem = new SQLDatabaseModelItem();
                    modelItem.name = dbName;

                    Logger.Log("Getting tables in database '" + dbName + "'.");

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
                    List<string> sqlsTablesWithColumns = this.GetSQLCommands(sqlDialect, Purpose.TABLES, firstSqlCommands, replaces);
                    firstSqlCommands = false;

                    List<SQLResult> tablesWithColumns = new List<SQLResult>();
                    foreach (var item in sqlsTablesWithColumns)
                    {
                        DBExecutor.RunSql(tablesWithColumns, item);
                    }

                    foreach (var item in tablesWithColumns)
                    {
                        string tableName = item.Column2;
                        string schemaName = item.Column1;

                        SQLTableModelItem tableModelItem = modelItem.tables.Find(x => x.name == tableName && x.schema == schemaName);

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
                    Logger.Log("Tables #["+ modelItem.tables.Count + "] in database" + dbName + " processed.");

                    // synonyms
                    if (sqlDialect != "greenplum" && sqlDialect != "redshift" && sqlDialect != "postgres")
                    {
                        Logger.Log("Getting synonyms in database " + dbName + ".");

                        modelItem.synonyms = new List<SQLSynonymModelItem>();
                        List<string> sqlsSynonyms = this.GetSQLCommands(sqlDialect, Purpose.SYNONYMS, firstSqlCommands2, replaces);
                        firstSqlCommands2 = false;
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
                        
                        Logger.Log("Synonyms #[" + sqlsSynonyms.Count + "] in database" + dbName + "processed.");
                    }
                    ret.Add(modelItem);
                }
                catch (Exception ex)
                {

                    if (ex.Message.IndexOf("offline") >= 0)
                    {
                        ;//knonw error - databse is offline, ignore
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return ret;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SQLtest
{
    class Executor
    {

        public void Run (string sqlConnection, int dbType, string customSqlSetName, string myKey, string sqlDialect, List<string> failedDbs, bool debug)
        {
            

            string sqlCommands = System.IO.File.ReadAllText(sqlDialect + ".sql");
            string[] separator = { "GO--" };
            List<string> sqlCommandsList = sqlCommands.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();

            // pripoj se do databaze
            SqlConnection connection = new SqlConnection(sqlConnection);
            connection.Open();
            ClassQuerries querries = this.Run(connection, sqlCommandsList, failedDbs, debug);


            querries.dialect = sqlDialect;
            querries.userAccountId = myKey;


            querries.customSqlSetName = customSqlSetName;


            // send
           this.SendQuerries(querries);

        }


        public ClassQuerries Run(SqlConnection connection, List<string> sqlCommandsList, List<string> failedDbs, bool debug)
        {

            // prvni prikaz zjistuje databaze
            List<string> dbNames = this.RunSql(connection, sqlCommandsList.FirstOrDefault(), null, null, debug).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

            ClassQuerries querries = new ClassQuerries();
            querries.queries = new List<SQLData>();

            foreach (var dbName in dbNames)
            {
                int iCmd = 0;
                try
                {
                    foreach (var item in sqlCommandsList)
                    {
                        if (iCmd > 0)
                        {
                            {
                                string sqlCmd = item.Replace("##DBNAME##", dbName);

                                List<SQLData> sqldata = new List<SQLData>();

                                string ret = RunSql(connection, sqlCmd, sqldata, dbName, debug);


                                foreach (var item_data in sqldata)
                                {
                                    querries.queries.Add(item_data);

                                }

                            }
                        }

                        iCmd++;
                    }
                }
                catch(Exception)
                {
                    failedDbs.Add(dbName);
                }
                if (debug) break;
            }

            if (failedDbs.Count == dbNames.Count)
            {
                throw new Exception("None database was succesfully analyzed.");
            }

            return querries;
        }

        private void SendQuerries (ClassQuerries querries)
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


        }

        private string RunSql(SqlConnection connection, string cmd, List<SQLData> sqldata, string schema, bool debug)
        {
            string ret = string.Empty;

            SqlCommand toGo = connection.CreateCommand();
            toGo.CommandText = cmd;

            SqlDataReader reader = toGo.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int nCol = reader.VisibleFieldCount;

                    for (int ii = 0; ii < nCol; ii++)
                    {
                        if (ii > 0) ret += "\t";
                        ret += reader.GetString(ii);
                    }
                    if (sqldata != null)
                    {
                        SQLData newItem = new SQLData();
                        if (nCol >= 1)
                            newItem.schema = reader.GetString(0);
                        if (nCol >= 2)
                            newItem.groupName = reader.GetString(1);
                        if (nCol >= 3)
                        {
                            if (debug)
                            {
                                newItem.sourceCode = reader.GetString(2).Substring(0, 30);
                            }
                            else
                            {
                                newItem.sourceCode = reader.GetString(2);
                            }
                        }

                        newItem.database = schema;
                        sqldata.Add(newItem);
                    }

                    ret += "\n";
                }
            }

            reader.Close();
            toGo.Dispose();

            return ret;
        }
    }
}

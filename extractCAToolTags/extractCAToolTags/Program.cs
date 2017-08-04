using System;
using System.Net; 
using System.Configuration;
//Install-Package Newtonsoft.Json 
using Newtonsoft.Json;
using System.IO; 
using System.Collections.Generic; 
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient; 
using System.Threading.Tasks;
using System.Linq;

namespace extractCAToolTags
{


    class Program
    {

        static NameValueCollection nvc = ConfigurationManager.AppSettings;
        static string sqlquery = @"WITH cte " +
           "AS (SELECT *, " +
      "'https://cas-portal-uat-api.azurewebsites.net/api/v2.0/instances?product=' " +
      "+ product + '&platform=' + [platform] " +
      "+ '&queue=&date={\"startDate\":\"' + startdate " +
      "+ '\",\"endDate\":\"' + enddate + '\"}&code=' + id requestURL," +
      "Row_number() " +
      "  OVER(" +
      "    partition BY product, platform, startdate, enddate, level1node," +
      "  level2node" +
      "    ORDER BY indate DESC)   rn," +
      " id % 8[GROUP]" +
      " FROM   tagssumarizedtable) " +
      "SELECT *" +
      "FROM   cte " +
      "WHERE  rn = 1 " +
      "ORDER BY [group] ";

        static string ConnectionString = @"Server=10.168.178.132;Database=LithiumDatabase2;User Id=RWuser4SocialMediaTestDB;Password = Sjynige2b;";
        static DateTime startDate_ = new DateTime(2017, 1, 1);

        //static string endDate = ConfigurationManager.AppSettings["endDate"]; 

        static void Main(string[] args)
        {

            clearTwoTablesInDatabase();
            
            loadDataToTagsSummizedTable(); 

            IList<DataTable> dttbllist = PullData();

            loadDataToTagsDetailedTable(dttbllist);  

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static void clearTwoTablesInDatabase()
        { 
                using(SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("TRUNCATE TABLE TagsSumarizedTable; TRUNCATE TABLE TagsDetailsTable", connection);
                connection.Open();

                cmd.ExecuteNonQuery();
                // this will query your database and return the result to your datatable
                 
                connection.Close(); 
            }
        }

        private static void loadDataToTagsDetailedTable(IList<DataTable> dttbllist)
        {
            int counter = 1;
            Parallel.ForEach(dttbllist, (datatbl) =>
            {
                
               DataTable dataTable  =  GetDetailTable();

                foreach (DataRow row in datatbl.Rows)
                {

                    IList<Thread> threads =  getThreadInfo(row["requestURL"].ToString());

                    foreach (Thread thread in threads)
                    { 

                        dataTable.Rows.Add(row["product"].ToString(), row["platform"].ToString(),
                            row["Level1Node"].ToString(), row["Level2Node"].ToString(),
                            thread.id, thread.orignalQuestionId, thread.title, thread.url, thread.queue.displayname,
                            thread.createdOn, thread.sentiment);

                        
                        Console.WriteLine(thread.url);
                        Console.WriteLine("{0} threads are proceeded", counter);
                        counter++; 
                    } 
                }

                BulkInsert(dataTable, "TagsDetailsTable"); 

            });
        }

        private static IList<DataTable> PullData()
        {
            DataTable dataTable = new DataTable();


            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(sqlquery, connection);
                connection.Open();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataTable);
                connection.Close();
                da.Dispose();
            }

            var uniqueList = dataTable.AsEnumerable().Select(x => x.Field<int>("group")).Distinct();
            List<int> myList = new List<int>();
            myList = uniqueList.ToList();

            IList<DataTable> dataTableList = new List<DataTable>(); 

            foreach (int item in myList)
            {
                var Result = from x in dataTable.AsEnumerable()
                             where x.Field<int>("group") == item
                             select x;
                DataTable table = Result.CopyToDataTable();
                dataTableList.Add(table); 
            }

            return dataTableList; 

        }




    private static void loadDataToTagsSummizedTable()
        {
            Parallel.ForEach(nvc.AllKeys, (key) =>
            {
                DateTime startDateInLoop = startDate_;

                do
                {

                    IList<Parent> ParentTags = getProductInfo(key, nvc[key], startDateInLoop.ToString("s") + "Z", startDateInLoop.AddMonths(1).ToString("s") + "Z");

                    DataTable sumarizedTable = GetSummarizedTable();

                    foreach (Parent p in ParentTags)
                    {
                        foreach (Child c in p.children)
                        {
                            sumarizedTable.Rows.Add(key, nvc[key], startDateInLoop.ToString("s") + "Z", startDateInLoop.AddMonths(1).ToString("s") + "Z", p.name, c.name, c.data, c.id);
                        }
                    }

                    BulkInsert(sumarizedTable, "TagsSumarizedTable");

                    startDateInLoop = startDateInLoop.AddMonths(1);


                } while (startDateInLoop < DateTime.Now);

            });
        }

        static DataTable GetSummarizedTable()
        {
            // Here we create a DataTable with four columns.
            DataTable table = new DataTable();

            table.Columns.Add("product", typeof(string));
            table.Columns.Add("platform", typeof(string));
            table.Columns.Add("StartDate", typeof(string));
            table.Columns.Add("EndDate", typeof(string));
            table.Columns.Add("Level1Node", typeof(string));
            table.Columns.Add("Level2Node", typeof(string));
            table.Columns.Add("Amount", typeof(int));
            table.Columns.Add("Id", typeof(string));

            return table;
        }

        static DataTable GetDetailTable() {

            DataTable table = new DataTable(); 

            table.Columns.Add("product", typeof(string));
            table.Columns.Add("platform", typeof(string)); 
            table.Columns.Add("Level1Node", typeof(string));
            table.Columns.Add("Level2Node", typeof(string));
            table.Columns.Add("id", typeof(string));
            table.Columns.Add("orignalQuestionId", typeof(string));
            table.Columns.Add("title", typeof(string));
            table.Columns.Add("url", typeof(string));
            table.Columns.Add("queueDisplayname", typeof(string));
            table.Columns.Add("createdOn", typeof(string));
            table.Columns.Add("sentiment", typeof(int)); 

            return table;


        }

        public static void BulkInsert(DataTable dt, string TableName)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                // make sure to enable triggers
                // more on triggers in next post
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy
                    (
                    connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                    );

                // set the destination table name
                bulkCopy.DestinationTableName = TableName;
                connection.Open();

                // write the data in the "dataTable"
                bulkCopy.WriteToServer(dt);
                connection.Close();
            }
            // reset
            //this.dataTable.Clear();
        }

        static IList<Parent> getProductInfo(string product, string platform, string startDate, string endDate)
        {

            HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp("http://cas-portal-uat-api.azurewebsites.net/api/v1.0/products/" + product + "/tree?platform=" + platform + "&queueId=&date={%22startDate%22:%22" + startDate + "%22,%22endDate%22:%22" + endDate + "%22}");
            //POST web request to create a datasource.
            request.KeepAlive = true;
            request.Method = "GET";
            request.ContentLength = 0;

            IList<Parent> responseJson = null;

            //Write JSON byte[] into a Stream
            using (var response = (HttpWebResponse)request.GetResponse())
            {

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                responseJson = JsonConvert.DeserializeObject<IList<Parent>>(responseString);

            }
            return responseJson;
        }
        static IList<Thread> getThreadInfo(String url)
        {

            //HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp("http://cas-portal-uat-api.azurewebsites.net/api/v1.0/products/" + product + "/tree?platform=" + platform + "&queueId=&date={%22startDate%22:%22" + startDate + "%22,%22endDate%22:%22" + endDate + "%22}");
            HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp(url);
            //POST web request to create a datasource.
            request.KeepAlive = true;
            request.Method = "GET";
            request.ContentLength = 0;

            IList<Thread> responseJson = null;

            //Write JSON byte[] into a Stream
            using (var response = (HttpWebResponse)request.GetResponse())
            {

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                responseJson = JsonConvert.DeserializeObject<IList<Thread>>(responseString);

            }
            return responseJson;
        }


    }


    class Thread
    {

        [JsonProperty("id")]
        public string id { set; get; }

        [JsonProperty("orignalQuestionId")]
        public string orignalQuestionId { set; get; }

        [JsonProperty("title")]
        public string title { set; get; }

        [JsonProperty("url")]
        public string url { set; get; }

        [JsonProperty("createdOn")]
        public string createdOn { set; get; }

        [JsonProperty("sentiment")]
        public string sentiment { set; get; }

        [JsonProperty("queue")]
        public Queue queue { set; get; }
    }

    class Queue
    {
        [JsonProperty("id")]
        public string id { set; get; }
        [JsonProperty("name")]
        public string name { set; get; }
        [JsonProperty("displayname")]
        public string displayname { set; get; }

    }

    class Child
    {

        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("data")]
        public string data { get; set; }
    }

    class Parent
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("data")]
        public string data { get; set; }

        [JsonProperty("children")]
        public IList<Child> children { get; set; }
    }

}

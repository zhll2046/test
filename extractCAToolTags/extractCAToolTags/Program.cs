using System;
using System.Net;
using System.Text;
using System.Configuration;
//Install-Package Newtonsoft.Json 
using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Net.Http;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Threading.Tasks;

namespace extractCAToolTags
{

    
    class Program
    {

        static NameValueCollection nvc = ConfigurationManager.AppSettings;


        static string ConnectionString = @"Server=wx-sql-test-01;Database=UserVoiceDB;User Id=sa;Password = sa;";
        static DateTime startDate_ = new DateTime(2017,1,1);

        //static string endDate = ConfigurationManager.AppSettings["endDate"]; 

        static void Main(string[] args)
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
             
            Console.WriteLine("Done");
            Console.ReadKey();
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

        static IList<Parent> getProductInfo(string product, string platform, string startDate , string endDate)
        {

            HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp("http://cas-portal-uat-api.azurewebsites.net/api/v1.0/products/"+product+"/tree?platform="+platform+"&queueId=&date={%22startDate%22:%22"+startDate+"%22,%22endDate%22:%22"+endDate+"%22}");
            //POST web request to create a datasource.
            request.KeepAlive = true;
            request.Method = "GET";
            request.ContentLength = 0;

            IList<Parent> responseJson=null;

            //Write JSON byte[] into a Stream
            using (var response = (HttpWebResponse)request.GetResponse())
            {

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                responseJson = JsonConvert.DeserializeObject<IList<Parent>>(responseString);
                 
            }
            return responseJson;
        }
         

    }

    class Child {

        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("data")]
        public string data { get; set; }
    }

    class Parent {
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

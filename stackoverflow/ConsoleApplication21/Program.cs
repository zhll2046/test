using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication21
{
    class Program
    {
        public static int fromdate = 1388534400;
        public static int todate = 1488153600;
        public static string ConnectionString = "Server=10.168.176.30;Database=SocialMediaTest;User Id=RWuser4SocialMediaTestDB;Password = Sjynige3b;";
        public static string questionTableName = "ThirdPartyPBICases";
        public static string answerTableName = "ThirdPartyPBIAnswers";
        public static string site = "stackoverflow";
        static void Main(string[] args)
        {
             
            string tag = "microsoft-powerbi";

            /**

            IList<Question> Questions=GetQuestions(site, tag);

            InsertQuestion(Questions,site);
            **/


            IList<string> list = GetAnswerUrls(); 

            foreach (string url in list) { 

                IList<Answer> alist =  GetAnsers(url);
                InsertAnswers(alist,site);  

            }


            Console.WriteLine("FINISHED");
            Console.ReadKey();


        }


        private static void InsertAnswers(IList<Answer> Answers, string site)
        {
            DataTable datatbl = ConvertToDatatable<Answer>(Answers);

            datatbl.Columns.Add("site");

            foreach (DataRow row in datatbl.Rows)
            {
                row["site"] = site;  
            }
            
            BulkInsert(datatbl, answerTableName);

        }


        private static IList<Answer> GetAnsers(string url)
        {
            IList<Answer> Answers = new List<Answer>();
            int counter = 1;

            while (true)
            { 
                string responseContent = DownloadString(url.Replace("|pageNo|", counter.ToString()));

                responseAnswerJSON rj = JsonConvert.DeserializeObject<responseAnswerJSON>(responseContent);

                AddRange(Answers, rj.items);

                //if no more question, break
                if (!rj.has_more)
                {
                    break;
                } 

                counter++;
                Console.WriteLine(counter);

            }

            return Answers;


        }

        private static IList<string> GetAnswerUrls() {

            IList<string> answersUrls = new List<string>();

            Dictionary<string, List<string>> questionIDs = GetQuestionIDsFromDB();

            Dictionary<string, List<List<string>>> dicQID = new Dictionary<string, List<List<string>>>();

            foreach (string key in questionIDs.Keys)
            {

                //Console.WriteLine(key);
                //Console.WriteLine(questionIDs[key].Count);
                dicQID.Add(key, SplitListToChunks(questionIDs[key], 100));

            }

            foreach (string key in dicQID.Keys)
            {

                Console.WriteLine(key);

                foreach (List<string> l in dicQID[key])
                {
                    string[] array = new string[l.Count];
                    l.CopyTo(array, 0);
                    //Console.WriteLine(String.Join(";",array));
                    answersUrls.Add(string.Format(@"https://api.stackexchange.com/2.2/questions/{0}/answers?page=|pageNo|&pagesize=100&order=desc&sort=activity&site={1}&filter=!9YdnSKu68", String.Join(";", array),key));
                      
                }
                
            }
              
            return answersUrls; 

        }

        private static List<List<string>> SplitListToChunks(List<string> list, int  nSize = 30)
        {

            var list2 = new List<List<string>>();

            for (int i = 0; i < list.Count; i += nSize)
            {
                list2.Add(list.GetRange(i, Math.Min(nSize, list.Count - i)));
            }

            return list2;


        }

        private static Dictionary<string, List<string>> GetQuestionIDsFromDB()
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<String>>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT site, Question_id FROM ThirdPartyPBICases WHERE site='"+site+"'", connection))
            {
                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    
                    // Check is the reader has any rows at all before starting to read.
                    if (reader.HasRows)
                    {
                        // Read advances to the next row.
                        while (reader.Read())
                        {
                            string site = reader.GetString(reader.GetOrdinal("site"));
                            if (dic.ContainsKey(site))
                            {
                                dic[site].Add(reader.GetString(reader.GetOrdinal("Question_id")));
                            }
                            else
                            {
                                dic.Add(site, new List<string>());
                                dic[site].Add(reader.GetString(reader.GetOrdinal("Question_id")));

                            }


                             
                        }
                    }
                }
            }

            return dic;
        }

        private static DataTable ConvertToDatatable<T>(IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    table.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Count];

            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }


        public static void BulkInsert(DataTable dt,string TableName)
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

        private static void InsertQuestion(IList<Question> questions, string site)
        {
            DataTable datatbl = ConvertToDatatable<Question>(questions);
             
            datatbl.Columns.Add("site");
            datatbl.Columns.Add("TagsTemp");


            foreach (DataRow row in datatbl.Rows)
            {
               row["site"]= site;


                IList list = (IList)row["tags"];

                string[] array = new string[list.Count];
                list.CopyTo(array, 0);
                row["TagsTemp"] = String.Join(";", array);
                 
            }

            datatbl.Columns.Remove("tags");
            datatbl.Columns["TagsTemp"].ColumnName = "tags"; 


            BulkInsert(datatbl, questionTableName);  

        }
         

        private static void AddRange<T>(ICollection<T> collection, IEnumerable<T> enumerable)
        {
            foreach (var cur in enumerable)
            {
                collection.Add(cur);
            }
        }

        private static IList<Question> GetQuestions(string site, string tag)
        {
            IList<Question> Questions = new List<Question>();
            int counter = 1;

            while (true)
            {

                string url = @"https://api.stackexchange.com/2.2/questions?page=" + counter + "&pagesize=100&fromdate=" + fromdate + "&todate=" + todate + "&order=desc&sort=creation&tagged=" + tag + "&site=" + site; 

                string responseContent = DownloadString(url);

                responseJSON rj = JsonConvert.DeserializeObject<responseJSON>(responseContent);

                AddRange(Questions, rj.items);

                //if no more question, break
                if (!rj.has_more)
                {
                    break;
                }

                counter++;

            }

            return Questions;


        }



        public static string DownloadString(string url)
        {
            WebClient wc = null;
            try
            {
                wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                byte[] b = wc.DownloadData(url);

                MemoryStream output = new MemoryStream();
                using (GZipStream g = new GZipStream(new MemoryStream(b), CompressionMode.Decompress))
                {
                    g.CopyTo(output);
                }

                return Encoding.UTF8.GetString(output.ToArray());
            }
            catch
            {

            }
            finally
            {
                if (wc != null)
                {
                    wc.Dispose();
                }
            }
            return null;
        } 
    }

    public class Answer
    {
        public bool is_accepted { get; set; }
        public int score { get; set; }
        public string creation_date { get; set; }
        public string answer_id { get; set; }
        public string question_id { get; set; }
        public string link { get; set; } 

    }

    public class responseJSON
    {
        public IList<Question> items { get; set; }

        public bool has_more { get; set; }


    }
    public class responseAnswerJSON
    {
        public IList<Answer> items { get; set; }

        public bool has_more { get; set; }


    }



    public class Question
    {
        public int view_count { get; set; }

        public int answer_count { get; set; }
        public bool is_answered { get; set; }
        public string creation_date { get; set; }
        public string question_id { get; set; }
        public string link { get; set; }

        public string title { get; set; }
        public IList<string> tags { get; set; }
    }


}

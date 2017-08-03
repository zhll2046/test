using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExtractFlowBlog



    class Response
    {
        public Int64 lastModified;
        public IList<Post> posts;
        public BlogThread thread;

    }

    class BlogThread
    {
        public string id;
        public string link;
    }

    class Post
    {
        public string id;
        public string parent;
        public Author author;
        public string raw_message;
        public string createdAt;
    }

    class Post_
    {
        public string thread_id { get; set; }
        public string thread_link { get; set; }
        public string id { get; set; }
        public string parent { get; set; }
        public string author { get; set; }
        public string raw_message { get; set; }
        public string createdAt { get; set; }

        public Post_(string thread_id,
         string thread_link,
         string id,
         string parent,
         string author,
         string raw_message,
         string createdAt)
        {
            this.thread_id = thread_id;
            this.thread_link = thread_link;
            this.id = id;
            this.parent = parent;
            this.author = author;
            this.raw_message = raw_message;
            this.createdAt = createdAt;
        }

    }

    class Author
    {
        public string username;
        public string id;
    }

    class Program
    {
        public static string root = @"https://flow.microsoft.com";
        public static string ConnectionString = @"Server=ALANYAO-12R2;Database=SocialMediaTest;User Id=RWuser4SocialMediaTestDB;Password = Sjynige3b;";
        static void Main(string[] args)
        {

            string url = @"https://disqus.com/embed/comments/?base=default&version=1973d3a3c1957d897d05fa77fa3b9839&f=microsoftflow&t_i=en-us-blog-welcome-to-microsoft-flow&t_u=https://flow.microsoft.com/en-us/blog/welcome-to-microsoft-flow/";
            //HtmlDocument hdoc = Crawler.getHtmlDoc(url);
            //StreamWriter sw = new StreamWriter(@"c:\test\2.txt");

            //var commentsData = JsonConvert.DeserializeObject<dynamic>(DecodeEncodedNonAsciiCharacters(hdoc.GetElementbyId("disqus-threadData").InnerHtml));


            /// sw.WriteLine(commentsData["response"].ToString());
            //var Response = JsonConvert.DeserializeObject<Response>(commentsData["response"].ToString());

            // sw.Close();


            IList<Post_> ilposts = new List<Post_>(); 

            Dictionary<string, string> dic = GetAllBlogs();
            //Console.WriteLine(dic.Keys.Count);
            foreach (string key in dic.Keys)
            {

                string commentsRoot = @"https://disqus.com/embed/comments/?base=default&version=1973d3a3c1957d897d05fa77fa3b9839&f=microsoftflow&t_i=";
                string commentsSub = key.Substring(key.IndexOf("en-us"));

                string commentUrl = commentsRoot + commentsSub.Remove(commentsSub.Length - 1).Replace(@"/", "-") + "&t_u=" + key;

                HtmlDocument hdoc = Crawler.getHtmlDoc(commentUrl);

                var commentsData = JsonConvert.DeserializeObject<dynamic>(DecodeEncodedNonAsciiCharacters(hdoc.GetElementbyId("disqus-threadData").InnerHtml));


                /// sw.WriteLine(commentsData["response"].ToString());
                Response response = JsonConvert.DeserializeObject<Response>(commentsData["response"].ToString());

                string threadid = response.thread.id;
                string thread_link = response.thread.link;

                foreach (Post pst in response.posts)
                {
                    Post_ p_ = new Post_(threadid, thread_link, pst.id, pst.parent, pst.author.username, pst.raw_message, pst.createdAt);
                    ilposts.Add(p_);
                }



            }

            Console.WriteLine("DONE");
            DataTable DT = ConvertToDatatable<Post_>(ilposts);
            BulkInsert(DT, "MicrosoftFlowComments");



            SqlConnection cnn = new SqlConnection(ConnectionString);

            cnn.Open();

            SqlCommand cmd2 = new SqlCommand("dealMicrosoftFlowComments", cnn);
            cmd2.CommandType = CommandType.StoredProcedure;

            cmd2.ExecuteNonQuery();

            cnn.Close();




            Console.WriteLine(ilposts.Count);
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

        private static Dictionary<string, string> GetAllBlogs()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            int page = 1;

            while (true)
            {

                string url = @"https://flow.microsoft.com/en-us/blog/?page=" + page;
                HtmlDocument hdoc = Crawler.getHtmlDoc(url);
                HtmlNode node = hdoc.GetElementbyId("blog-results");

                if (node.SelectNodes("div/h3")?[0]?.InnerHtml != "No blog posts were found.")
                {

                    foreach (HtmlNode childnode in node.SelectNodes("div[contains(@class,'row column')]"))
                    {

                        HtmlNode targetNode = childnode.SelectNodes("article/div/h2/a")[0];

                        dic.Add(root + targetNode.GetAttributeValue("href", ""), targetNode.InnerHtml);

                    }

                    page++;

                }
                else
                {

                    Console.WriteLine("No more blog posts were found, totall {0} pages", page - 1);
                    break;
                }
            }

            return dic;

        }


        static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        static string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m =>
                {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }
    }
}

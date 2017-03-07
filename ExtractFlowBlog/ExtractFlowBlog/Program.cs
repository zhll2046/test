using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExtractFlowBlog
{

    class Program
    {
        public static string root = @"https://flow.microsoft.com";
        static void Main(string[] args)
        {
             
            string url = @"https://disqus.com/embed/comments/?base=default&version=1973d3a3c1957d897d05fa77fa3b9839&f=microsoftflow&t_i=en-us-blog-welcome-to-microsoft-flow&t_u=https://flow.microsoft.com/en-us/blog/welcome-to-microsoft-flow/";
            HtmlDocument hdoc = Crawler.getHtmlDoc(url);
            StreamWriter sw = new StreamWriter(@"c:\test\2.txt");

            sw.WriteLine(DecodeEncodedNonAsciiCharacters(hdoc.GetElementbyId("disqus-threadData").InnerHtml));

            Console.WriteLine("DONE");
             
           

            //Dictionary<string, string> dic = GetAllBlogs();
            //Console.WriteLine(dic.Keys.Count);
            //foreach (string key in dic.Keys)
            //{

            //    string commentsRoot = @"https://disqus.com/embed/comments/?base=default&version=1973d3a3c1957d897d05fa77fa3b9839&f=microsoftflow&t_i=";
            //    string commentsSub = key.Substring(key.IndexOf("en-us"));

            //    string commentUrl = commentsRoot + commentsSub.Remove(commentsSub.Length - 1).Replace(@"/", "-") + "&t_u=" + key;

            //    sw.WriteLine(commentUrl);
            //}
            sw.Close();

            Console.ReadKey();
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

                    Console.WriteLine("No blog posts were found");
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
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }
    }
}

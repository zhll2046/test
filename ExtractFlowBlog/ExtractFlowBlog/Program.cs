using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractFlowBlog
{

    class Program
    {
        public static string root = @"https://flow.microsoft.com";
        static void Main(string[] args)
        {

            GetAllBlogs();

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
    }
}

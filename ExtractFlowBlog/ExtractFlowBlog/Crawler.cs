using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractFlowBlog
{
    class Crawler
    {

        public static HtmlDocument getHtmlDoc(string url) {

            HtmlWeb page = new HtmlWeb();
            HtmlDocument document = page.Load(url);
            return document;

        }



    }
}

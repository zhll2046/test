using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Load += new EventHandler(Form1_Load);
            this.webBrowser1.DocumentCompleted +=
                new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);

            this.webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate(@"https://flow.microsoft.com/en-us/blog/managing-flow-resources-in-the-admin-center/");

            //Thread.Sleep(15000);


            PageLoad(100);
             
             

            

        }


        private async Task PageLoad(int TimeOut)
        {
            TaskCompletionSource<bool> PageLoaded = null;
            PageLoaded = new TaskCompletionSource<bool>();
            int TimeElapsed = 0;
            //webBrowser1.DocumentCompleted += (s, e) =>
            //{
            //    if (webBrowser1.ReadyState != WebBrowserReadyState.Complete) return;
            //    if (PageLoaded.Task.IsCompleted) return; PageLoaded.SetResult(true);
            //};
            ////
            //while (PageLoaded.Task.Status != TaskStatus.RanToCompletion)
            //{
               
            //    TimeElapsed++;
            //    if (TimeElapsed >= TimeOut * 100) PageLoaded.TrySetResult(true);
            //}
            await Task.Delay(10000);//interval of 10 ms worked good for me
            HtmlDocument hdoc = webBrowser1.Document;

            foreach (var a in hdoc.Window.Frames) {

                MessageBox.Show(a.ToString());

            }


        https://disqus.com/embed/comments/?base=default&amp;version=1973d3a3c1957d897d05fa77fa3b9839&amp;f=microsoftflow&amp;t_i=en-us-blog-managing-flow-resources-in-the-admin-center&amp;t_u=https%3A%2F%2Fflow.microsoft.com%2Fen-us%2Fblog%2Fmanaging-flow-resources-in-the-admin-center%2F&amp;t_e=Manage%20Flows%20in%20the%20Admin%20Center%20and%20two%20new%20services&amp;t_d=Manage%20Flows%20in%20the%20Admin%20Center%20and%20two%20new%20services&amp;t_t=Manage%20Flows%20in%20the%20Admin%20Center%20and%20two%20new%20services&amp;s_o=default&amp;l=en

            string html = hdoc.GetElementById("dsq-app1").InnerHtml;  

            StreamWriter sw = new StreamWriter(@"c:\test\233333.txt");

            sw.Write(html);

            sw.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             

            

        }



        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
             
            // this.webBrowser1.Navigate("https://flow.microsoft.com/en-us/blog/managing-flow-resources-in-the-admin-center/");
        }

        void webKitBrowser1_Navigated(object sender,
     WebBrowserNavigatedEventArgs e)
        {
            //textBox1.Text = webKitBrowser1.Url.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebKitTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
            this.webBrowser1.Navigated +=
                new WebBrowserNavigatedEventHandler(webKitBrowser1_Navigated);

            webBrowser1.Navigate(@"https://flow.microsoft.com/en-us/blog/managing-flow-resources-in-the-admin-center/");
        }

        void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.DocumentText =
                "<h1><a href=\"http://google.com\">Hello, World!</a></h1>";
        }

        void webKitBrowser1_Navigated(object sender,
            WebBrowserNavigatedEventArgs e)
        {
            //textBox1.Text = webKitBrowser1.Url.ToString();
        }
    }
}

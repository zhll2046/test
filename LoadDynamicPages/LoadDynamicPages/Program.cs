using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WbFetchPage
{
    public partial class MainForm  
    {
        private static WebBrowser webBrowser = new WebBrowser();

        public static void Main()
        {
            SetFeatureBrowserEmulation(); 
             MainForm_Load();
        }

        // start the task
        async static void MainForm_Load()
        {
            try
            {
                var cts = new CancellationTokenSource(10000); // cancel in 10s
                var html = await LoadDynamicPage("https://www.google.com/#q=where+am+i", cts.Token);
                MessageBox.Show(html.Substring(0, 1024) + "..."); // it's too long!
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // navigate and download 
        async static Task<string> LoadDynamicPage(string url, CancellationToken token)
        {
            // navigate and await DocumentCompleted
            var tcs = new TaskCompletionSource<bool>();
            WebBrowserDocumentCompletedEventHandler handler = (s, arg) =>
                tcs.TrySetResult(true);

            using (token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: true))
            {
                 webBrowser.DocumentCompleted += handler;
                try
                {
                     webBrowser.Navigate(url);
                    await tcs.Task; // wait for DocumentCompleted
                }
                finally
                {
                     webBrowser.DocumentCompleted -= handler;
                }
            }

            // get the root element
            var documentElement =  webBrowser.Document.GetElementsByTagName("html")[0];

            // poll the current HTML for changes asynchronosly
            var html = documentElement.OuterHtml;
            while (true)
            {
                // wait asynchronously, this will throw if cancellation requested
                await Task.Delay(500, token);

                // continue polling if the WebBrowser is still busy
                if ( webBrowser.IsBusy)
                    continue;

                var htmlNow = documentElement.OuterHtml;
                if (html == htmlNow)
                    break; // no changes detected, end the poll loop

                html = htmlNow;
            }

            // consider the page fully rendered 
            token.ThrowIfCancellationRequested();
            return html;
        }

        // enable HTML5 (assuming we're running IE10+)
        // more info: http://stackoverflow.com/a/18333982/1768303
        static void SetFeatureBrowserEmulation()
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
                return;
            var appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                appName, 10000, RegistryValueKind.DWord);
        }
    }
}
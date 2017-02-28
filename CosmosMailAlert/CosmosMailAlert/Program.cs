using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace ConsoleApplication19
{
    class Program
    {
        static void Main(string[] args)
        {
            string logPath = ConfigurationManager.AppSettings["logPath"];
            int expectedSucceedLines = Convert.ToInt32(ConfigurationManager.AppSettings["expectedSucceedLines"]);

            string fileNamePart = DateTime.Today.ToString("yyyyMMdd");

            Regex reg = new Regex(fileNamePart);

            DirectoryInfo di = new DirectoryInfo(logPath);
            FileInfo[] fiArr = di.GetFiles();

            Array.Sort(fiArr, delegate (FileInfo y, FileInfo x) { return x.CreationTime.CompareTo(y.CreationTime); });


            string mailBodyTail = "\n============================================================\nIF ANY ERROR OCCURS, PLEASE CHECK THE JOB AND LOG ON 10.168.176.30\r\n" +
                "\nThe daily Task in Task scheduler" +
                "\nName: COSMOSDataLoad\nAction: C:\\Windows\\System32\\cmd.exe / c \"D:\\COSMOSDataLoad\\sqlizer\\SQLizerConsole\\SQLizer.Console.exe\"" +
                " /x D:\\COSMOSDataLoad\\sqlizer\\SQLizerConsole\\conf\\sqlizer.xml\n\r\nLog Path: " + @"D:\COSMOSDataLoad\sqlizer\SQLizerLog\SQLizerConsole";

            //If the log files exists and the name match fileNamePart
            if (fiArr.Length > 0 && reg.IsMatch(fiArr[0].Name))
            {

                string[] MatchedLines = getLinesMatchPattern(logPath + "\\" + fiArr[0].Name, "Successfully validated stream");

                //if  MatchedLines = expectedSucceedLines
                if (MatchedLines.Length == expectedSucceedLines)
                {
                    string suceessMessage = String.Join(Environment.NewLine, MatchedLines);

                    //Console.WriteLine(suceessMessage);

                    sendmail("Succeed to download Cosmos Data", suceessMessage + mailBodyTail);
                    //send suceess mail with log
                }

                else
                {
                    string suceessMessage = String.Join(Environment.NewLine, MatchedLines);

                    //Console.WriteLine(suceessMessage);

                    sendmail("Partially download Cosmos Data, please check the log on server", suceessMessage + mailBodyTail);


                }


            }
            else
            {
                //send failed mail alert here
                sendmail("failed to download Cosmos Data, please check the log on server", mailBodyTail);

            }

        }

        private static void sendmail(string mailSubject, string mailBody)
        {

            int port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpServerPort"]);
            string host = ConfigurationManager.AppSettings["smtpServer"];
            string mailAccount = ConfigurationManager.AppSettings["mailAccount"];
            string pwd = ConfigurationManager.AppSettings["mailPassword"];
            string mailToList = ConfigurationManager.AppSettings["mailToList"];
            string mailCopyList = ConfigurationManager.AppSettings["mailCopyList"];

            SmtpClient client = new SmtpClient();
            client.Port = port;
            client.Host = host;
            client.EnableSsl = true;
            client.Timeout = 100000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(mailAccount, pwd);

            MailMessage mailMessage = new MailMessage();

            foreach (var address in mailToList.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                mailMessage.To.Add(address);
            }

            foreach (var address in mailCopyList.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                mailMessage.CC.Add(address);
            }

            mailMessage.From = new MailAddress(mailAccount);
            mailMessage.Subject = mailSubject;
            mailMessage.Body = mailBody;
            mailMessage.BodyEncoding = UTF8Encoding.UTF8;
            mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mailMessage);
        }

        private static string[] getLinesMatchPattern(string filePath, string pattern)
        {

            IEnumerable<string> lines = File.ReadAllLines(filePath);

            List<string> matchedLines = new List<string>();
            foreach (var line in lines)
            {
                // make sure its not null doesn't start with an empty line or something.
                if (line != null && !string.IsNullOrEmpty(line) && line.Length > 0)
                {

                    // use regex to find some key in your case the "ID".
                    // look into regex and word boundry find only lines with ID
                    // double check the below regex below going off memory. \B is for boundry
                    var regex = new Regex(pattern);

                    var isMatch = regex.Match(line);
                    if (isMatch.Success)
                    {

                        // matchedLines.Add(line.Substring(line.IndexOf(pattern)));
                        matchedLines.Add(line);
                    }

                }
            }

            return matchedLines.ToArray();

        }
    }
}

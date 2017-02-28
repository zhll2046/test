using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Utils
{
   
 public   class MailUtil
    {
        public void sendmail(string mailSubject, string mailBody)
        {
            try
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
                mailMessage.IsBodyHtml = true;

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
            catch (Exception ex)
            {

                throw ex;

            }
        }
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using Microsoft.Exchange.WebServices.Data;
using System.Net;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Exchange101
{ 
    public class EmailMessageExt
    { 
        public string threadCreatedDateTime { get; set; } 
        public string threadUrl { get; set; } 
        public string question { get; set; }


        public EmailMessage emessage { get; set; }

        public EmailMessageExt()
        {

        }

        public EmailMessageExt(EmailMessage emessage, string threadCreatedDateTime, string threadUrl, string question)
        {
            this.emessage = emessage;
            this.threadCreatedDateTime = threadCreatedDateTime;
            this.threadUrl = threadUrl;
            this.question = question;
        }
    }

    public class EmailMessageExts
    {
        public Collection<EmailMessageExt> emailMessageExts { get; set; }

        public EmailMessageExts()
        {
            emailMessageExts = new Collection<EmailMessageExt>();
        }
    }

    public class MailConversations
    {
        public Dictionary<string, List<EmailMessage>> conversations { get; set; }

        public MailConversations()
        {

            conversations = new Dictionary<string, List<EmailMessage>>();

        }


    }
}

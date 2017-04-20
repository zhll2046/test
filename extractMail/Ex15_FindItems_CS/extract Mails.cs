using System;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;
using System.IO;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using System.Linq;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using Utils;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Exchange101
{ 
    class Ex15_FindItems
    { 

        // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization. 
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string MailAddress = "v-lvzhan@microsoft.com";
        private static string password = "Sjynige@b";
        private static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(MailAddress, password), new TraceListener());
        private static string subFolderName = ConfigurationManager.AppSettings["mailFolderName"];
        private static string matchTableHeader = ConfigurationManager.AppSettings["tableHeader"]; 



        // Note that for this sample, the ExchangeVersion is hard-coded in UserData.cs to ExchangeVersion.Exchange2013.  
        static void Main(string[] args)
        {


            FindItemsResults<Item> results = FindItems(service);

            if (results.TotalCount > 0)
            {

                MailConversations mc = extractItems(results);

                if (mc != null)
                {

                    EmailMessageExts emes = ParseMailConversations(mc);

                    if (emes != null)
                    { 

                        //Console.WriteLine(Utils.XmlObjectUtil.GetXMLFromObject(emes.emailMessageExts[0]));

                        DBUtil.insertIntoDB(emes); 
                        /** foreach (EmailMessageExt eme in emes.emailMessageExts) {

                             Console.WriteLine(eme.threadCreatedDateTime);
                             Console.WriteLine(eme.threadUrl);
                             Console.WriteLine(eme.question);

                         } **/
                    }

                }

            }

            try
            {

                /**
                MailUtil mu = new MailUtil();
                mu.sendmail("test","test"); 
                **/

            }

            catch (Exception ex)
            {

                log.Error("Error", ex);

            }

            //mu.sendmail("","");

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();

        }

        private static void saveMailConversationInDB(EmailMessageExts emes)
        {
            throw new NotImplementedException();
        }

        private static EmailMessageExts ParseMailConversations(MailConversations mc)
        {

            EmailMessageExts emes = new EmailMessageExts();

            log.Info("extracting mail information....");

            foreach (string key in mc.conversations.Keys)
            {


                foreach (EmailMessage mm in mc.conversations[key])
                {

                    HtmlDocument doc = new HtmlDocument();

                    doc.LoadHtml(mm.Body);

                    EmailMessageExt eme;


                    HtmlNodeCollection allTablesInOneMail = doc.DocumentNode.SelectNodes("//table");

                    //find the last table in the mail chain
                    //logically the table is in the first mail, so it usually at the bottom a mail html
                    if (allTablesInOneMail != null) {

                        if (allTablesInOneMail.Count > 0)
                        {

                            HtmlNode table = allTablesInOneMail[allTablesInOneMail.Count - 1];
                            HtmlNodeCollection rows = table.SelectNodes("tbody/tr");

                            if (rows[0].InnerText.Replace(" ", "").Trim() == matchTableHeader.Replace(" ", "").Trim())
                            {

                                //row[0] = "Power BI Social Media Escalation"
                                //row[1] = date
                                //row[2] = url
                                //row[3] = question body
                                eme = new EmailMessageExt(mm, rows[1].InnerText.Trim(), rows[2].InnerText.Trim(), rows[3].InnerText.Trim());

                            }
                            else
                            {
                                eme = new EmailMessageExt(mm, null, null, null);
                            }

                            emes.emailMessageExts.Add(eme);

                        }

                    }
                }

            }

            return emes.emailMessageExts.Count == 0 ? null : emes;
        }

        private static MailConversations extractItems(FindItemsResults<Item> findResults)
        {

            log.Info("searching social medial mails in folder" + subFolderName);

            MailConversations mc = new MailConversations();


            if (findResults.TotalCount > 0)
            {

                Dictionary<string, List<EmailMessage>> dict = mc.conversations;

                foreach (Item myItem in findResults.Items)
                {
                    //myItem.Load(); 
                    PropertySet propSet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, EmailMessageSchema.IsRead);
                    EmailMessage mail = EmailMessage.Bind(service, myItem.Id, propSet);
                    mail.Load();
                    //if the conversation is not in the dict, then create a new list and add to dict
                    if (!dict.ContainsKey(mail.ConversationId))
                    {
                        List<EmailMessage> list = new List<EmailMessage>();
                        list.Add(mail);
                        dict.Add(mail.ConversationId, list);
                    }
                    //if the conversation is already in the dict, then get it and add mail to the list
                    else
                    {
                        dict[mail.ConversationId].Add(mail);
                    }
                }

                /** 
                StreamWriter sw = new StreamWriter(@"c:\test\test\2.html"); 
                foreach (string key in dict.Keys)
                {
                    //sort the mail of each conversation to find out the latest reply
                    dict[key].OrderBy(o => string.Join("", o.ConversationIndex)).ToList(); 
                    foreach (EmailMessage mail in dict[key])
                    {
                        sw.WriteLine(mail.Body);
                        sw.WriteLine("===============================" + mail.ConversationId + "==========" + mail.DateTimeReceived + "====================" + string.Join("", mail.ConversationIndex) + "==================");
                                    } 
                    //sw.WriteLine("===============================" + dict[key][0].ConversationId + "==========" + dict[key][0].DateTimeReceived + "====================" + string.Join("", dict[key][0].ConversationIndex) + "==================");
                 
                }

                sw.Close();
                
                 * **/
                return mc;
            }
            //if no result was found
            else
            {
                return null;
            }
        }

        private static FindItemsResults<Item> FindItems(ExchangeService service)
        {
            log.Info("connected to " + MailAddress);
            // Create a search collection that contains your search conditions.
            List<SearchFilter> searchFilterCollection = new List<SearchFilter>();
            searchFilterCollection.Add(new SearchFilter.ContainsSubstring(ItemSchema.Subject, ConfigurationManager.AppSettings["mailSubjectFilter"]));

            DateTime dt1 = DateTime.Now;
            TimeSpan Days = new TimeSpan(Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeReceived"]), 0, 0, 0);
            DateTime filterDate = dt1 - Days;

            searchFilterCollection.Add(new SearchFilter.IsGreaterThanOrEqualTo(ItemSchema.DateTimeReceived, filterDate));

            // Create the search filter with a logical operator and your search parameters.
            SearchFilter searchFilter = new SearchFilter.SearchFilterCollection(LogicalOperator.And, searchFilterCollection.ToArray());

            // Limit the view to 50 items.
            ItemView view = new ItemView(50);

            // Limit the property set to the property ID for the base property set, and the subject and item class for the additional properties to retrieve.
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.ItemClass);

            // Setting the traversal to shallow will return all non-soft-deleted items in the specified folder.
            view.Traversal = ItemTraversal.Shallow;

            FolderView fview = new FolderView(1000);
            fview.PropertySet = new PropertySet(BasePropertySet.IdOnly);
            fview.PropertySet.Add(FolderSchema.DisplayName);
            fview.Traversal = FolderTraversal.Deep;

            FindFoldersResults findFolderResults = service.FindFolders(WellKnownFolderName.Inbox, fview);


            FolderId folderid = null;

            foreach (Folder f in findFolderResults)
            {
                //show folderId of the folder "test"
                if (f.DisplayName == subFolderName)
                {
                    folderid = f.Id;
                    break;
                }
            }

            // Send the request to search the Inbox and get the results.
            FindItemsResults<Item> findResults = service.FindItems(folderid, searchFilter, view);

            return findResults;

        }
    }
     
}

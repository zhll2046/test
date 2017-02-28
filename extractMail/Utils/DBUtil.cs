using Exchange101;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class DBUtil
    {

        public static void insertIntoDB(EmailMessageExts emes) { 

            string connetionString = null;
            string sql = null;
            connetionString = ConfigurationManager.AppSettings["connectionString"];

           

            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                sql = "INSERT INTO mailConversation(conversationID, conversationIndex, conversationTopic, CcRecipients,   [From], ReceivedBy, ReplyTo, sender, [subject], body, ThreadUrl, question, ThreadCreatedDateTime, size, DateTimeCreated, DateTimeReceived, DateTimeSent)"+
                        "VALUES(@conversationID, @conversationIndex, @conversationTopic, @CcRecipients, @From, @ReceivedBy, @ReplyTo, @sender,@subject,@body,@ThreadUrl,@question,@ThreadCreatedDateTime,@size,@DateTimeCreated,@DateTimeReceived,@DateTimeSent); ";
                cnn.Open();
                 

                    foreach (EmailMessageExt eme in emes.emailMessageExts) {
                    SqlCommand cmd = new SqlCommand(sql, cnn);

                        cmd.Parameters.AddWithValue("@conversationID", eme.emessage.ConversationId.UniqueId);
                        cmd.Parameters.AddWithValue("@conversationIndex", string.Join("", eme.emessage.ConversationIndex));
                        cmd.Parameters.AddWithValue("@conversationTopic", eme.emessage.ConversationTopic);
                        cmd.Parameters.AddWithValue("@CcRecipients", string.Join(";", eme.emessage.CcRecipients.Select(x => x.Address).ToArray()));
                        cmd.Parameters.AddWithValue("@From", eme.emessage.From.Address);
                        cmd.Parameters.AddWithValue("@ReceivedBy", string.Join(";", eme.emessage.ToRecipients.Select(x => x.Address).ToArray()));
                        cmd.Parameters.AddWithValue("@ReplyTo", string.Join(";", eme.emessage.ReplyTo.Select(x => x.Address).ToArray()));
                        cmd.Parameters.AddWithValue("@sender", eme.emessage.Sender.Address);
                        cmd.Parameters.AddWithValue("@subject", eme.emessage.Subject);
                        cmd.Parameters.AddWithValue("@body", eme.emessage.Body.Text.ToString());
                        cmd.Parameters.AddWithValue("@ThreadUrl", eme.threadUrl??"");
                        cmd.Parameters.AddWithValue("@question", eme.question??"");
                        cmd.Parameters.AddWithValue("@ThreadCreatedDateTime", eme.threadCreatedDateTime??"");
                        cmd.Parameters.AddWithValue("@size", eme.emessage.Size);
                        cmd.Parameters.AddWithValue("@DateTimeCreated", eme.emessage.DateTimeCreated);
                        cmd.Parameters.AddWithValue("@DateTimeReceived", eme.emessage.DateTimeReceived);
                        cmd.Parameters.AddWithValue("@DateTimeSent", eme.emessage.DateTimeSent);
                        //cmd.Parameters.AddWithValue("@mailArchive", null);
                    cmd.ExecuteNonQuery();

                }

                SqlCommand cmd2 = new SqlCommand("UpdateSocialMediaCases", cnn);
                cmd2.CommandType = CommandType.StoredProcedure;
                  
                cmd2.ExecuteNonQuery(); 



                cnn.Close();
                      
                }
            }

        }

    }




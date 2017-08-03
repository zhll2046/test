using System;
using System.Net;
//Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory -Version 2.21.301221612
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Text;
//Install-Package Newtonsoft.Json 
using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Net.Http;
using System.Collections.Specialized;

namespace ConsoleApplication39
{

    class Program
    {

        //Step 1 - Replace {client id} with your client app ID. 
        //To learn how to get a client app ID, see Register a client app (https://blogs.technet.microsoft.com/stefan_stranger/2016/10/21/using-the-azure-arm-rest-apin-get-access-token/)  

        private static string clientID = "99a17e8c-e579-4d36-a3db-291eb95fcb14";
        //RedirectUri you used when you registered your app.
        //For a client app, a redirect uri gives AAD more details on the specific application that it will authenticate.
        private static string redirectUri = "https://login.live.com/oauth20_desktop.srf";

        //Resource Uri for Power BI API
        private static string resourceUri = "https://management.core.windows.net/";

        //OAuth2 authority Uri
        private static string authority = "https://login.windows.net/common/oauth2/authorize";

        private static string subscriptionID = "7df9301f-26a4-44fa-a328-87f0e7bc3b36";
        private static string resourceGroups = "EricTest";
        private static string workspaceCollectionName = "testPbiEmb2";


        //the account used to login your Azure subscription when use getAccessTokenSilently()
        private static string username = "v-lvzhan@microsoft.com";
        private static string password = "Sjynige#b";

        private static AuthenticationContext authContext = null;
        private static string token = String.Empty;


        static void Main(string[] args)
        {

            //token = getAccessTokenSilently();
            token = getAccessTokenWithLoginPopUp();

            getPBIworkspaceCollectionBilling(subscriptionID, resourceGroups, workspaceCollectionName);

            Console.ReadKey();

        }


        static void getPBIworkspaceCollectionBilling(string subscriptionID, string resourceGroups, string workspaceCollectionName )
        {

            HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp(String.Format(" https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.PowerBI/workspaceCollections/{2}/billingUsage?api-version=2016-01-29", subscriptionID, resourceGroups, workspaceCollectionName));
            //POST web request to create a datasource.
            request.KeepAlive = true;
            request.Method = "GET";
            request.ContentLength = 0;

            //Add token to the request header
            request.Headers.Add("Authorization", String.Format("Bearer {0}", token));

            //Write JSON byte[] into a Stream
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {

                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    dynamic responseJson = JsonConvert.DeserializeObject<dynamic>(responseString);
                    var billinginfo = responseJson.value;
                     
                        /** response body
                        {
                        "value": [
                                 {
                                "workspaceCollectionName": "testPbiEmb2",
                                "totalUsage": 2,
                                "meterType": "Sessions"
                                  }
                                ]
                                } 
                        **/
                        Console.WriteLine("workspaceCollection {0} already consumed {1} {2}", billinginfo[0]["workspaceCollectionName"].ToString(), billinginfo[0]["totalUsage"].ToString(), billinginfo[0]["meterType"].ToString());
                        Console.WriteLine("");
                    
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string errorString = reader.ReadToEnd();
                            dynamic respJson = JsonConvert.DeserializeObject<dynamic>(errorString);
                            Console.WriteLine(respJson["error"]["message"]); 
                            //TODO: use JSON.net to parse this string and look at the error message
                        }
                    }
                }
            }  
        }




        static string getAccessTokenSilently()
        {

            HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp("https://login.windows.net/common/oauth2/token");
            //POST web request to create a datasource.
            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentLength = 0;
            request.ContentType = "application/x-www-form-urlencoded";

            //Add token to the request header
            request.Headers.Add("Authorization", String.Format("Bearer {0}", token));

            NameValueCollection parsedQueryString = HttpUtility.ParseQueryString(String.Empty);
            parsedQueryString.Add("client_id", clientID);
            parsedQueryString.Add("grant_type", "password");
            parsedQueryString.Add("resource", resourceUri);
            parsedQueryString.Add("username", username);
            parsedQueryString.Add("password", password);
            string postdata = parsedQueryString.ToString();


            //POST web request
            byte[] dataByteArray = System.Text.Encoding.ASCII.GetBytes(postdata); ;
            request.ContentLength = dataByteArray.Length;


            try
            {
                //Write JSON byte[] into a Stream
                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(dataByteArray, 0, dataByteArray.Length);
                    var response = (HttpWebResponse)request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    dynamic responseJson = JsonConvert.DeserializeObject<dynamic>(responseString);
                    return responseJson["access_token"];
                }

            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string errorString = reader.ReadToEnd();
                            dynamic respJson = JsonConvert.DeserializeObject<dynamic>(errorString);
                            Console.WriteLine(respJson["error_description"]);

                            //TODO: use JSON.net to parse this string and look at the error message
                        }
                    }
                }
            }
            return null;


        }


        static string getAccessTokenWithLoginPopUp()
        {
            if (token == String.Empty)
            {
                //Get Azure access token
                // Create an instance of TokenCache to cache the access token
                TokenCache TC = new TokenCache();
                // Create an instance of AuthenticationContext to acquire an Azure access token
                authContext = new AuthenticationContext(authority, TC);
                // Call AcquireToken to get an Azure token from Azure Active Directory token issuance endpoint
                token = authContext.AcquireToken(resourceUri, clientID, new Uri(redirectUri), PromptBehavior.RefreshSession).AccessToken;
            }
            else
            {
                // Get the token in the cache
                token = authContext.AcquireTokenSilent(resourceUri, clientID).AccessToken;
            }

            return token;
        }
    }
}

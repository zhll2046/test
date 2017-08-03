using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication32
{
    class Program
    {
        static void Main(string[] args)
        {
            string token = GetToken();
            Console.WriteLine(token);
            Console.ReadLine();

        }

        #region Get an authentication access token
        private static string GetToken()
        {  
            //The client id that Azure AD created when you registered your client app.
            string clientID = "d9146xxxxxx946e8";

            //RedirectUri you used when you register your app.
            //For a client app, a redirect uri gives Azure AD more details on the application that it will authenticate.
            // You can use this redirect uri for your client app
            string redirectUri = "https://login.live.com/oauth20_desktop.srf";

            //Resource Uri for Power BI API
            string resourceUri = "https://analysis.windows.net/powerbi/api";

            //OAuth2 authority Uri
            string authorityUri = "https://login.windows.net/common/oauth2/authorize";
 
            // Call AcquireToken to get an Azure token from Azure Active Directory token issuance endpoint
            AuthenticationContext authContext = new AuthenticationContext(authorityUri);

            //string token = authContext.AcquireToken(resourceUri, clientID, new Uri(redirectUri)).AccessToken;
            authContext.acqui
            return authContext.AcquireTokenAsync(resourceUri, clientID, new Uri(redirectUri), new PlatformParameters(0)).Result.AccessToken;
             

        }

        #endregion
    }
}
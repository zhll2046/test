using System;using System.Net;using System.IO;
namespace PBIGettingStarted{    class Program    {
        //Access key for app token
        private static string accessKey = "yi6Ow5Wqt7uboGx9B8wGYTID9RQUI1uDexJLZLblJ7vax5uBo4WbKgnKx3kZBZkKNb/Q3Lz02q7B7GY6HLPOqA==";
        //Power BI app token values
        private static string workspaceCollectionName = "wrkspcCllctn4PBI";        private static string workspaceId = "b7cf433b-437c-4ca5-9df2-d9fd82182461";
        private static string pbixFileName = "IamApbix.pbix";
        static void Main(string[] args)        {
            //Imports uri
            var uri = String.Format
("https://api.powerbi.com/v1.0/collections/{0}/workspaces/{1}/imports?datasetDisplayName=SampleImport",
workspaceCollectionName, workspaceId);
            //PBIX file to import             string fileName = @"C:\test\kpi.pbix";

            //Create web request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            //Import Request format:
            // Header:
            //  Content - Type: multipart / form - data; ----------BOUNDARY
            //  Authorization: AppToken
            // Body:
            //  ----------BOUNDARY
            //  Content - Disposition: form - data; filename = "{pbix file}.pbix"
            //  Content - Type: application / octet – stream

            //Define POST
            request.Method = "POST";            request.UseDefaultCredentials = true;
            //Header
            // Boundary
            string boundary = "----------BOUNDARY";            byte[] boundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            // Content - Type
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            // Authorization - Use AppToken jwt for Authorization header
            request.Headers.Add("Authorization", String.Format("AppKey {0}", accessKey));
            //Body
            string bodyTemplate = "Content-Disposition: form-data; filename=\"{0}\"\r\nContent-Type: application / octet - stream\r\n\r\n";            string body = string.Format(bodyTemplate, fileName);            byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(body);
            //Get request stream 
            using (Stream rs = request.GetRequestStream())            {                rs.Write(boundaryBytes, 0, boundaryBytes.Length);                rs.Write(bodyBytes, 0, bodyBytes.Length);
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))                {                    byte[] buffer = new byte[4096];                    int bytesRead = 0;                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)                    {                        rs.Write(buffer, 0, bytesRead);                    }                }
                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");                rs.Write(trailer, 0, trailer.Length);            }
            //Get response
            using (HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse)            {
                //If Import succeeds, StatusCode = Accepted 
                var responseStatusCode = response.StatusCode.ToString();            }        }    }

}
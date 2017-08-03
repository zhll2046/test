﻿using System;
namespace PBIGettingStarted
        //Access key for app token
        private static string accessKey = "yi6Ow5Wqt7uboGx9B8wGYTID9RQUI1uDexJLZLblJ7vax5uBo4WbKgnKx3kZBZkKNb/Q3Lz02q7B7GY6HLPOqA==";
        //Power BI app token values
        private static string workspaceCollectionName = "wrkspcCllctn4PBI";
        private static string pbixFileName = "asf.pbix";
        static void Main(string[] args)
            //Imports uri
            var uri = String.Format
("https://api.powerbi.com/v1.0/collections/{0}/workspaces/{1}/imports?datasetDisplayName=SampleImport",
workspaceCollectionName, workspaceId);
            //PBIX file to import
            DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

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
            request.Method = "POST";
            //Header
            // Boundary
            string boundary = "----------BOUNDARY";
            // Content - Type
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            // Authorization - Use AppToken jwt for Authorization header
            request.Headers.Add("Authorization", String.Format("AppKey {0}", accessKey));
            //Body
            string bodyTemplate = "Content-Disposition: form-data; filename=\"{0}\"\r\nContent-Type: application / octet - stream\r\n\r\n";
            //Get request stream 
            using (Stream rs = request.GetRequestStream())
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            //Get response
            using (HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse)
                //If Import succeeds, StatusCode = Accepted 
                var responseStatusCode = response.StatusCode.ToString();

}
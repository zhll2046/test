using System; 
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace walkthrough_push_data
{
    class Program
    {
        private static string token = string.Empty;

        static void Main(string[] args)
        {

            //Get an authentication access token
            token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Il9VZ3FYR190TUxkdVNKMVQ4Y2FIeFU3Y090YyIsImtpZCI6Il9VZ3FYR190TUxkdVNKMVQ4Y2FIeFU3Y090YyJ9.eyJhdWQiOiJodHRwczovL2FuYWx5c2lzLndpbmRvd3MubmV0L3Bvd2VyYmkvYXBpIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvNzJmOTg4YmYtODZmMS00MWFmLTkxYWItMmQ3Y2QwMTFkYjQ3LyIsImlhdCI6MTQ4ODQzNTgyMywibmJmIjoxNDg4NDM1ODIzLCJleHAiOjE0ODg0Mzk3MjMsImFjciI6IjEiLCJhaW8iOiJOQSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwaWQiOiI4NzFjMDEwZi01ZTYxLTRmYjEtODNhYy05ODYxMGE3ZTkxMTAiLCJhcHBpZGFjciI6IjIiLCJlX2V4cCI6MTA4MDAsImZhbWlseV9uYW1lIjoiWmhhbmciLCJnaXZlbl9uYW1lIjoiRXJpYyIsImluX2NvcnAiOiJ0cnVlIiwiaXBhZGRyIjoiMTY3LjIyMC4yNTUuNjAiLCJuYW1lIjoiRXJpYyBaaGFuZyAoU2hhbmcgSGFpIFdlaSBDaHVhbmcgUnVhbiBKaWFuKSIsIm9pZCI6IjQ3NjgwMTFhLTY2NjgtNGFlNC1hYmYwLTU3NDM2ZGIyODQ1MSIsIm9ucHJlbV9zaWQiOiJTLTEtNS0yMS0yMTQ2NzczMDg1LTkwMzM2MzI4NS03MTkzNDQ3MDctMTg4MDU0NSIsInBsYXRmIjoiMyIsInB1aWQiOiIxMDAzMDAwMDhDMjJBQkRFIiwic2NwIjoidXNlcl9pbXBlcnNvbmF0aW9uIiwic3ViIjoiNDQ2MVF0VDh6MGpTdHMxV29lZmVHRk5RMkhMQ1VKcWF6SDVuOEJLWnRVUSIsInRpZCI6IjcyZjk4OGJmLTg2ZjEtNDFhZi05MWFiLTJkN2NkMDExZGI0NyIsInVuaXF1ZV9uYW1lIjoidi1sdnpoYW5AbWljcm9zb2Z0LmNvbSIsInVwbiI6InYtbHZ6aGFuQG1pY3Jvc29mdC5jb20iLCJ2ZXIiOiIxLjAifQ.kEXP9_tuxmiX-uRhQTuYnIqJqpiw7RikL-ZXBjZSwuIuIw8Gd1kJtY5-KTjGiJQw3rLsapMuQ__zgEljMHqM3KYTCxtO3ffndcqbLNBu_uuo-L5WhogT2n0n34EW1dbmuKdjBG6xtx4QS2lX8_QIsJDwjGA4ulA6ojCF55wL-qHgZOHtvOXrXnBRm24nY8wSOQoWDwCmQ7MIUMoAuhrrdxYR-MnYmhi7LCmw3LWeMGOC8XG0Gw-uGqVhDfDojTHAdE00zPfMWVh0-ZznmIa68u-ieHukvDebsZH7ubY9oyLDcB03W5IM3e040M-rdEdcnNBJMGweMdg3wUDjFYrP9A";

            //Create a dataset in Power BI
            //CreateDataset();

            //Get a dataset to add rows into a Power BI table
            string datasetId = GetDataset();

            //Add rows to a Power BI table
            AddRows(datasetId, "Product");

        }

        #region Get an authentication access token
       

        #endregion

        #region Create a dataset in a Power BI
        private static void CreateDataset()
        {
            //TODO: Add using System.Net and using System.IO

            string powerBIDatasetsApiUrl = "https://api.powerbi.com/v1.0/myorg/datasets";
            //POST web request to create a dataset.
            //To create a Dataset in a group, use the Groups uri: https://api.PowerBI.com/v1.0/myorg/groups/{group_id}/datasets
            HttpWebRequest request = System.Net.WebRequest.Create(powerBIDatasetsApiUrl) as System.Net.HttpWebRequest;
            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentLength = 0;
            request.ContentType = "application/json";

            //Add token to the request header
            request.Headers.Add("Authorization", String.Format("Bearer {0}", token));

            //Create dataset JSON for POST request
            string datasetJson = "{\"name\": \"1111SalesMarketingForTest\", \"tables\": " +
                "[{\"name\": \"Product\", \"columns\": " +
                "[{ \"name\": \"ProductID\", \"dataType\": \"Int64\"}, " +
                "{ \"name\": \"Name\", \"dataType\": \"string\"}, " +
                "{ \"name\": \"Category\", \"dataType\": \"string\"}," +
                "{ \"name\": \"IsCompete\", \"dataType\": \"bool\"}," +
                "{ \"name\": \"ManufacturedOn\", \"dataType\": \"DateTime\"}" +
                "]}]}";

            //POST web request
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(datasetJson);
            request.ContentLength = byteArray.Length;

            //Write JSON byte[] into a Stream
            using (Stream writer = request.GetRequestStream())
            {
                writer.Write(byteArray, 0, byteArray.Length);

                var response = (HttpWebResponse)request.GetResponse();

                Console.WriteLine(string.Format("Dataset {0}", response.StatusCode.ToString()));

                Console.ReadLine();
            }
        }
        #endregion

        #region Get a dataset to add rows into a Power BI table
        private static string GetDataset()
        {
            string powerBIDatasetsApiUrl = "https://api.powerbi.com/v1.0/myorg/datasets";
            //POST web request to create a dataset.
            //To create a Dataset in a group, use the Groups uri: https://api.PowerBI.com/v1.0/myorg/groups/{group_id}/datasets
            HttpWebRequest request = System.Net.WebRequest.Create(powerBIDatasetsApiUrl) as System.Net.HttpWebRequest;
            request.KeepAlive = true;
            request.Method = "GET";
            request.ContentLength = 0;
            request.ContentType = "application/json";

            //Add token to the request header
            request.Headers.Add("Authorization", String.Format("Bearer {0}", token));

            string datasetId = string.Empty;
            //Get HttpWebResponse from GET request
            using (HttpWebResponse httpResponse = request.GetResponse() as System.Net.HttpWebResponse)
            {
                //Get StreamReader that holds the response stream
                using (StreamReader reader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                {
                    string responseContent = reader.ReadToEnd();

                    //TODO: Install NuGet Newtonsoft.Json package: Install-Package Newtonsoft.Json
                    //and add using Newtonsoft.Json
                    var results = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    //Get the first id
                    

                    foreach (var result in results["value"]) {

                        if (result["name"] == "1111SalesMarketingForTest") {

                            datasetId = result["id"];

                        }


                    }




                    Console.WriteLine(String.Format("Dataset ID: {0}", datasetId));
                    Console.ReadLine();

                    return datasetId;
                }
            }
        }
        #endregion

        #region Add rows to a Power BI table
        private static void AddRows(string datasetId, string tableName)
        {
            string powerBIApiAddRowsUrl = String.Format("https://api.powerbi.com/v1.0/myorg/datasets/{0}/tables/{1}/rows", datasetId, tableName);

            //POST web request to add rows.
            //To add rows to a dataset in a group, use the Groups uri: https://api.powerbi.com/v1.0/myorg/groups/{group_id}/datasets/{dataset_id}/tables/{table_name}/rows
            //Change request method to "POST"
            HttpWebRequest request = System.Net.WebRequest.Create(powerBIApiAddRowsUrl) as System.Net.HttpWebRequest;
            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentLength = 0;
            request.ContentType = "application/json";

            //Add token to the request header
            request.Headers.Add("Authorization", String.Format("Bearer {0}", token));

            //JSON content for product row
            string rowsJson = "{\"rows\":" +
                "[{\"ProductID\":4,\"Name\":\"Adjustable Race 5\",\"Category\":\"Components\",\"IsCompete\":true,\"ManufacturedOn\":\"07/30/2014\"}," +
                "{\"ProductID\":8,\"Name\":\"LL Crankarm test  888\",\"Category\":\"Components\",\"IsCompete\":true,\"ManufacturedOn\":\"07/30/2014\"}," +
                "{\"ProductID\":6,\"Name\":\"HL Mountain Frame - Silver\",\"Category\":\"Bikes\",\"IsCompete\":true,\"ManufacturedOn\":\"07/30/2014\"}]}";

            //POST web request
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(rowsJson);
            request.ContentLength = byteArray.Length;

            //Write JSON byte[] into a Stream
            using (Stream writer = request.GetRequestStream())
            {
                writer.Write(byteArray, 0, byteArray.Length);

                var response = (HttpWebResponse)request.GetResponse();

                Console.WriteLine("Rows Added");

                Console.ReadLine();
            }
        }

        #endregion
    }
}
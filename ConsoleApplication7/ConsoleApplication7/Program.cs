using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication7
{
    class Program
    {
        static string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImEzUU4wQlpTN3M0bk4tQmRyamJGMFlfTGRNTSIsImtpZCI6ImEzUU4wQlpTN3M0bk4tQmRyamJGMFlfTGRNTSJ9.eyJhdWQiOiJodHRwczovL2FuYWx5c2lzLndpbmRvd3MubmV0L3Bvd2VyYmkvYXBpIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvNzJmOTg4YmYtODZmMS00MWFmLTkxYWItMmQ3Y2QwMTFkYjQ3LyIsImlhdCI6MTQ5MjQ5NDU1OSwibmJmIjoxNDkyNDk0NTU5LCJleHAiOjE0OTI0OTg0NTksImFjciI6IjEiLCJhaW8iOiJBU1FBMi84REFBQUFIUDNWQW9jSnFkMWF1TGVqM3RySGJ3MTJRdTVVRE90NngyRTRQSWFxSDVjPSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwaWQiOiI4NzFjMDEwZi01ZTYxLTRmYjEtODNhYy05ODYxMGE3ZTkxMTAiLCJhcHBpZGFjciI6IjIiLCJlX2V4cCI6MjYyODAwLCJmYW1pbHlfbmFtZSI6IlpoYW5nIiwiZ2l2ZW5fbmFtZSI6IkVyaWMiLCJpbl9jb3JwIjoidHJ1ZSIsImlwYWRkciI6IjE2Ny4yMjAuMjU1LjEwIiwibmFtZSI6IkVyaWMgWmhhbmcgKFNoYW5nIEhhaSBXZWkgQ2h1YW5nIFJ1YW4gSmlhbikiLCJvaWQiOiI0NzY4MDExYS02NjY4LTRhZTQtYWJmMC01NzQzNmRiMjg0NTEiLCJvbnByZW1fc2lkIjoiUy0xLTUtMjEtMjE0Njc3MzA4NS05MDMzNjMyODUtNzE5MzQ0NzA3LTE4ODA1NDUiLCJwbGF0ZiI6IjMiLCJwdWlkIjoiMTAwMzAwMDA4QzIyQUJERSIsInNjcCI6InVzZXJfaW1wZXJzb25hdGlvbiIsInN1YiI6IjQ0NjFRdFQ4ejBqU3RzMVdvZWZlR0ZOUTJITENVSnFhekg1bjhCS1p0VVEiLCJ0aWQiOiI3MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDciLCJ1bmlxdWVfbmFtZSI6InYtbHZ6aGFuQG1pY3Jvc29mdC5jb20iLCJ1cG4iOiJ2LWx2emhhbkBtaWNyb3NvZnQuY29tIiwidmVyIjoiMS4wIn0.dv1hfahDKO5HE4cJcrMVmpVfx1s59zrevNwBrI8-21Elh09sttkRRc3brBI1m7HncDYP20qyqql93qhrPJLV1x1D5TzYdsuACi7W5odbIFzHL-grh3maQkjxbUvf_2q_NVh7TF39CHolMHUomvf3bwTBdn9tzzgdzVsm0Y4BjTHOgiKmuRm26ypVVBW3fhSJdUdmiWleGj5X7ykN4ApBCel9v8Qvad68ujIcv695yc6KYs-LG2_rhgAE8hWeMXQQ2taJa7k6gOyQrZF7d7dA_kkrdg28Ohviq13xR6PQaEVvbr27i0fmtKVYDkPAQMoSbPncUktEPPV9vWCIru8-GgSS";
        static void Main(string[] args)
        {
            createDataset();
        }




        public static string createDataset()
        {
            string rs = "";
            HttpWebRequest request = null;
            //string datasetJson = null;
            //Push data into a Power BI dashboard
          
                string powerBIDatasetsApiUrl = "https://api.powerbi.com/v1.0/myorg/datasets";
                //POST web request to create a dataset.
                //To create a Dataset in a group, use the Groups uri: https://api.PowerBI.com/v1.0/myorg/groups/{group_id}/datasets

                request = System.Net.WebRequest.Create(powerBIDatasetsApiUrl) as System.Net.HttpWebRequest;
                request.KeepAlive = true;
                request.Method = "POST";

                request.ContentType = "application/json";

                if (String.IsNullOrEmpty(token))
                {
                    rs = "token is null. " + "<br/>";
                    return rs;
                }

                //Add token to the request header
                request.Headers.Add("Authorization", String.Format("Bearer {0}", token));

                //Create dataset JSON for POST request
                string datasetJson = "{\"name\": \"CorporationProduct\", \"tables\": " +
                        "[{\"name\": \"Product\", \"columns\": " +
                        "[{ \"name\": \"ProductID\", \"dataType\": \"Int64\"}, " +
                        "{ \"name\": \"Name\", \"dataType\": \"string\"}, " +
                        "{ \"name\": \"Category\", \"dataType\": \"string\"}," +
                        "{ \"name\": \"IsCompete\", \"dataType\": \"bool\"}," +
                        "{ \"name\": \"ManufacturedOn\", \"dataType\": \"DateTime\"}" +
                        "]}]}";
                request.ContentLength = datasetJson.Length;
                //POST web request
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(datasetJson);
                request.ContentLength = byteArray.Length;

                //Write JSON byte[] into a Stream
                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                    writer.Close();
                    var response = (HttpWebResponse)request.GetResponse(); // Error: The remote server returned an error: (403) Forbidden.
                    rs += string.Format("Dataset {0}", response.StatusCode.ToString()) + "<br/>";
                    rs += "createDataset operation is successfull." + "<br/>";
                } 
             
            return rs;
        }
    }



}

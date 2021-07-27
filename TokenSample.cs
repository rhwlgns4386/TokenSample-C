using Newtonsoft.Json.Linq;
using System.Net;
using System;
using System.IO;
using System.Web;
using AES256;


namespace TokenSample
{
    class TokenSample
    {
        public static string key = "137fc3717f6449fe96889a42aeffda88";
        public static string clientID = "448844e7ad85b3d489b49aef796d19a89ea7bc5ce2b99f437976b1c8d40ce742";
        public static string MAC_address = "B8-12-65-E8-E7-D4";
        
        public static string refreshToken="6df816aac969043fe860ffd03df79a8ca63bfc315c7024418e59ddb466ea08f7";

        public static string accessToken="370d4a87dd5d0f680fcb49b705f66b1c8f716a3d947cf17fe634623777d75da8";


        public static void generateToken()
        {
            string date = DateTime.Now.ToString("yyyyMMddHHmmss");
            string responseText = string.Empty;

            var json = new JObject();
            json.Add("mac_address", MAC_address);
            json.Add("datetime", date);

            AES256Util aES256Util = new AES256Util();

            string encrypted_txt=HttpUtility.UrlEncode( aES256Util.encrypt(json.ToString(), key));

            string target_URL = "https://apigateway.kisti.re.kr/tokenrequest.do?accounts=" + encrypted_txt + "&client_id=" + clientID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(target_URL);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode statusCode = response.StatusCode;
                Console.WriteLine(statusCode);

                Stream respStream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                }
            }
            JObject responseJson=JObject.Parse(responseText);
            refreshToken=responseJson["refresh_token"].ToString();
            accessToken=responseJson["access_token"].ToString();
            Console.WriteLine("access_token : " +accessToken);
            Console.WriteLine("refresh_token : "+refreshToken);
        }
        
        public static void regenerateToken(){
            string target_URL = "https://apigateway.kisti.re.kr/tokenrequest.do?client_id="+clientID+"&refreshToken="+refreshToken;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(target_URL);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode statusCode = response.StatusCode;
                Console.WriteLine(statusCode);

                Stream respStream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                }
            }
            JObject responseJson=JObject.Parse(responseText);
            accessToken=responseJson["access_token"].ToString();
            Console.WriteLine("access_token : " +accessToken);
        }
    }
}
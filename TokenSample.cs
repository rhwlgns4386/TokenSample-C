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
        /**============================ 사용자 정보 입력 ==========================================*/
                //맥주소 입력해주세요.
        public static string MAC_address = "Your Mac Address";
        //발급받은 client_id를 입력해주세요.
        public static string clientID = "Your clientId";
        //API Gateway로부터 발급받은 인증키를 입력해주세요.
        public static string key = "Your key";

        //refresh Token을 입력해주세요.(Access Token 재발급시 입력)
        public static string refreshToken = "Your RefreshToken";
        //accessToken을 입력해주세요(데이터요청시 이용)
        public static string accessToken = "Your AccessToken";
        /**====================================== ==========================================*/


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

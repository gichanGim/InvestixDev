using InvestixDev.Repository.DB;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace InvestixDev.Repository.KIS
{
    public class KISAuthenticator
    {
        private static string appName = AppDomain.CurrentDomain.FriendlyName;
        private static string appNameWithoutExtension = Path.GetFileNameWithoutExtension(appName);
        private static Dictionary<string, string> _baseHeaders;
        private static DB_User _dbuser;
        private static DateTime _lastAuthTime; // 마지막으로 토큰 발급 받은 시간

        private static string folderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            appNameWithoutExtension
        );

        private static string tokenTmp = folderPath + "KIS" + DateTime.Now.ToString("yyyyMMdd"); // token 저장 경로

        public KISAuthenticator(DB_User dbuser)
        {
            _dbuser = dbuser;
        }

        public async Task<string> GetToken(string userName)
        {
            if (!File.Exists(tokenTmp))
                File.Create(tokenTmp).Close();
            string? token = await Auth(userName);

            return token;
        }

        public static async Task doAuth(string userName)
        {
            if (!File.Exists(tokenTmp))
            {
                File.Create(tokenTmp).Close();
            }

            _baseHeaders = new Dictionary<string, string> // API 요청 헤더 설정
                {
                    { "Content-Type", "application/json" },
                    { "Accept", "text/plain" },
                    { "charset", "UTF-8" },
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36" }
            };

            await Auth(userName);
        }

        // tokenTmp경로에 txt파일에 발급 받은 Token, 만료 기간 저장
        private static void SaveToken(string myToken, string myExpired)
        {
            Console.WriteLine("===== Exp " + myExpired);

            using (var writer = new StreamWriter(tokenTmp, false, Encoding.UTF8))
            {
                writer.WriteLine($"token; {myToken}");
                writer.WriteLine($"valid-date; {myExpired}");
            }
        }

        private static string? ReadToken()
        {
            try
            {
                var lines = File.ReadAllLines(tokenTmp, Encoding.UTF8);
                var tokenData = lines.Select(line => line.Split(';')).ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());

                string expDt = DateTime.Parse(tokenData["valid-date"]).ToString("yyyy-MM-dd HH:mm:ss");
                string nowDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (string.Compare(expDt, nowDt) > 0) // 토큰 만료일 지났는지 확인
                    return tokenData["token"];
                
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>Token</returns>
        private static async Task<string?> Auth(string userName) // _mode (모의 or 실전), yaml 파일 내부 my_prod 
        {                                                                   // ( 01 : 국내/해외주식, 03 : 국내선물/옵션, 08 : 해외선물/옵션)
            var p = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };

            var APPKey = await _dbuser.GetUserAPIKeys(userName);

            string appkey = APPKey.AppKey;
            string appsec = APPKey.AppSecret;

            string? savedToken = ReadToken();

            if (savedToken == null)
            {
                try
                {
                    var client = new HttpClient();

                    var request = new HttpRequestMessage(HttpMethod.Post, "https://openapi.koreainvestment.com:9443/oauth2/tokenP");
                    var content = new StringContent($@"
                    {{
                        ""grant_type"": ""client_credentials"",
                        ""appkey"": ""{appkey}"",
                        ""appsecret"": ""{appsec}""
                    }}", Encoding.UTF8, "application/json");
                    request.Headers.Add("content-type", "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());
                        string myToken = jsonData["access_token"];
                        string myExpired = jsonData["access_token_token_expired"];
                        SaveToken(myToken, myExpired);
                        savedToken = ReadToken();
                        _lastAuthTime = DateTime.Now;

                        return savedToken;
                    }
                    else
                    {
                        Console.WriteLine("Get Authentication token fail!\nYou have to restart your app!!!");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"token 발급 요청 중 에러 발생 : {ex.Message}");
                    return null;
                }
            }
            else
            {
                _lastAuthTime = DateTime.Now;
                return savedToken;
            }
        }

        private static async void ReAuth(string userName)
        {
            await Auth(userName);
        }
    }
}

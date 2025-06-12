#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MessagingLib
{
    public class Agent
    {
        public string osPlatform;
        public string sdkVersion;

        public Agent()
        {
            osPlatform = Environment.OSVersion.Platform + " | " + Environment.Version;
            sdkVersion = "C#/1.0.2";
        }
    }
    public class Message
    {
        public string type;
        public string to;
        public string from;
        public string subject;
        public string text;
    }
    public class Response
    {
        public System.Net.HttpStatusCode StatusCode;
        public string ErrorCode;
        public string ErrorMessage;
        public JObject Data;
    }

    private JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    private string GetAuth(string apiKey, string apiSecret)
    {
        string salt = GetSalt();
        string dateStr = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        string data = dateStr + salt;

        return "HMAC-SHA256 apiKey=" + apiKey + ", date=" + dateStr + ", salt=" + salt + ", signature=" + GetSignature(apiKey, data, apiSecret);
    }

    private string GetSignature(string apiKey, string data, string apiSecret)
    {
        System.Security.Cryptography.HMACSHA256 sha256 = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(apiSecret));
        byte[] hashValue = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
        string hash = BitConverter.ToString(hashValue).Replace("-", "");
        return hash.ToLower();
    }

    private string GetSalt()
    {
        int length = 6;

        string s = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random r = new Random();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 1; i <= length; i++)
        {
            int idx = r.Next(0, 35);
            sb.Append(s.Substring(idx, 1));
        }
        return sb.ToString();
    }
    private string GetUrl(object path)
    {
        string url = Config.protocol + "://" + Config.domain;
        if (!string.IsNullOrEmpty(Config.prefix))
        {
            url += Config.prefix;
        }
        url += path;
        return url;
    }

    private Response Request(string path, string method, string data = null)
    {
        string auth = GetAuth(Config.apiKey, Config.apiSecret);

        try
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(GetUrl(path));
            req.Method = method; // POST or GET
            req.Headers.Add("Authorization", auth);
            // req.ContentType = "application/json; charset=utf-8"; // .NetFrameWork 호환성으로 인한 오류 발생 시 이 구문 사용. 아래 구문은 주석처리 
            req.Headers.Add("Content-type", "application/json; charset=utf-8");

            if (!string.IsNullOrEmpty(data))
            {
                using (var writer = new System.IO.StreamWriter(req.GetRequestStream()))
                {
                    writer.Write(data);
                    writer.Close();
                }
            }

            using (System.Net.WebResponse response = req.GetResponse())
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    var jsonResponseText = streamReader.ReadToEnd();
                    JObject jsonObj = JObject.Parse(jsonResponseText);
                    return new Response()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Data = jsonObj,
                        ErrorCode = null,
                        ErrorMessage = null
                    };
                }
            }
        }
        catch (System.Net.WebException ex)
        {
            using (System.IO.StreamReader streamReader = new System.IO.StreamReader(ex.Response.GetResponseStream()))
            {
                var jsonResponseText = streamReader.ReadToEnd();
                JObject jsonObj = JObject.Parse(jsonResponseText);
                string ErrorCode = jsonObj.SelectToken("errorCode").ToString();
                string ErrorMessage = jsonObj.SelectToken("errorMessage").ToString();
                System.Net.HttpWebResponse httpResp = (System.Net.HttpWebResponse)ex.Response;
                return new Response()
                {
                    StatusCode = httpResp.StatusCode,
                    Data = jsonObj,
                    ErrorCode = ErrorCode,
                    ErrorMessage = ErrorMessage
                };
            }
        }
        catch (Exception ex)
        {
            string ErrorCode = "Unknown Exception";
            string ErrorMessage = ex.Message;

            return new Response()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Data = null,
                ErrorCode = ErrorCode,
                ErrorMessage = ErrorMessage
            };
        }
    }

    public Response SendMessage(Message messagae)
    {
        return Request("/messages/v4/send", "POST", JsonConvert.SerializeObject(new { message = messagae }, Formatting.None, JsonSettings));
    }
}

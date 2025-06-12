using Azure;
using System.Numerics;

namespace InvestixDev.Repository.Message_SDK
{
    public class SendSMS
    {
        public static (int statusCode, string statusMessage) Send(string msg, string phone)
        {
            MessagingLib.Message message = new MessagingLib.Message()
            {
                to = phone,
                from = "01030756127",
                text = $"[Investix] 본인 확인을 위해 인증번호 {msg}를 인증란에 입력해주세요."
            };

            var lib = new MessagingLib();

            MessagingLib.Response response = lib.SendMessage(message);

            int statusCode = int.Parse(response.Data.SelectToken("statusCode").ToString());
            string statusMessage = response.Data.SelectToken("statusMessage")?.ToString();

            return (statusCode, statusMessage);
        }
    }
}

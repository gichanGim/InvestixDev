using System.ComponentModel.DataAnnotations;

namespace InvestixDev.ViewModels
{
    public class VerifyPhoneViewModel
    {
        [Phone(ErrorMessage = "전화번호 형식으로 입력해주세요")]
        public string Phone { get; set; }

        
        public bool IsVerified { get; set; } = false;

        public int StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public string Salt {  get; set; }
    }
}

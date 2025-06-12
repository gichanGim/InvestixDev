using System.ComponentModel.DataAnnotations;

namespace ViewMyAssetDev.ViewModels
{
    public class RegisterViewModel
    { 
        public int Id { get; set; } 

        [Required(ErrorMessage = "아이디를 입력해주세요")]
        [StringLength(8, MinimumLength = 3, ErrorMessage = "아이디는 3자 이상 8자 이하로 입력해주세요")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "전화번호를 입력해주세요")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "전화번호 형식으로 입력해주세요.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNum { get; set; }

        public string VerfiyPhoneSalt {  get; set; } // 번호 input에 입력된 인증 문자열

        [Required(ErrorMessage = "비밀번호를 입력해주세요")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "비밀번호는 8자 이상 15자 이하로 입력해주세요")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "비밀번호를 확인해주세요")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "비밀번호가 일치하지 않습니다")]
        [Display(Name = "비밀번호 확인")]
        public string ConfirmPassword {  get; set; }

        public bool IsIdAvailable { get; set; } 

        public bool IsPhoneVerified { get; set; }

        [Required(ErrorMessage = "APPKEY를 입력해주세요.")]
        public string AppKey { get; set; }

        [Required(ErrorMessage = "APPSECRET를 입력해주세요.")]
        public string AppSecret {  get; set; }
    }
}

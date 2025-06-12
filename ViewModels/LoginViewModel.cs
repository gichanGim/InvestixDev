using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ViewMyAssetDev.ViewModels
{
    public class LoginViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "아이디를 입력해주세요")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "비밀번호를 입력해주세요")]
        public string Password { get; set; }

        [Display(Name = "로그인 정보 기억하기")]
        public bool RememberMe { get; set; }
    }   
}

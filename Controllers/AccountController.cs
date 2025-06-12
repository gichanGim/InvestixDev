using InvestixDev.Models;
using InvestixDev.Repository.Message_SDK;
using InvestixDev.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewMyAssetDev.ViewModels;
using static MessagingLib;

namespace InvestixDev.Controllers
{
    // dbo.AspNetUsers의 UserName -> User.cs의 UserId
    // dbo.AspNetUsers의 Phone -> User.cs의 Phone
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.UserId);

                if (user != null) // 아이디에 해당하는 유저가 있는 경우
                {
                    if (await userManager.CheckPasswordAsync(user, model.Password)) // 비밀번호가 일치하는 경우
                    {
                        await signInManager.SignInAsync(user, model.RememberMe);
                        return RedirectToAction("Main", "Auth");
                    }
                    else
                    {
                        ModelState.AddModelError("", "올바르지 않은 비밀번호입니다");
                        return View(model);
                    }
                }

                else
                {
                    ModelState.AddModelError("", "가입되지 않은 아이디입니다");
                    return View(model);
                }
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsIdAvailable)
                {
                    if (model.IsPhoneVerified)
                    {
                        Users users = new Users()
                        {
                            UserId = model.UserId,
                            Phone = model.PhoneNum,
                            UserName = model.UserId,
                            AppKey = model.AppKey,
                            AppSecret = model.AppSecret
                        };

                        var result = await userManager.CreateAsync(users, model.Password);

                        if (result.Succeeded) // 회원가입 성공했을 경우
                        {
                            return RedirectToAction("Login");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "휴대번호 인증을 진행해주세요.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "가입 가능한 아이디로 다시 시도해주세요.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> CheckIdAvailable(string id)
        {
            var user = await userManager.FindByNameAsync(id);

            if (user == null) // 가입된 아이디 없는 경우
                return Json(true);
            
            else
                return Json(false);
        }

        // 번호 인증 문자 발송
        // CheckPhoneAvailable -> SendVerification (중복 여부 확인 -> 문자 전송)
        [HttpPost]
        public async Task<JsonResult> CheckPhoneAvailable(string phone)
        {
            var user = await userManager.Users.AnyAsync(u => u.Phone == phone);

            if (!user) // 가입된 휴대번호 없을 때
                return Json(true);
            else
                return Json(false);
        }

        [HttpPost]
        public async Task<JsonResult> SendVerification(string phone)
        {
            string salt = GenerateSalt();

            var (code, message) = SendSMS.Send(salt, phone);

            if (code != 4000 && code != 2000 && code != 3000) // 인증 메세지 전송 실패
                return Json(new { success = false, statusMessage = message, salt = "0"});
            else                            // 인증 메세지 전송 성공
                return Json(new { success = true, statusMessage = message, salt = salt });
        }

        [HttpPost]
        public async Task<JsonResult> VerifySalt(string salt, string savedSalt, bool isTimerExpired)
        {
            if (isTimerExpired)
                return Json(new { success = false, message = "인증 번호 유효시간이 만료되었습니다." });
            else
            {
                if (salt == savedSalt)
                    return Json(new { success = true, message = "인증 완료되었습니다." });
                else
                    return Json(new { success = false, message = "인증 번호와 일치하지 않습니다." });
            }
        }

        string GenerateSalt()
        {
            int length = 6;

            string s = "0123456789";
            Random r = new Random();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 1; i <= length; i++)
            {
                int idx = r.Next(0, s.Length);
                sb.Append(s.Substring(idx, 1));
            }
            return sb.ToString();
        }

    }
}

using InvestixDev.Models;
using InvestixDev.Repository;
using InvestixDev.Repository.DB;
using InvestixDev.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InvestixDev.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> _userManager;
        private readonly DB_Update _dbupdate;
        private readonly DB_Stocks _dbstocks;
        private readonly DB_User _dbuser; 
        public AuthController(SignInManager<Users> signInManager, UserManager<Users> userManager, DB_Update dbupdate, DB_Stocks dbstocks, DB_User dbuser)
        {
            this.signInManager = signInManager;
            _userManager = userManager;
            _dbupdate = dbupdate;
            _dbstocks = dbstocks;
            _dbuser = dbuser;
        }

        public async Task<IActionResult> Main()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "UnAuth");

            var user = await _userManager.GetUserAsync(User);   

            if (user != null)
                ViewData["UserName"] = user.UserId;

            return View();
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> StockInfo(string symbol, string name, string exchange, string currency, string type)
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new StockInfoViewModel()
            {
                Symbol = symbol,
                Name = name,
                Exchange = exchange,
                Currency = currency,
                Type = type,
                UserAPIKey = await _dbuser.GetUserAPIKeys(user.UserId)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetCodeFromDB(string param)
        {
            var elements = await _dbstocks.GetAutocomplete(param);

            return Json(elements);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "UnAuth");
        }

        public async Task<JsonResult> AutocompleteClicked(string symbol, string name, string exchange, string currency, string type)
        {
            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("StockInfo", new { symbol = symbol, name = name, exchange = exchange, currency = currency, type = type })
            });
        }

        public async Task<IActionResult> MyPage()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user.UserId;

            var model = new MyPageViewModel();

            model.UserAPIKey = await _dbuser.GetUserAPIKeys(user.UserId);

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> ModifyAppKey(string appkey)
        {
            var user = await _userManager.GetUserAsync(User);

            string userName = user.UserName;

            await _dbuser.ModifyAppKey(userName, appkey);

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("MyPage")
            });
        }

        [HttpPost]
        public async Task<JsonResult> ModifyAppSecret(string appsecret)
        {
            var user = await _userManager.GetUserAsync(User);

            await _dbuser.ModifyAppSecret(user.UserName, appsecret);

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("MyPage")
            });
        }
    }
}

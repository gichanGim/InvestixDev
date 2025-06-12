using Microsoft.AspNetCore.Mvc;

namespace InvestixDev.Controllers
{
    public class UnAuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

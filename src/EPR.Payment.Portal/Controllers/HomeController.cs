using Microsoft.AspNetCore.Mvc;

namespace EPR.Payment.Portal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

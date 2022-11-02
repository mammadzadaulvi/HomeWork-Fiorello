using Microsoft.AspNetCore.Mvc;

namespace Fiorello1.Controllers
{
    public class Myaccount : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

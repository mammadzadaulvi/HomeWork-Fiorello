using Microsoft.AspNetCore.Mvc;

namespace Fiorello1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

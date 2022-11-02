using Microsoft.AspNetCore.Mvc;

namespace Fiorello1.Controllers
{
    public class AboutusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

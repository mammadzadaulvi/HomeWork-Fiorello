using Microsoft.AspNetCore.Mvc;

namespace Fiorello1.Controllers
{
    public class FagpageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

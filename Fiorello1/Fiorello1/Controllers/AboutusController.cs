using Fiorello1.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Fiorello1.Controllers
{
    public class AboutusController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public AboutusController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

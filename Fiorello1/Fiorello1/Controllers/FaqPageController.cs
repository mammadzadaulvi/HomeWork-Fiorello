using Fiorello1.Areas.Admin.ViewModels.FaqPage;
using Fiorello1.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello1.Controllers
{
    public class FaqpageController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public FaqpageController (AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var model = new FaqPageIndexViewModel
            {
                FaqPages = await _appDbContext.FaqPages.OrderBy(fp => fp.Order).ToListAsync()
            };
            return View(model);
        }
    }
}

using Fiorello1.DAL;
using Fiorello1.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public HomeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexViewModel
            {
                Products = await _appDbContext.Products.OrderByDescending(p => p.Id).Take(8).ToListAsync(),
                HomeIntroSlider = await _appDbContext.HomeIntroSliders.Include(his => his.HomeIntroSliderPhotos.OrderBy(his => his.Order)).FirstOrDefaultAsync()
            };


            return View(model);
        }
    }
}

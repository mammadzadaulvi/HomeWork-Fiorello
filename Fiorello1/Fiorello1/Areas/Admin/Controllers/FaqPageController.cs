using Fiorello1.Areas.Admin.ViewModels.FaqPage;
using Fiorello1.DAL;
using Fiorello1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FaqPageController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public FaqPageController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var model = new FaqPageIndexViewModel
            {
                FaqPages = await _appDbContext.FaqPages.ToListAsync()
            };
            return View(model);

        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FaqPageCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            bool isExist = await _appDbContext.FaqPages.AnyAsync(c => c.Title.ToLower().Trim() == model.Title.ToLower().Trim());

            if (isExist)
            {
                ModelState.AddModelError("Title", "Choose another FaqPage name");

                return View(model);
            }

            var faqPage = new FaqPage
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Order = model.Order
            };

            await _appDbContext.FaqPages.AddAsync(faqPage);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        




        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var faqPage = await _appDbContext.FaqPages.FindAsync(id);

            if (faqPage == null) return NotFound();

            var model = new FaqPageUpdateViewModel
            {
                Id = faqPage.Id,
                Description = faqPage.Description,
                Order = faqPage.Order,
                Title = faqPage.Title
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, FaqPageUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var faqPage = await _appDbContext.FaqPages.FindAsync(id);
            if (id != model.Id) return BadRequest();
            if (faqPage == null) return NotFound();

            faqPage.Title = model.Title;
            faqPage.Order = model.Order;
            faqPage.Description = model.Description;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }





        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var faqPage = await _appDbContext.FaqPages.FindAsync(id);
            if (faqPage == null) return NotFound();

            _appDbContext.Remove(faqPage);

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]

        public async Task<IActionResult> Details(int id)
        {
            var faqPage = await _appDbContext.FaqPages.FindAsync(id);
            if (faqPage == null) return NotFound();

            var model = new FaqPageDetailsViewModel
            {
                Id = faqPage.Id,
                Title = faqPage.Title,
                Description = faqPage.Description,
                Order = faqPage.Order
            };

            return View(model);
        }
    }
}

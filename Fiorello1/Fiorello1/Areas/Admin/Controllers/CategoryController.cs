using Fiorello1.Areas.Admin.ViewModels.Category;
using Fiorello1.DAL;
using Fiorello1.Helpers;
using Fiorello1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //private readonly IFileService _fileService;

        public CategoryController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            //_fileService = fileService;
        }

        public async Task<IActionResult>  Index()
        {
            var model = new CategoryIndexViewModel
            {
                Categories = await _appDbContext.Categories.ToListAsync()
            };
            return View(model);
        }

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            bool isExist = await _appDbContext.Categories
                            .AnyAsync(c => c.Title.ToLower().Trim() == category.Title.ToLower().Trim());

            if (isExist)
            {
                ModelState.AddModelError("Title", "This title is already exist");
                return View(category);
            }

            await _appDbContext.Categories.AddAsync(category);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Update

        public async Task<IActionResult> Update(int id)
        {
            var category = await _appDbContext.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (!ModelState.IsValid) return View(category);
            if (id != category.Id) return BadRequest();

            var dbCategory = await _appDbContext.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();

            bool isExist = await _appDbContext.Categories.AnyAsync(ct => ct.Title.ToLower().Trim() == category.Title.ToLower().Trim() && ct.Id != category.Id);
            if (isExist)
            {
                ModelState.AddModelError("Title", "This component is already exist");
                return View(category);
            }
            dbCategory.Title = category.Title;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dbCategory = await _appDbContext.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();

            return View(dbCategory);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComponent(int id)
        {
            var dbCategory = await _appDbContext.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();

            _appDbContext.Categories.Remove(dbCategory);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

        #endregion

        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var recentWorkComponent = await _appDbContext.Categories.FindAsync(id);
            if (recentWorkComponent == null) return NotFound();
            return View(recentWorkComponent);
        }

        #endregion
    }
}

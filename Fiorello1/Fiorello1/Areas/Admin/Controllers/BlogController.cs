using Fiorello1.Areas.Admin.ViewModels.Blog;
using Fiorello1.Areas.Admin.ViewModels.Product;
using Fiorello1.Areas.Admin.ViewModels.Product.ProductPhoto;
using Fiorello1.DAL;
using Fiorello1.Helpers;
using Fiorello1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fiorello1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new BlogIndexViewModel
            {
                Blogs = await _appDbContext.Blogs.ToListAsync()
            };
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }


        [HttpPost]

        public async Task<IActionResult> Create(BlogCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            bool isExist = await _appDbContext.Blogs.AnyAsync(b => b.Title.ToLower().Trim() == model.Title.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Title", "Choose another Blog name");
                return View(model);
            }
            if (!_fileService.IsImage(model.Photo))
            {
                ModelState.AddModelError("MainPhoto", "Yüklənən fayl image formatında olmalıdır.");
                return View(model);
            }
            if (!_fileService.CheckSize(model.Photo, 300))
            {
                ModelState.AddModelError("MainPhoto", "Şəkilin ölçüsü 300 kb-dan böyükdür");
                return View(model);
            }

            if (model.DateCreated == null)
            {
                model.DateCreated = DateTime.Today;
            }

            var blog = new Blog
            {
                Title = model.Title,
                Description = model.Description,
                DateCreated = model.DateCreated.Value,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath),
            };

            await _appDbContext.Blogs.AddAsync(blog);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var blog = await _appDbContext.Blogs.FindAsync(id);

            if (blog == null) return NotFound();

            var model = new BlogUpdateViewModel
            {
                Title = blog.Title,
                Description = blog.Description,
                DateCreated = blog.DateCreated,
                PhotoPath = blog.PhotoPath
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(BlogUpdateViewModel model, int id)
        {
            var blog = await _appDbContext.Blogs.FindAsync(id);
            if (!ModelState.IsValid) return View(model);
            if (id != model.Id) return BadRequest();

            bool isExist = await _appDbContext.Blogs.AnyAsync(p => p.Title.ToLower().Trim() == model.Title.ToLower().Trim() && p.Id != blog.Id);

            if (isExist)
            {
                ModelState.AddModelError("Title", "Choose another Blog name");
                return View(model);
            }

            blog.Title = model.Title;
            blog.Description = model.Description;
            blog.DateCreated = model.DateCreated.Value;
            blog.PhotoPath = model.PhotoPath;


            if (model.Photo != null)
            {

                if (!_fileService.IsImage(model.Photo))
                {
                    ModelState.AddModelError("Photo", "Yüklənən fayl image formatında olmalıdır.");
                    return View(model);
                }
                if (!_fileService.CheckSize(model.Photo, 300))
                {
                    ModelState.AddModelError("Photo", "Şəkilin ölçüsü 300 kb-dan böyükdür");
                    return View(model);
                }
                blog.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }


            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }






        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _appDbContext.Blogs.FindAsync(id);
            if (blog == null) return NotFound();

            _appDbContext.Blogs.Remove(blog);

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }




        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var blog = await _appDbContext.Blogs.FindAsync(id);
            if (blog == null) return NotFound();

            var model = new BlogDetailsViewModel
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                DateCreated = blog.DateCreated,
                PhotoPath = blog.PhotoPath
            };
            return View(model);

        }
    }
}

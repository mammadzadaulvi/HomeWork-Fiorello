using Fiorello1.Areas.Admin.ViewModels.Expert;
using Fiorello1.DAL;
using Fiorello1.Helpers;
using Fiorello1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fiorello1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ExpertController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ExpertController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {

            var model = new ExpertIndexViewModel
            {
                Experts = await _appDbContext.Experts.ToListAsync()
            };
            return View(model);
        }

        [HttpGet]

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(ExpertCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

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

            var expert = new Expert
            {
                Fullname = model.Fullname,
                Position = model.Position,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.Experts.AddAsync(expert);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        [HttpGet]

        public async Task<IActionResult> Delete(int id)
        {
            var expert = await _appDbContext.Experts.FindAsync(id);
            if (expert == null) return NotFound();

            _fileService.Delete(expert.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.Experts.Remove(expert);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var expert = await _appDbContext.Experts.FindAsync(id);
            if (expert == null) return NotFound();

            var model = new ExpertDetailsViewModel
            {
                Id = expert.Id,
                Fullname = expert.Fullname,
                Position = expert.Position,
                PhotoPath = expert.PhotoPath
            };
            return View(model);
        }


        [HttpGet]

        public async Task<IActionResult> Update(int id)
        {
            var expert = await _appDbContext.Experts.FindAsync(id);

            if (expert == null) return NotFound();


            var model = new ExpertUpdateViewModel
            {
                Id = expert.Id,
                Fullname = expert.Fullname,
                PhotoPath = expert.PhotoPath,
                Position = expert.Position
            };
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Update(ExpertUpdateViewModel model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var expert = await _appDbContext.Experts.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (expert == null) return NotFound();

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

                _fileService.Delete(expert.PhotoPath, _webHostEnvironment.WebRootPath);
                expert.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            expert.Fullname = model.Fullname;
            expert.Position = model.Position;
            model.PhotoPath = expert.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}

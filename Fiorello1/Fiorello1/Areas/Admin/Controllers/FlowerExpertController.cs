using Fiorello1.Areas.Admin.ViewModels.FlowerExpert;
using Fiorello1.DAL;
using Fiorello1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fiorello1.Helpers;

namespace Fiorello1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FlowerExpertController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public FlowerExpertController(AppDbContext appDbContext,
            IWebHostEnvironment webHostEnvironment,
            IFileService fileService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }


        public async Task<IActionResult> Index()
        {
            var model = new FlowerExpertIndexViewModel
            {
                FlowerExperts = await _appDbContext.FlowerExperts.ToListAsync()
            };
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FlowerExpertCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (!_fileService.IsImage(model.Photo))
            {
                ModelState.AddModelError("Photo", "Yüklənən fayl image formatında olmalıdır.");
                return View(model);
            }


            if (!_fileService.CheckSize(model.Photo, 300))
            {
                ModelState.AddModelError("Photo", $"Şəkilin ölçüsü 300 kb-dan böyükdür");
                return View(model);
            }

            var flowerExpert = new FlowerExpert
            {
                Title = model.Title,
                Description = model.Description,
                ExpertName = model.ExpertName,
                ExpertPosition = model.ExpertPosition,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.FlowerExperts.AddAsync(flowerExpert);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }





        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var flowerExpert = await _appDbContext.FlowerExperts.FindAsync(id);

            if (flowerExpert == null) return NotFound();


            var model = new FlowerExpertUpdateViewModel
            {
                Id = flowerExpert.Id,
                Title = flowerExpert.Title,
                Description = flowerExpert.Description,
                ExpertName = flowerExpert.ExpertName,
                ExpertPosition = flowerExpert.ExpertPosition,
                PhotoPath = flowerExpert.PhotoPath
            };
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Update(FlowerExpertUpdateViewModel model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var flowerExpert = await _appDbContext.FlowerExperts.FindAsync(id);
            
            if (id != model.Id) return BadRequest();
            if (flowerExpert == null) return NotFound();


            if (model.Photo != null)
            {

                if (!_fileService.IsImage(model.Photo))
                {
                    ModelState.AddModelError("Photo", "Yüklənən fayl image formatında olmalıdır.");
                    return View(model);
                }
                if (!_fileService.CheckSize(model.Photo, 300))
                {
                    ModelState.AddModelError("Photo", "Sekilin olcusu 300 kb dan boyukdur");
                    return View(model);
                }

                _fileService.Delete( flowerExpert.PhotoPath, _webHostEnvironment.WebRootPath);
                flowerExpert.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }


            flowerExpert.Title = model.Title;
            flowerExpert.Description = model.Description;
            flowerExpert.PhotoPath = model.PhotoPath;
            flowerExpert.ExpertName = model.ExpertName;
            flowerExpert.ExpertPosition = model.ExpertPosition;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var flowerExpert = await _appDbContext.FlowerExperts.FindAsync(id);
            if (flowerExpert == null) return NotFound();

            _fileService.Delete( flowerExpert.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.FlowerExperts.Remove(flowerExpert);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var flowerExpert = await _appDbContext.FlowerExperts.FindAsync(id);
            if (flowerExpert == null) return NotFound();

            var model = new FlowerExpertDetailsViewModel
            {
                Title = flowerExpert.Title,
                Description = flowerExpert.Description,
                ExpertName = flowerExpert.ExpertName,
                ExpertPosition = flowerExpert.ExpertPosition,
                PhotoPath = flowerExpert.PhotoPath
            };
            return View(model); 
        }
    }
}

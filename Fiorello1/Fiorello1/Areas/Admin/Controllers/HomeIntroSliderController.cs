using Fiorello1.Areas.Admin.ViewModels.HomeIntroSlider;
using Fiorello1.Areas.Admin.ViewModels.HomeIntroSlider.HomeIntroSliderPhoto;
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
    public class HomeIntroSliderController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public HomeIntroSliderController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }
        public async Task<IActionResult> Index()
        {
            var model = new HomeIntroSliderIndexViewModel
            {
                HomeIntroSlider = await _appDbContext.HomeIntroSliders.FirstOrDefaultAsync()
            };
            return View(model);

        }





        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var homeIntroSlider = await _appDbContext.HomeIntroSliders.FirstOrDefaultAsync();
            if (homeIntroSlider != null) return NotFound();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(HomeIntroSliderCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            bool isExist = await _appDbContext.HomeIntroSliders.AnyAsync(hs => hs.Title.ToLower().Trim() == model.Title.ToLower().Trim());

            if (isExist)
            {
                ModelState.AddModelError("Title", "Choose another Slide name");
                return View(model);
            }

            if (!_fileService.IsImage(model.AddPhoto))
            {
                ModelState.AddModelError("AddPhoto", "Yüklənən fayl image formatında olmalıdır.");
                return View(model);
            }

            if (!_fileService.CheckSize(model.AddPhoto, 300))
            {
                ModelState.AddModelError("AddPhoto", "Şəkilin ölçüsü 300 kb-dan böyükdür");
                return View(model);
            }

            bool hasError = false;
            foreach (var photo in model.Photos)
            {
                if (!_fileService.IsImage(photo))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} yuklediyiniz file sekil formatinda olmalidir");
                    hasError = true;

                }
                else if (!_fileService.CheckSize(photo, 300))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} ci yuklediyiniz sekil 300 kb dan az olmalidir");
                    hasError = true;

                }

            }

            if (hasError) return View(model);

            var homeIntroSlider = new HomeIntroSlider
            {
                Title = model.Title,
                Description = model.Description,
                AddPhotoName = await _fileService.UploadAsync(model.AddPhoto, _webHostEnvironment.WebRootPath),
            };

            await _appDbContext.HomeIntroSliders.AddAsync(homeIntroSlider);
            await _appDbContext.SaveChangesAsync();

            int order = 1;
            foreach (var photo in model.Photos)
            {
                var homeIntroSliderPhoto = new HomeIntroSliderPhoto
                {
                    Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                    Order = order,
                    HomeIntroSliderId = homeIntroSlider.Id
                };
                await _appDbContext.HomeIntroSliderPhotos.AddAsync(homeIntroSliderPhoto);
                await _appDbContext.SaveChangesAsync();

                order++;
            }

            return RedirectToAction("Index");
        }


        


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var homeIntroSlider = await _appDbContext.HomeIntroSliders.Include(hs => hs.HomeIntroSliderPhotos).FirstOrDefaultAsync(hs => hs.Id == id);
            if (homeIntroSlider == null) return NotFound();

            var model = new HomeIntroSliderUpdateViewModel
            {
                Title = homeIntroSlider.Title,
                Description = homeIntroSlider.Description,
                AddPhotoName = homeIntroSlider.AddPhotoName,
                HomeIntroSliderPhotos = homeIntroSlider.HomeIntroSliderPhotos,
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Update(HomeIntroSliderUpdateViewModel model, int id)
        {
            if (!ModelState.IsValid) return View(model);
            if (id != model.Id) return BadRequest();

            var homeIntroSlider = await _appDbContext.HomeIntroSliders.Include(hs => hs.HomeIntroSliderPhotos).FirstOrDefaultAsync(hs => hs.Id == id);

            model.HomeIntroSliderPhotos = homeIntroSlider.HomeIntroSliderPhotos.ToList();
            if (homeIntroSlider == null) return NotFound();



            if (model.AddPhoto != null)
            {

                if (!_fileService.IsImage(model.AddPhoto))
                {
                    ModelState.AddModelError("Photo", "Image formatinda secin");
                    return View(model);
                }
                if (!_fileService.CheckSize(model.AddPhoto, 300))
                {
                    ModelState.AddModelError("Photo", "Sekilin olcusu 300 kb dan boyukdur");
                    return View(model);
                }

                _fileService.Delete(homeIntroSlider.AddPhotoName, _webHostEnvironment.WebRootPath);
                homeIntroSlider.AddPhotoName = await _fileService.UploadAsync(model.AddPhoto, _webHostEnvironment.WebRootPath);
            }

            bool hasError = false;

            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    if (!_fileService.IsImage(photo))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName} yuklediyiniz file sekil formatinda olmalidir");
                        hasError = true;
                    }
                    else if (!_fileService.CheckSize(photo, 300))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName} ci yuklediyiniz sekil 300 kb dan az olmalidir");
                        hasError = true;
                    }
                }

                if (hasError) { return View(model); }
                var homeIntroSliderPhoto = homeIntroSlider.HomeIntroSliderPhotos.OrderByDescending(hs => hs.Order).FirstOrDefault();
                int order = homeIntroSliderPhoto != null ? homeIntroSliderPhoto.Order : 0;
                foreach (var photo in model.Photos)
                {
                    var productPhoto = new HomeIntroSliderPhoto
                    {
                        Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                        Order = ++order,
                        HomeIntroSliderId = homeIntroSlider.Id
                    };
                    await _appDbContext.HomeIntroSliderPhotos.AddAsync(productPhoto);
                    await _appDbContext.SaveChangesAsync();
                }
            }

            homeIntroSlider.Title = model.Title;
            homeIntroSlider.Description = model.Description;
            model.AddPhotoName = homeIntroSlider.AddPhotoName;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }







        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var homeIntroSlider = await _appDbContext.HomeIntroSliders.Include(hs => hs.HomeIntroSliderPhotos).FirstOrDefaultAsync(his => his.Id == id);
            if (homeIntroSlider == null) return NotFound();

            _fileService.Delete(homeIntroSlider.AddPhotoName, _webHostEnvironment.WebRootPath);

            foreach (var photo in homeIntroSlider.HomeIntroSliderPhotos)
            {
                _fileService.Delete(photo.Name, _webHostEnvironment.WebRootPath);

            }

            _appDbContext.HomeIntroSliders.Remove(homeIntroSlider);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }




        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var homeIntroSlider = await _appDbContext.HomeIntroSliders.Include(hs => hs.HomeIntroSliderPhotos).FirstOrDefaultAsync(his => his.Id == id);

            if (homeIntroSlider == null) return NotFound();

            var model = new HomeIntroSliderDetailsViewModel
            {
                Id = homeIntroSlider.Id,
                Title = homeIntroSlider.Title,
                Description = homeIntroSlider.Description,
                AddPhotoName = homeIntroSlider.AddPhotoName,
                Photos = homeIntroSlider.HomeIntroSliderPhotos
            };

            return View(model);
        }






        [HttpGet]
        public async Task<IActionResult> UpdatePhoto(int id)
        {
            var homeIntroSliderPhoto = await _appDbContext.HomeIntroSliderPhotos.FindAsync(id);
            if (homeIntroSliderPhoto == null) return NotFound();

            var model = new HomeIntroSliderPhotoUpdateViewModel
            {
                Order = homeIntroSliderPhoto.Order
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhoto(int id, HomeIntroSliderPhotoUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (id != model.Id) return BadRequest();

            var homeIntroSliderPhoto = await _appDbContext.HomeIntroSliderPhotos.FindAsync(model.Id);
            if (homeIntroSliderPhoto == null) return NotFound();

            homeIntroSliderPhoto.Order = model.Order;
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("update", "homeintroslider", new { id = homeIntroSliderPhoto.HomeIntroSliderId });

        }




        [HttpGet]
        public async Task<IActionResult> Deletephoto(int id)
        {
            var homeIntroSliderPhoto = await _appDbContext.HomeIntroSliderPhotos.FindAsync(id);
            if (homeIntroSliderPhoto == null) return NotFound();

            _fileService.Delete(homeIntroSliderPhoto.Name, _webHostEnvironment.WebRootPath);

            _appDbContext.HomeIntroSliderPhotos.Remove(homeIntroSliderPhoto);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("update", "homeintroslider", new { id = homeIntroSliderPhoto.HomeIntroSliderId });
        }
    }
}

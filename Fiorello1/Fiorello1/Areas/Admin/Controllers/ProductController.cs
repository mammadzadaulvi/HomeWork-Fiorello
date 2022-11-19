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
using static Fiorello1.Models.Product;

namespace Fiorello1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Filter

        private IQueryable<Product> FilterProducts(ProductIndexViewModel model)
        {
            var products = FilterByTitle(model.Title);
            products = FilterByCategory(products, model.CategoryId);

            products = FilterByPrice(products, model.MinPrice, model.MaxPrice);

            products = FilterByQuantity(products, model.MinQuantity, model.MaxQuantity);

            products = FilterByCreatedAt(products, model.DateCreatedStart, model.DateCreatedEnd);

            products = FilterByStatus(products, model.Status);

            return products;
        }

        private IQueryable<Product> FilterByTitle(string title)
        {
            return _appDbContext.Products.Where(p => !string.IsNullOrEmpty(title) ? p.Title.Contains(title) : true);
        }

        private IQueryable<Product> FilterByCategory(IQueryable<Product> products, int? categoryId)
        {
            return products.Where(p => categoryId != null ? p.CategoryId == categoryId : true);
        }

        private IQueryable<Product> FilterByPrice(IQueryable<Product> products, double? minPrice, double? maxPrice)
        {
            return products.Where(p => (minPrice != null ? p.Price >= minPrice : true) && (maxPrice != null ? p.Price <= maxPrice : true));
        }

        private IQueryable<Product> FilterByQuantity(IQueryable<Product> products, int? minQuantity, int? maxQuantity)
        {
            return products.Where(p => (minQuantity != null ? p.Quantity >= minQuantity : true) && (maxQuantity != null ? p.Quantity <= maxQuantity : true));
        }

        private IQueryable<Product> FilterByCreatedAt(IQueryable<Product> products, DateTime? createdAtstart, DateTime? createdEndstart)
        {
            return products.Where(p => (createdAtstart != null ? p.CreateAt>= createdAtstart : true) && (createdEndstart != null ? p.CreateAt <= createdEndstart : true));
        }

        private IQueryable<Product> FilterByStatus(IQueryable<Product> products, ProductStatus? status)
        {
            return products.Where(p => status != null ? p.Status == status : true);
        }

        #endregion


        public async Task<IActionResult> Index(ProductIndexViewModel model)
        {
            var products = FilterProducts(model);

            model = new ProductIndexViewModel
            {
                Products = await products.Include(p => p.Category).ToListAsync(),
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                })
               .ToListAsync()

            };
            return View(model);
        }


        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ProductCreateViewModel
            {
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            model.Categories = await _appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();

            if (!ModelState.IsValid) return View(model);
            var category = await _appDbContext.Categories.FindAsync(model.CategoryId);

            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Category not found");
                return View(model);
            }

            bool isExist = await _appDbContext.Products.AnyAsync(p => p.Title.ToLower().Trim() == model.Title.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Title", "This category is already exist");
                return View(model);
            }
            if (!_fileService.IsImage(model.MainPhoto))
            {
                ModelState.AddModelError("MainPhoto", "The image must be img format");
                return View(model);
            }
            if (!_fileService.CheckSize(model.MainPhoto, 300))
            {
                ModelState.AddModelError("MainPhoto", "This image is bigger than 300kb");
                return View(model);
            }

            bool hasError = false;
            foreach (var photo in model.AddPhotos)
            {
                if (!_fileService.IsImage(photo))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} image must be img format");
                    hasError = true;
                }
                else if (!_fileService.CheckSize(photo, 300))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} image is bigger than 300kb");
                    hasError = true;
                }

            }

            if (hasError) { return View(model);}

            var product = new Product
            {
                Title = model.Title,
                Price = model.Price,
                Quantity = model.Quantity,
                Description = model.Description,
                Weight = model.Weight,
                Dimension = model.Dimension,
                CategoryId = model.CategoryId,
                Status = model.Status,
                MainPhotoName = await _fileService.UploadAsync(model.MainPhoto, _webHostEnvironment.WebRootPath),
            };

            await _appDbContext.Products.AddAsync(product);
            await _appDbContext.SaveChangesAsync();

            int order = 1;
            foreach (var photo in model.AddPhotos)
            {
                var productPhoto = new ProductPhoto
                {
                    Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                    Order = order,
                    ProductId = product.Id
                };
                await _appDbContext.ProductPhotos.AddAsync(productPhoto);
                await _appDbContext.SaveChangesAsync();

                order++;
            }
            return RedirectToAction("Index");

        }
        #endregion

        #region Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _appDbContext.Products.Include(p => p.ProductPhotos).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var model = new ProductUpdateViewModel
            {
                Title = product.Title,
                Price = product.Price,
                Quantity = product.Quantity,
                Description = product.Description,
                Weight = product.Weight,
                Dimension = product.Dimension,
                CategoryId = product.CategoryId,
                Status = product.Status,
                MainPhotoName = product.MainPhotoName,
                ProductPhotos = product.ProductPhotos,

                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateViewModel model, int id)
        {
            model.Categories = await _appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();

            if (id != model.Id) return View(model);
            if (id != model.Id) return BadRequest();

            var product = await _appDbContext.Products.Include(p => p.ProductPhotos).FirstOrDefaultAsync(p => p.Id == id);

            model.ProductPhotos = product.ProductPhotos.ToList();

            if (product == null) return NotFound();
            bool isExist = await _appDbContext.Products.AnyAsync(p => p.Title.ToLower().Trim() == product.Title.ToLower().Trim() && p.Id != product.Id);

            if (isExist)
            {
                ModelState.AddModelError("Title", "This product is already exist");
                return View(model);
            }


            if (model.MainPhoto != null)
            {

                if (!_fileService.IsImage(model.MainPhoto))
                {
                    ModelState.AddModelError("Photo", "The image must be img format");
                    return View(model);
                }
                if (!_fileService.CheckSize(model.MainPhoto, 300))
                {
                    ModelState.AddModelError("Photo", "The image is bigger than 300kb");
                    return View(model);
                }

                _fileService.Delete(model.MainPhotoName, _webHostEnvironment.WebRootPath);
                product.MainPhotoName = await _fileService.UploadAsync(model.MainPhoto, _webHostEnvironment.WebRootPath);
            }

            var category = await _appDbContext.Categories.FindAsync(model.CategoryId);
            if (category == null) return NotFound();
            product.CategoryId = category.Id;


            await _appDbContext.SaveChangesAsync();


            bool hasError = false;

            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    if (!_fileService.IsImage(photo))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName} must be img format");
                        hasError = true;
                    }
                    else if (!_fileService.CheckSize(photo, 300))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName} is bigger than 300kb ");
                        hasError = true;
                    }
                }

                if (hasError) { return View(model); }

                int order = 1;
                foreach (var photo in model.Photos)
                {
                    var productPhoto = new ProductPhoto
                    {
                        Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                        Order = order,
                        ProductId = product.Id
                    };
                    await _appDbContext.ProductPhotos.AddAsync(productPhoto);
                    await _appDbContext.SaveChangesAsync();

                    order++;
                }
            }



            product.Title = model.Title;
            product.Price = model.Price;
            product.Description = model.Description;
            product.Quantity = model.Quantity;
            product.Weight = model.Weight;
            product.Dimension = model.Dimension;
            product.Status = model.Status;
            product.MainPhotoName = model.MainPhotoName;

            return RedirectToAction("Index");

        }

        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _appDbContext.Products.Include(p => p.ProductPhotos).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            _fileService.Delete(product.MainPhotoName, _webHostEnvironment.WebRootPath);

            foreach (var photo in product.ProductPhotos)
            {
                _fileService.Delete(photo.Name, _webHostEnvironment.WebRootPath);

            }
            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Details
        [HttpGet]

        public async Task<IActionResult> Details(int id)
        {
            var product = await _appDbContext.Products.Include(p => p.ProductPhotos).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var model = new ProductDetailsViewModel
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                Quantity = product.Quantity,
                CategoryId = product.CategoryId,
                Description = product.Description,
                Weight = product.Weight,
                Dimension = product.Dimension,
                Status = product.Status,
                MainPhoto = product.MainPhotoName,
                AddPhotos = product.ProductPhotos,
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }

        #endregion

        


        #region UpdatePhoto

        [HttpGet]
        public async Task<IActionResult> UpdatePhoto(int id)
        {
            var productPhoto = await _appDbContext.ProductPhotos.FindAsync(id);
            if (productPhoto == null) return NotFound();

            var model = new ProductPhotoUpdateViewModel
            {
                Order = productPhoto.Order
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhoto(int id, ProductPhotoUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (id != model.Id) return BadRequest();

            var productPhoto = await _appDbContext.ProductPhotos.FindAsync(model.Id);
            if (productPhoto == null) return NotFound();

            productPhoto.Order = model.Order;
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("update", "product", new { id = productPhoto.ProductId });
        }

        #endregion

        #region DeletePhoto

        [HttpGet]
        public async Task<IActionResult> Deletephoto(int id)
        {
            var productPhoto = await _appDbContext.ProductPhotos.FindAsync(id);
            if (productPhoto == null) return NotFound();
            _fileService.Delete(productPhoto.Name, _webHostEnvironment.WebRootPath);

            _appDbContext.ProductPhotos.Remove(productPhoto);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("update", "product", new { id = productPhoto.ProductId });
        }

        #endregion


    }
}

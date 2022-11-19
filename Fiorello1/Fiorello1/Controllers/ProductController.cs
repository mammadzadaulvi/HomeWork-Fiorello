using Fiorello1.ViewModels.Product;
using Fiorello1.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello1.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public ProductController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ProductIndexViewModel
            {
                Products = await _appDbContext.Products
                                              .OrderByDescending(p => p.Id)
                                              .Take(4)
                                              .ToListAsync()
            };
            return View(model);
        }


        public async Task<IActionResult> LoadMore(int skipRow)
        {
            bool isLast = false;
            var product = await _appDbContext.Products
                               .OrderByDescending(p => p.Id)
                               .Skip(4 * skipRow)
                               .Take(4)
                               .ToListAsync();

            if ((4 * skipRow) + 4 > _appDbContext.Products.Count())
            {
                isLast = true;
            }

            var model = new ProductLoadMoreViewModel
            {
                Products = product,
                Islast = isLast
            };

            return PartialView("_ProductsPartial", model);
        }


        public async Task<IActionResult> Details(int id)
        {
            var product = await _appDbContext.Products.Include(p => p.ProductPhotos).Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var model = new ProductDetailsViewModel
            {
                Id = product.Id,
                Status = product.Status,
                Category = product.Category,
                Description = product.Description,
                Quantity = product.Quantity,
                Title = product.Title,
                Dimension = product.Dimension,
                MainPhoto = product.MainPhotoName,
                Weight = product.Weight,
                Price = product.Price,
                AddPhotos = product.ProductPhotos,
            };

            return View(model);
        }
    }
}

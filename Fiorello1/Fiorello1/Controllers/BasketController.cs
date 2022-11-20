using Fiorello1.DAL;
using Fiorello1.Models;
using Fiorello1.ViewModels.Basket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello1.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public BasketController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var basket = await _context.Baskets.Include(b => b.BasketProducts).ThenInclude(bp => bp.Product).FirstOrDefaultAsync(b => b.UserId == user.Id);
            var model = new BasketIndexViewModel();

            if (basket == null) return View(model);

            foreach (var dbBasketProduct in basket.BasketProducts)
            {
                var basketProduct = new BasketProductViewModel
                {
                    Id = dbBasketProduct.Id,
                    Title = dbBasketProduct.Product.Title,
                    Quantity = dbBasketProduct.Quantity,
                    PhotoName = dbBasketProduct.Product.MainPhotoName,
                    StockQuantity = dbBasketProduct.Product.Quantity,
                    Price = dbBasketProduct.Product.Price,
                };
                model.BasketProducts.Add(basketProduct);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(BasketAddViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(model);

            var user = await _userManager.GetUserAsync(User);

            if (user == null) return Unauthorized();

            var product = await _context.Products.FindAsync(model.Id);

            if (product == null) return NotFound();

            var basket = await _context.Baskets.FirstOrDefaultAsync(b => b.UserId == user.Id);

            if (basket == null)
            {
                basket = new Basket
                {
                    UserId = user.Id
                };

                await _context.Baskets.AddAsync(basket);
                await _context.SaveChangesAsync();
            }

            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(bp => bp.ProductId == product.Id && bp.BasketId == basket.Id);
            if (basketProduct != null)
            {
                basketProduct.Quantity++;
            }
            else
            {
                basketProduct = new BasketProduct
                {
                    BasketId = basket.Id,
                    ProductId = product.Id,
                    Quantity = 1
                };

                await _context.BasketProducts.AddAsync(basketProduct);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(bp => bp.Id == id && bp.Basket.UserId == user.Id);
            if (basketProduct == null) return NotFound();

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketProduct.ProductId);
            if (product == null) return NotFound();

            _context.BasketProducts.Remove(basketProduct);

            await _context.SaveChangesAsync();

            return Ok();

        }



        [HttpPost]
        public async Task<IActionResult> UpCount(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            
            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(bp => bp.Id == id && bp.Basket.UserId == user.Id);
            if (basketProduct == null) return NotFound();
            
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketProduct.ProductId);
            if (product == null) return NotFound();

            basketProduct.Quantity++;

            await _context.SaveChangesAsync();

            return Ok();
        }




        [HttpPost]
        public async Task<IActionResult> DownCount(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(bp => bp.Id == id && bp.Basket.UserId == user.Id);
            if (basketProduct == null) return NotFound();

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketProduct.ProductId);
            if (product == null) return NotFound();

            basketProduct.Quantity--;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}

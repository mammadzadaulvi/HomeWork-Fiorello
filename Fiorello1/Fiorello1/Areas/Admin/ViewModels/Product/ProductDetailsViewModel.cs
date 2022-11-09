using Microsoft.AspNetCore.Mvc.Rendering;
using static Fiorello1.Models.Product;

namespace Fiorello1.Areas.Admin.ViewModels.Product
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }

        public List<SelectListItem>? Categories { get; set; }
        public string Description { get; set; }
        public string Weight { get; set; }
        public string Dimension { get; set; }
        public ProductStatus Status { get; set; }
        public string MainPhoto { get; set; }
        public ICollection<Models.ProductPhoto> AddPhotos { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using static Fiorello1.Models.Product;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Fiorello1.Areas.Admin.ViewModels.Product
{
    public class ProductIndexViewModel
    {
        public List<Models.Product> Products { get; set; }




        public string? Title { get; set; }
        public List<SelectListItem> Categories { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }



        [Display(Name = "Minimum Price")]
        public double? MinPrice { get; set; }

        [Display(Name = "Maximum Price")]
        public double? MaxPrice { get; set; }



        [Display(Name = "Minimum Quantity")]
        public int? MinQuantity { get; set; }

        [Display(Name = "Maximum Quantity")]
        public int? MaxQuantity { get; set; }



        [Display(Name = "Start date")]
        public DateTime? DateCreatedStart { get; set; }

        [Display(Name = "End date")]
        public DateTime? DateCreatedEnd { get; set; }



        public ProductStatus? Status { get; set; }
    }
}

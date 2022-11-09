using System.ComponentModel.DataAnnotations;

namespace Fiorello1.Areas.Admin.ViewModels.FlowerExpert
{
    public class FlowerExpertUpdateViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IFormFile? Photo { get; set; }

        public string? PhotoPath { get; set; }

        public string ExpertName { get; set; }

        public string ExpertPosition { get; set; }
    }
}

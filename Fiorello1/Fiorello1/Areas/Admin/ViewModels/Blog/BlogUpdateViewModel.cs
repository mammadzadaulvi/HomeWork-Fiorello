namespace Fiorello1.Areas.Admin.ViewModels.Blog
{
    public class BlogUpdateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreated { get; set; }
        public IFormFile? Photo { get; set; }
        public string? PhotoPath { get; set; }
    }
}

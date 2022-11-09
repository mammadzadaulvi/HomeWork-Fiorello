namespace Fiorello1.Areas.Admin.ViewModels.Expert
{
    public class ExpertUpdateViewModel
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Position { get; set; }

        public string? PhotoPath { get; set; }
        public IFormFile? Photo { get; set; }
    }
}

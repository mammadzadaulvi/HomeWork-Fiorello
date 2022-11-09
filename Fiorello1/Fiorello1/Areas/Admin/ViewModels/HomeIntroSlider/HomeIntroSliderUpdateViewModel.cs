namespace Fiorello1.Areas.Admin.ViewModels.HomeIntroSlider
{
    public class HomeIntroSliderUpdateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile? AddPhoto { get; set; }
        public string? AddPhotoName { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public ICollection<Models.HomeIntroSliderPhoto>? HomeIntroSliderPhotos { get; set; }
    }
}

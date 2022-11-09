namespace Fiorello1.Areas.Admin.ViewModels.HomeIntroSlider
{
    public class HomeIntroSliderCreateViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile AddPhoto { get; set; }
        public List<IFormFile> Photos { get; set; }
    }
}

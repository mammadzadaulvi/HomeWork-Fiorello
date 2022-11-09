namespace Fiorello1.Areas.Admin.ViewModels.HomeIntroSlider
{
    public class HomeIntroSliderDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string AddPhotoName { get; set; }
        public ICollection<Models.HomeIntroSliderPhoto> Photos { get; set; }
    }
}

namespace Fiorello1.Models
{
    public class HomeIntroSlider
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AddPhotoName { get; set; }
        public ICollection<HomeIntroSliderPhoto> HomeIntroSliderPhotos { get; set; }
    }
}

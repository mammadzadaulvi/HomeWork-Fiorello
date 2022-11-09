namespace Fiorello1.Models
{
    public class HomeIntroSliderPhoto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int HomeIntroSliderId { get; set; }
        public HomeIntroSlider HomeIntroSlider { get; set; }
    }
}

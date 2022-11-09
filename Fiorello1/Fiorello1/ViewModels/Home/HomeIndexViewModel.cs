namespace Fiorello1.ViewModels.Home
{
    public class HomeIndexViewModel
    {
        public List<Models.Expert> Experts { get; set; }
        public List<Models.Product> Products { get; set; }

        public Models.HomeIntroSlider HomeIntroSlider { get; set; } 
    }
}

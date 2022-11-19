namespace Fiorello1.ViewModels.Blog
{
    public class BlogIndexViewModel
    {
        public List<Models.Blog> Blogs { get; set; }

        public int Page { get; set; } = 1;

        public int Take { get; set; } = 9;

        public int PageCount { get; set; }
    }
}

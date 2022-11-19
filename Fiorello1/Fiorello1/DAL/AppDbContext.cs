using Fiorello1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fiorello1.DAL
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<FlowerExpert> FlowerExperts { get; set; }
        public DbSet<Expert> Experts { get; set; }
        public DbSet<FaqPage> FaqPages { get; set; }

        public DbSet<HomeIntroSlider> HomeIntroSliders { get; set; }

        public DbSet<HomeIntroSliderPhoto> HomeIntroSliderPhotos { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketProduct> BasketProducts { get; set; }
    }
}

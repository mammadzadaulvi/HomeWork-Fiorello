namespace Fiorello1.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public string MainPhotoName { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public string Weight { get; set; }  
        public string Dimension { get; set; }   
        public ProductStatus Status { get; set; }
        public DateTime CreateAt { get; set; }

        public ICollection<ProductPhoto> ProductPhotos { get; set; }
        public List<BasketProduct> BasketProducts { get; set; }

        public enum ProductStatus
        {
            New,
            Sold,
            Sale
        }
    }
}

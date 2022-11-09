namespace Fiorello1.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 
        public string PhotoPath { get; set; }   
        public DateTime DateCreated { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace Fiorello1.Models
{
    public class User : IdentityUser    
    {
        public string FullName { get; set; }
        public Basket Basket { get; set; }
    }
}

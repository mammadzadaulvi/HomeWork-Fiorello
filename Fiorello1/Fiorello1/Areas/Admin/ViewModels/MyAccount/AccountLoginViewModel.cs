using System.ComponentModel.DataAnnotations;

namespace Fiorello1.Areas.Admin.ViewModels.MyAccount
{
    public class AccountLoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

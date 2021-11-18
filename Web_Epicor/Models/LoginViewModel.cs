using System.ComponentModel.DataAnnotations;


namespace Web_Epicor.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string pass { get; set; }
    }
}

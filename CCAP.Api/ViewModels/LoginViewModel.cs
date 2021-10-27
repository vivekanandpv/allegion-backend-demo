using System.ComponentModel.DataAnnotations;

namespace CCAP.Api.ViewModels {
    public class LoginViewModel {
        [Required]
        [EmailAddress]
        public string Username { get; set; }
        
        [Required]
        [MinLength(5, ErrorMessage = "Password should be minimum 5 characters")]
        public string Password { get; set; }
    }
}
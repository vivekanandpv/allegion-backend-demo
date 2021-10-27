using System.ComponentModel.DataAnnotations;

namespace CCAP.Api.ViewModels {
    public class GeneralUserRegisterViewModel {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required, RegularExpression(pattern:"^.*(?=.{8,})(?=.*[a-zA-Z])(?=.*\\d)(?=.*[!@#$%&? \"]).*$", ErrorMessage = "Minimum 8 characters, must contain a digit, a special character, and a capital letter")]
        public string Password { get; set; }
    }
}
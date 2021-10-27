using System.ComponentModel.DataAnnotations;

namespace CCAP.Api.ViewModels {
    public class ChangePasswordViewModel {
        [Required, EmailAddress]
        public string Username { get; set; }
        
        [Required, RegularExpression(pattern:"^.*(?=.{8,})(?=.*[a-zA-Z])(?=.*\\d)(?=.*[!@#$%&? \"]).*$", ErrorMessage = "Minimum 8 characters, must contain a digit, a special character, and a capital letter")]
        public string CurrentPassword { get; set; }
        
        [Required, RegularExpression(pattern:"^.*(?=.{8,})(?=.*[a-zA-Z])(?=.*\\d)(?=.*[!@#$%&? \"]).*$", ErrorMessage = "Minimum 8 characters, must contain a digit, a special character, and a capital letter")]
        public string NewPassword { get; set; }
        
        [Required, RegularExpression(pattern:"^.*(?=.{8,})(?=.*[a-zA-Z])(?=.*\\d)(?=.*[!@#$%&? \"]).*$", ErrorMessage = "Minimum 8 characters, must contain a digit, a special character, and a capital letter")]
        public string ConfirmNewPassword { get; set; }
    }
}
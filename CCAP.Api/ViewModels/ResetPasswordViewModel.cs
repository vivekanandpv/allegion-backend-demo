using System.ComponentModel.DataAnnotations;

namespace CCAP.Api.ViewModels {
    public class ResetPasswordViewModel {
        [Required, EmailAddress]
        public string Username { get; set; }

        [Required]
        public string ResetKey { get; set; }
        
        [Required, RegularExpression(pattern:"^.*(?=.{8,})(?=.*[a-zA-Z])(?=.*\\d)(?=.*[!@#$%&? \"]).*$", ErrorMessage = "Minimum 8 characters, must contain a digit, a special character, and a capital letter")]
        public string NewPassword { get; set; }
        
        [Required, RegularExpression(pattern:"^.*(?=.{8,})(?=.*[a-zA-Z])(?=.*\\d)(?=.*[!@#$%&? \"]).*$", ErrorMessage = "Minimum 8 characters, must contain a digit, a special character, and a capital letter")]
        public string ConfirmNewPassword { get; set; }
    }
}
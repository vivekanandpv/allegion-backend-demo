using System.Threading.Tasks;
using CCAP.Api.Services;
using CCAP.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCAP.Api.Controllers {
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenViewModel>> Login(LoginViewModel viewModel) {
            return Ok(await _authService.Login(viewModel));
        }

        
        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser(GeneralUserRegisterViewModel viewModel) {
            await _authService.Register(viewModel);
            return Ok();
        }
        
        // [Authorize(Policy = "Admin")]
        [HttpPost("register-staff")]
        public async Task<IActionResult> RegisterUser(StaffUserRegisterViewModel viewModel) {
            await _authService.Register(viewModel);
            return Ok();
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel) {
            await _authService.ChangePassword(viewModel);
            return Ok();
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("reset-user/{username}")]
        public async Task<ActionResult<ResetKeyViewModel>> ResetUser(string username) {
            return Ok(await _authService.ResetForUser(username));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel) {
            await _authService.ResetPassword(viewModel);
            return Ok();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCAP.Api.Controllers {
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DummyController: ControllerBase {
        
        [AllowAnonymous]
        [HttpGet("visitor")]
        public IActionResult GreetGeneral() {
            return Ok("Good afternoon! Visitor");
        }
        
        
        [HttpGet("user")]
        public IActionResult GreetUser() {
            return Ok("Good afternoon! User");
        }
        
        
        [HttpGet("approver")]
        public IActionResult GreetApprover() {
            return Ok("Good afternoon! Approver");
        }
    }
}
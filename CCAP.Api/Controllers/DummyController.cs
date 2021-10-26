using Microsoft.AspNetCore.Mvc;

namespace CCAP.Api.Controllers {
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DummyController: ControllerBase {
        
        [HttpGet]
        public IActionResult Greet() {
            return Ok("Good afternoon!");
        }
    }
}
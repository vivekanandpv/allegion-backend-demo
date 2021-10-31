using System.Threading.Tasks;
using CCAP.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCAP.Api.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CardsController : ControllerBase {
        private readonly ICreditCardService _service;

        public CardsController(ICreditCardService service) {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {
            return Ok(await _service.Get());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id) {
            return Ok(await _service.Get(id));
        }
    }
}
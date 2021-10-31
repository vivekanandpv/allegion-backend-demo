using System.Collections.Generic;
using System.Threading.Tasks;
using CCAP.Api.Services;
using CCAP.Api.Utils;
using CCAP.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCAP.Api.Controllers {
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ApplicationsController : ControllerBase {
        private readonly ICreditCardApplicationService _service;

        public ApplicationsController(ICreditCardApplicationService service) {
            _service = service;
        }

        [Authorize(Policy = StaticProvider.UserPolicy)]
        [HttpPost("register")]
        public async Task<IActionResult> Register(CreditCardApplicationCreateViewModel viewModel) {
            await _service.Register(viewModel);
            return Ok();
        }

        [Authorize(Policy = StaticProvider.StaffPolicy)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CreditCardApplicationReportViewModel>> Get(int id) {
            return Ok(await _service.GetApplication(id));
        }

        [Authorize(Policy = StaticProvider.ApproverPolicy)]
        [HttpGet("get-approver-queue")]
        public async Task<ActionResult<IEnumerable<CreditCardApplicationViewModel>>> GetPendingApproval() {
            return Ok(await _service.GetPendingApproval());
        }

        [Authorize(Policy = StaticProvider.IssuerPolicy)]
        [HttpGet("get-issuer-queue")]
        public async Task<ActionResult<IEnumerable<CreditCardApplicationViewModel>>> GetPendingIssuance() {
            return Ok(await _service.GetPendingIssuance());
        }

        [Authorize(Policy = StaticProvider.ApproverPolicy)]
        [HttpPost("approve")]
        public async Task<IActionResult> Approve(CreditCardApplicationStatusChangeViewModel viewModel) {
            await _service.Approve(viewModel);
            return Ok();
        }

        [Authorize(Policy = StaticProvider.ApproverPolicy)]
        [HttpPost("reject")]
        public async Task<IActionResult> Reject(CreditCardApplicationStatusChangeViewModel viewModel) {
            await _service.Reject(viewModel);
            return Ok();
        }

        [Authorize(Policy = StaticProvider.IssuerPolicy)]
        [HttpPost("issue")]
        public async Task<IActionResult> Issue(CreditCardApplicationStatusChangeViewModel viewModel) {
            await _service.IssueCard(viewModel);
            return Ok();
        }

        [Authorize(Policy = StaticProvider.ApproverPolicy)]
        [HttpGet("get-approver-statistics")]
        public async Task<ActionResult<ApproverStatisticsViewModel>> GetApproverStatistics() {
            return Ok(await _service.GetApproverStatistics());
        }

        [Authorize(Policy = StaticProvider.IssuerPolicy)]
        [HttpGet("get-issuer-statistics")]
        public async Task<ActionResult<IssuerStaticsViewModel>> GetIssuerStatistics() {
            return Ok(await _service.GetIssuerStatistics());
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using CCAP.Api.ViewModels;

namespace CCAP.Api.Services {
    public interface ICreditCardApplicationService {
        //  for customer
        Task Register(CreditCardApplicationCreateViewModel viewModel);
        
        //  for staff
        Task<List<CreditCardApplicationViewModel>> GetPendingApproval();
        Task<List<CreditCardApplicationViewModel>> GetPendingIssuance();
        Task<CreditCardApplicationReportViewModel> GetApplication(int id);
        Task Approve(CreditCardApplicationStatusChangeViewModel viewModel);
        Task Reject(CreditCardApplicationStatusChangeViewModel viewModel);
        Task IssueCard(CreditCardApplicationStatusChangeViewModel viewModel);
        
        //  for statistics
        Task<int> GetTotalApplications();
        Task<int> GetTotalPendingApproval();
        Task<int> GetTotalPendingIssuance();
        Task<int> GetTotalApproved();
        Task<int> GetTotalRejected();
        Task<int> GetTotalIssued();
    }
}
namespace CCAP.Api.ViewModels {
    public class ApproverStatisticsViewModel {
        public int TotalApplications { get; set; }
        public int TotalApproved { get; set; }
        public int TotalRejected { get; set; }
        public int TotalPendingApproval { get; set; }
    }
}
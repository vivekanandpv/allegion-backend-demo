namespace CCAP.Api.Services {
    public class CreditCardDisplayViewModel {
        public int Id { get; set; }
        public string Category { get; set; }
        public string SubType { get; set; }
        public string CardCode { get; set; }
        public string ImageUrl { get; set; }
        public int MinimumCreditScore { get; set; }
        public int MinimumLimit { get; set; }
        public int MaximumLimit { get; set; }
        public int MinimumAnnualIncome { get; set; }
    }
}
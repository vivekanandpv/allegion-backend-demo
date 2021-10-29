using System;
using CCAP.Api.Models;

namespace CCAP.Api.ViewModels {
    public class CreditCardApplicationViewModel {
        public int Id { get; set; }
        public DateTime DateOfApplication { get; set; }
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public int CreditCardId { get; set; }
        public string CardCode { get; set; }
        public int LimitRequired { get; set; }
        public string CardDescription { get; set; }
        public string ImageUrl { get; set; }
        public StatusType Status { get; set; }
    }
}
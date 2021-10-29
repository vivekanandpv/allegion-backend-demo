using System;
using CCAP.Api.Models;

namespace CCAP.Api.ViewModels {
    public class CreditCardApplicationReportViewModel {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string MiddleName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string PAN { get; set; }
        public string PhoneNumber { get; set; }
        public string SecondaryPhoneNumber { get; set; }
        public string Qualification { get; set; }
        public int AnnualIncome { get; set; }
        public string EmploymentStatus { get; set; }
        
        public int ApplicationId { get; set; }
        public DateTime DateOfApplication { get; set; }
        public int CreditCardId { get; set; }
        public string CreditCardCode { get; set; }
        public string CreditCardDetails { get; set; }
        public int LimitRequired { get; set; }

        
        //  For communication
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int PIN { get; set; }
    }
}
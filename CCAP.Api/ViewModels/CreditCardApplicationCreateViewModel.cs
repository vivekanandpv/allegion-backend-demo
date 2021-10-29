using System;
using System.ComponentModel.DataAnnotations;
using CCAP.Api.Models;

namespace CCAP.Api.ViewModels {
    public class CreditCardApplicationCreateViewModel {
        //  profile
        [RegularExpression("^[A-Z ]{1, 50}$")]
        public string MiddleName { get; set; }
        
        [Required, DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public Gender Gender { get; set; }
        
        [Required, RegularExpression("^[A-Z]{5}[0-9]{4}[A-Z]{1}$")]
        public string PAN { get; set; }
        
        [Required, RegularExpression("^[6-9]{1}[0-9]{9}$")]
        public string PhoneNumber { get; set; }
        
        [RegularExpression("^[6-9]{1}[0-9]{9}$")]
        public string SecondaryPhoneNumber { get; set; }
        
        [MinLength(2), MaxLength(50)]
        public string Qualification { get; set; }
        
        //  application
        [Required, Range(50_000, 100_00_00_000)]
        public int AnnualIncome { get; set; }
        
        [Required, MinLength(3), MaxLength(50)]
        public string EmploymentStatus { get; set; }
        
        [Required, Range(1, 1_000_000)]
        public int CreditCardId { get; set; }

        [Required, Range(10_000, 25_00_000)]
        public int LimitRequired { get; set; }
        
        //  user
        [Required,EmailAddress]
        public string Username { get; set; }
        
        //  address
        [Required, MinLength(5), MaxLength(100)]
        public string PAddressLine1 { get; set; }
        
        [MinLength(5), MaxLength(100)]
        public string PAddressLine2 { get; set; }
        
        [MinLength(5), MaxLength(100)]
        public string PAddressLine3 { get; set; }
        
        [Required, MinLength(3), MaxLength(100)]
        public string PCity { get; set; }
        
        [Required, MinLength(3), MaxLength(100)]
        public string PState { get; set; }
        
        [Required, Range(100_000, 999_999)]
        public int PPIN { get; set; }

        public bool UsePermanentAddressForCommunication { get; set; }
        
        [MinLength(5), MaxLength(100)]
        public string CAddressLine1 { get; set; }
        
        [MinLength(5), MaxLength(100)]
        public string CAddressLine2 { get; set; }
        
        [MinLength(5), MaxLength(100)]
        public string CAddressLine3 { get; set; }
        
        [MinLength(3), MaxLength(100)]
        public string CCity { get; set; }
        
        [MinLength(3), MaxLength(100)]
        public string CState { get; set; }
        
        [Range(100_000, 999_999)]
        public int CPIN { get; set; }
    }
}
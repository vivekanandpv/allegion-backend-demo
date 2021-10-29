using System;
using System.Collections;
using System.Collections.Generic;

namespace CCAP.Api.Models {
    public class AppUser {
        public int Id { get; set; }
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsStaff { get; set; }
        public bool IsForcedToResetPassword { get; set; }
        public int NWrongAttempts { get; set; }
        public bool IsLocked { get; set; }
        
        //  newly added to the entity
        public string MiddleName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string PAN { get; set; }
        public string PhoneNumber { get; set; }
        public string SecondaryPhoneNumber { get; set; }
        public string Qualification { get; set; }

        public IList<Address> Addresses { get; set; } = new List<Address>();
        public IList<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

        public IList<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();
        public IList<ApplicationStatus> StatusList { get; set; } = new List<ApplicationStatus>();
    }

    public class AppRole {
        public int Id { get; set; }
        public string RoleName { get; set; }

        public IList<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();
    }

    public class AppUserRole {
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int AppRoleId { get; set; }
        public AppRole AppRole { get; set; }
    }
    
    //  business domain
    public class BankAccount {
        public int Id { get; set; }
        public long AccountNumber { get; set; }
        public string Branch { get; set; }
        public string BranchCode { get; set; }
        public double Balance { get; set; }
        
        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
    }
    
    public class CreditCardApplication {
        public int Id { get; set; }
        public DateTime DateOfApplication { get; set; }
        public int AnnualIncome { get; set; }
        public string EmploymentStatus { get; set; }
        public CreditCard CreditCard { get; set; }
        public int CreditCardId { get; set; }
        public int LimitRequired { get; set; }

        public IList<ApplicationStatus> StatusList { get; set; } = new List<ApplicationStatus>();
    }

    public class ApplicationStatus {
        public int CreditCardApplicationId { get; set; }
        public CreditCardApplication CreditCardApplication { get; set; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public DateTime DateOfProcessing { get; set; }
        public StatusType Status { get; set; }  //  submitted, approved, rejected, issued
        public string Remarks { get; set; }
    }

    public class Address {
        public int Id { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int PIN { get; set; }
        public bool IsPermanent { get; set; }
        public bool UseForCommunication { get; set; }

        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
    }

    public class CreditCard {
        public int Id { get; set; }
        public string Category { get; set; }
        public string SubType { get; set; }
        public string CardCode { get; set; }
        public string ImageUrl { get; set; }
        public int MinimumCreditScore { get; set; }
        public int MinimumLimit { get; set; }
        public int MaximumLimit { get; set; }
        public int MinimumAnnualIncome { get; set; }

        public IList<CreditCardApplication> Applications { get; set; } = new List<CreditCardApplication>();
    }
    
    public enum StatusType {
        Submitted,
        Approved,
        Rejected,
        Issued
    }

    public enum Gender {
        Female,
        Male,
        Others
    }

}
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

        public IList<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();
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
}
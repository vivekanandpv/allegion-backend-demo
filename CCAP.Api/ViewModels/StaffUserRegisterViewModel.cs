using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CCAP.Api.ViewModels {
    public class StaffUserRegisterViewModel : GeneralUserRegisterViewModel {
        [Required]
        public string[] Roles { get; set; }
    }
}
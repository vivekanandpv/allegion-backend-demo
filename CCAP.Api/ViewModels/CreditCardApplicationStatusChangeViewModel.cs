using System;
using System.ComponentModel.DataAnnotations;
using CCAP.Api.Models;

namespace CCAP.Api.ViewModels {
    public class CreditCardApplicationStatusChangeViewModel {
        [Required]
        public int CreditCardApplicationId { get; set; }
        [Required]
        public string Username { get; set; }
        [MaxLength(100)]
        public string Remarks { get; set; }
    }
}
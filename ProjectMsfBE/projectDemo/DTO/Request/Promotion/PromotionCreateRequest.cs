using System;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request.Promotion
{
    public class PromotionCreateRequest
    {
        [Required]
        public string Code { get; set; }

        public string? Description { get; set; }

        public decimal DiscountValue { get; set; }
        public decimal? AmountLimit { get; set; }

        public decimal? DiscountAmount { get; set; }

        public string DiscountType { get; set; } // Percentage or FixedAmount

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public int? UsageLimit { get; set; }
    }
}

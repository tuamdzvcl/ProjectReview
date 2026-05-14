using System;

namespace projectDemo.DTO.Response.Promotion
{
    public class PromotionResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public bool IsSystem { get; set; }
        public string Description { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? AmountLimit { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string DiscountType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTick.Model.asbtract;
using projectDemo.AttributeConfig;
using projectDemo.Common;
using projectDemo.BaseInit.Excel;

namespace projectDemo.Entity.Models
{
    [Table("Promotions")]
    public class Promotion : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthStatus, ErrorMessage = "quá số kí tự rồi")]
        public string Code { get; set; }

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthStatus, ErrorMessage = "quá số kí tự rồi")]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountValue { get; set; }
        public int ? ReservedQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AmountLimit { get; set; }

        public bool IsSystem { get; set; }

        [StringLength(20)]
        public string DiscountType { get; set; } // Percentage or FixedAmount

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public int? UsageLimit { get; set; }

        public int UsedCount { get; set; }

        public virtual ICollection<UserPromotion> UserPromotions { get; set; }
    }
}

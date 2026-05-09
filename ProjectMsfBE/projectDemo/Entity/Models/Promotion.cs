using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTick.Model.asbtract;
using projectDemo.Common;

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

        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        [StringLength(20)]
        public string DiscountType { get; set; } // Percentage or FixedAmount

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public int? UsageLimit { get; set; }

        public int UsedCount { get; set; }
    }
}

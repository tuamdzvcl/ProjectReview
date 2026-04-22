using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTick.Model.asbtract;

namespace projectDemo.Entity.Models
{
    [Table("Promotions")]
    public class Promotion : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(255)]
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

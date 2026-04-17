using EventTick.Model.asbtract;
using EventTick.Model.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectDemo.Entity.Models
{
    [Table("UserUpgrade")]
    public class UserUpgrade : Auditable 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public int UpgradeId { get; set; }
        [ForeignKey("UpgradeId")]
        public virtual Upgrade Upgrade { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int CurrentDayUsageCount { get; set; } // Số lượng đã tạo trong ngày
        public DateTime? LastUsageDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePaid { get; set; }            // Số tiền thực tế đã trả (sau khấu trừ)

        [MaxLength(50)]
        public string Status { get; set; }
    }
}

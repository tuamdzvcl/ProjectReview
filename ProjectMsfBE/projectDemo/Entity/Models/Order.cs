using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventTick.Model.asbtract;
using EventTick.Model.Enum;
using projectDemo.Entity.Models;

namespace EventTick.Model.Models
{
    [Table("Orders")]
    public class Order : Auditable
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderCode { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public EnumStatusOrder Status { get; set; }
        
        public string OrderType { get; set; } 

        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public Guid? UserUpgradeId { get; set; }
        [ForeignKey("UserUpgradeId")]
        public virtual UserUpgrade UserUpgrade { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public Payment Payment { get; set; }
    }
}

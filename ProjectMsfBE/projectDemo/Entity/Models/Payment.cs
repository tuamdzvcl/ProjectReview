using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventTick.Model.asbtract;
using EventTick.Model.Enum;

namespace EventTick.Model.Models
{
    [Table("Payment")]
    public class Payment : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string PaymentMethod { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(100)]
        public string TransactionCode { get; set; } = null!;

        public string RequestId { get; set; }

        [Required]
        public EnumStatusPayment Status { get; set; }

        public DateTime? PaidDate { get; set; }

        public Guid OrderID { get; set; }

        [ForeignKey("OrderID")]
        public Order Orders { get; set; }
    }
}

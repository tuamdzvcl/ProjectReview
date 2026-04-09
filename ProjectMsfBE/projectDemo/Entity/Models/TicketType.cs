using EventTick.Model.asbtract;
using EventTick.Model.Enum;
using projectDemo.Entity.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTick.Model.Models
{
    [Table("TicketType")]
    public class TicketType : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string Name { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int TotalQuantity { get; set; }

        [Required]
        public int SoldQuantity { get; set; }

        public int ReservedQuantity { get; set; }

        [Required]
        public EnumStatusTickType Status { get; set; }

        public Guid EventID { get; set; }
        [ForeignKey("EventID")]
        public virtual Event Event { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }



    }
}

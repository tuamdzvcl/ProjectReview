using EventTick.Model.asbtract;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTick.Model.Models
{
    [Table("OrderDetail")]
    public class OrderDetail 
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        //[Required]
        //[Column(TypeName = "decimal(18,2)")]
        //public decimal SubTotal { get; set; }

        public Guid OrderID { get; set; }
        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }
        public int TicketTypeId { get; set; }
        [ForeignKey("TicketTypeId")]
        public virtual TicketType TicketTypes { get; set; }

        public virtual ICollection<Ticket> Ticket { get; set; }
    }
}

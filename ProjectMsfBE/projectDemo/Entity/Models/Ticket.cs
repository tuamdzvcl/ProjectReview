using EventTick.Model.asbtract;
using EventTick.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTick.Model.Models
{
    [Table("Ticket")]
    public class Ticket : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {  get; set; }
        

        [Required]
        [MaxLength(100)]
        public string TicketCode { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string QRCode { get; set; } = null!;

        [Required]
        public EnumStatusTick Status { get; set; }

        public DateTime? CheckInDate { get; set; }

        public Guid OrderDetailID { get; set; }
        [ForeignKey("OrderDetailID")]
        public virtual OrderDetail OrderDetail { get; set; }
    }
}

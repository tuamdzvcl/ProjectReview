using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventTick.Model.asbtract;
using EventTick.Model.Enum;
using projectDemo.Common;

namespace EventTick.Model.Models
{
    [Table("Ticket")]
    public class Ticket : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]

        public string TicketCode { get; set; } = null!;

        [Required]
        public string QRCode { get; set; } = null!;

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthStatus)]

        public EnumStatusTick Status { get; set; }

        public DateTime? CheckInDate { get; set; }

        public Guid OrderDetailID { get; set; }

        [ForeignKey("OrderDetailID")]
        public virtual OrderDetail OrderDetail { get; set; }
    }
}

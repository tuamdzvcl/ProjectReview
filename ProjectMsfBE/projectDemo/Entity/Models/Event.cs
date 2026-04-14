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
    [Table("Event")]
    public class Event : Auditable
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        [MaxLength(255)]
        public string Location { get; set; } = null!;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? SaleStartDate { get; set; }

        public DateTime? SaleEndDate { get; set; }

        [Required]
        public string PosterUrl { get; set; }

        [Required]
        public EnumStatusEvent Status { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        public Guid CatetoryID { get; set; }

        [ForeignKey("CatetoryID")]
        public virtual Catetory Catetory { get; set; }

        public virtual ICollection<TicketType> TicketTypes { get; set; }
    }
}

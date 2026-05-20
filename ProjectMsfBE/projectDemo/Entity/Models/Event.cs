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
using projectDemo.Entity.Models;

namespace EventTick.Model.Models
{
    [Table("Event")]
    public class Event : Auditable
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(ConfigValidation.MaxLength,ErrorMessage ="Không được vượt quá {0} kí tự")]
        [MinLength(ConfigValidation.MinLength,ErrorMessage ="Không được ít hơn {0} kí tự")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        [MaxLength(255)]
        public string Location { get; set; } = null!;

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public DateTimeOffset? SaleStartDate { get; set; }

        public DateTimeOffset? SaleEndDate { get; set; }

        public bool? Isfalse { get; set; }

        [Required]
        public string PosterUrl { get; set; }

        [Required]
        [MaxLength(ConfigValidation.MaxLengthStatus,ErrorMessage ="Không vượt quá {0} kí tự")]
        public string Status { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        public Guid CatetoryID { get; set; }

        [ForeignKey("CatetoryID")]
        public virtual Catetory Catetory { get; set; }
        public virtual ICollection<UserEventFavorite> UserEventFvorites { get; set; }
        public virtual ICollection<TicketType> TicketTypes { get; set; }

        public string? Reason { get; set; }
    }
}

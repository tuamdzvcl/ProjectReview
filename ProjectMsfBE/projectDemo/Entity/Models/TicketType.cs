using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventTick.Model.asbtract;
using EventTick.Model.Enum;
using projectDemo.AttributeConfig;
using projectDemo.Common;
using projectDemo.Entity.Enum;

namespace EventTick.Model.Models
{
    [Table("TicketType")]
    public class TicketType : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthStatus)]

        public string Name { get; set; }

        [Required(ErrorMessage ="không được để trống")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [TextValidation(1, 3,ErrorMessage ="Không được vượt quá số cho phép")]
        public int TotalQuantity { get; set; }

        [TextValidation(1, 3, ErrorMessage = "Không được vượt quá số cho phép")]
        public int SoldQuantity { get; set; }

        public int ReservedQuantity { get; set; }

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthStatus)]

        public EnumStatusTickType Status { get; set; }

        public Guid EventID { get; set; }

        [ForeignKey("EventID")]
        public virtual Event Event { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

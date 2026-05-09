using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTick.Model.Models;
using projectDemo.Common;

namespace projectDemo.Entity.Models
{
    [Table("Catetorys")]
    public class Catetory
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        [MaxLength(ConfigValidation.MaxLength, ErrorMessage = "Không được vượt quá khí tự cho phép")]
        [MinLength(ConfigValidation.MinLength, ErrorMessage = "Không được ít hơn {0} kí tự")]
        public string Name { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}

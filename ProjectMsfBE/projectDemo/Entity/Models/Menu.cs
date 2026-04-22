using EventTick.Model.asbtract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectDemo.Entity.Models
{
    [Table("Menu")]
    public class Menu :Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Path { get; set; }
        public int ParentId { get; set; }
        [Required]
        public int OrderIndex { get; set; }
        public virtual ICollection<MenuPermissions> MenuPermissions { get; set; }
    }
}

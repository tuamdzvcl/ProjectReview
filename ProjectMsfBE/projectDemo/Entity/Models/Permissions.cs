using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTick.Model.asbtract;

namespace projectDemo.Entity.Models
{
    [Table("Permissions")]
    public class Permissions : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PermissonsName { get; set; }

        [Required]
        [StringLength(255)]
        public string PermissonsDescription { get; set; }

        public virtual ICollection<RolePermissions> RolePermissions { get; set; }
    }
}

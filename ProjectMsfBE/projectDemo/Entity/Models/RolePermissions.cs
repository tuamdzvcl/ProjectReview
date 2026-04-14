using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTick.Model.asbtract;
using EventTick.Model.Models;

namespace projectDemo.Entity.Models
{
    [Table("RolePermissions")]
    public class RolePermissions : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id;

        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        public int PermissionId { get; set; }

        [ForeignKey("PermissionId")]
        public virtual Permissions Permissions { get; set; }
    }
}

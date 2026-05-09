using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTick.Model.asbtract;
using projectDemo.Common;

namespace projectDemo.Entity.Models
{
    [Table("Permissions")]
    public class Permissions : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthStatus, ErrorMessage = "quá số kí tự rồi")]

        public string PermissonsName { get; set; }

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthStatus, ErrorMessage = "quá số kí tự rồi")]
        public string PermissonsDescription { get; set; }

        public virtual ICollection<RolePermissions> RolePermissions { get; set; }
        public virtual ICollection<MenuPermissions> MenuPermissions { get; set; }
    }
}

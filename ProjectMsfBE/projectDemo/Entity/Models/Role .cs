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
    [Table("Role")]
    public class Role : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string RoleName { get; set; }

        public virtual ICollection<UserRole> UserRole { get; set; }

        public virtual ICollection<RolePermissions> RolePermissions { get; set; }
    }
}

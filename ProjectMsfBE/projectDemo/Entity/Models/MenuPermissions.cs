using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectDemo.Entity.Models
{
    [Table("MenuPermissions")]
    public class MenuPermissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MenuId { get; set; }
        [ForeignKey("MenuId")]
        public virtual Menu Menu { get; set; }
        public int PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public virtual Permissions Permission { get; set; }

    }
}

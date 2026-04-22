using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTick.Model.asbtract;

namespace projectDemo.Entity.Models
{
    [Table("Setting")]
    public class SystemSetting : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string SettingKey { get; set; }

        [Required]
        public string SettingValue { get; set; }

        [StringLength(255)]
        public string Description { get; set; }
    }
}

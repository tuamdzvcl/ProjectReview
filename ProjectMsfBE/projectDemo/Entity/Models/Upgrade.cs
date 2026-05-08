using EventTick.Model.asbtract;
using EventTick.Model.Models;
using projectDemo.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectDemo.Entity.Models
{
    [Table("Upgrade")]
    public class Upgrade : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]

        public string TitleUpgrade {  get; set; }

        public string Description {  get; set; }

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthStatus)]

        public string status { get; set; }

        public int DailyLimit { get; set; } // Giới hạn tạo event mỗi ngày
        public bool IsDailyPackage { get; set; }
        [Required]        
        
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public virtual ICollection<UserUpgrade> UserUpgrades { get; set; } = new List<UserUpgrade>();


    }
}

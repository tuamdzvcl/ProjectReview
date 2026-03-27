using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EventTick.Model.Models;
using EventTick.Model.asbtract;

namespace projectDemo.Entity.Models
{
    [Table("ApiLog")]
    public class ApiLog :Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Service { get; set; }
        [MaxLength(50)]
        public string FuncName { get; set; }
        [MaxLength(10)]
        public string? IpAddress { get; set; }
        public string Username { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}

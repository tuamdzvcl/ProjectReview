using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTick.Model.asbtract;
using EventTick.Model.Models;

namespace projectDemo.Entity.Models
{
    [Table("ApiLog")]
    public class ApiLog : Auditable
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

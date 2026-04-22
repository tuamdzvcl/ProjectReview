using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTick.Model.asbtract;

namespace projectDemo.Entity.Models
{
    [Table("EmailSettings")]
    public class EmailSetting : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string SmtpServer { get; set; }

        public int Port { get; set; }

        [Required]
        [StringLength(100)]
        public string SenderEmail { get; set; }

        [StringLength(100)]
        public string SenderName { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        public bool EnableSsl { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectDemo.Entity.Models
{
    [Table("AuditLog")]
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid? UserId { get; set; }

        [MaxLength(100)]
        public string? Username { get; set; }

        [Required]
        [MaxLength(20)]
        public string Method { get; set; } // GET, POST, PUT, DELETE

        [Required]
        public string Path { get; set; } // /api/events

        public int StatusCode { get; set; } // 200, 401, 500

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(1000)]
        public string? Note { get; set; } // Action description
    }
}

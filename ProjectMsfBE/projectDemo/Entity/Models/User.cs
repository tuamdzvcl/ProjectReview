using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventTick.Model.asbtract;
using projectDemo.Entity.Models;

namespace EventTick.Model.Models
{
    [Table("User")]
    public class User : Auditable
    {
        [Key]
        public Guid Id { get; set; } // Dùng Guid thì kiểu dữ liệu này dổi đi

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Username { get; set; }

        [MaxLength(500)]
        public string? PasswordHash { get; set; } = null!;

        [Required]
        public bool IsLock { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = null!;

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        [MaxLength(500)]
        public string? BackGroupUrl { get; set; }

        public DateTime? DateLock { get; set; }
        public int? Isfalse { get; set; }

        public virtual ICollection<UserLogin> UserLogins { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<ApiLog> ApiLogs { get; set; }

        public virtual ICollection<UserUpgrade> UserUpgrades { get; set; }
    }
}

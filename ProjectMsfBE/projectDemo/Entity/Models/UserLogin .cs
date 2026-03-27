using EventTick.Model.asbtract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTick.Model.Models
{
    [Table("UserLogin")]
    public class UserLogin : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
       
        
        [MaxLength(50)]
        public string? Provider { get; set; } = null!;

        [MaxLength(255)]
        public string? ProviderUserId { get; set; } = null!;

        
        

        // Navigation
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User user { get; set; }
    }
}

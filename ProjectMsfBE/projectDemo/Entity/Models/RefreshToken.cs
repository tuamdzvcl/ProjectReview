using EventTick.Model.Models;
using projectDemo.AttributeConfig;
using projectDemo.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectDemo.Entity.Models
{
    public class RefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthStatus, ErrorMessage = "quá số kí tự rồi")]

        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }



        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public bool IsActive => !IsRevoked && ExpiryDate > DateTime.UtcNow;
    }
}

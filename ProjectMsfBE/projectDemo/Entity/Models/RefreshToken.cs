using EventTick.Model.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectDemo.Entity.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }



        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public bool IsActive => !IsRevoked && ExpiryDate > DateTime.UtcNow;
    }
}

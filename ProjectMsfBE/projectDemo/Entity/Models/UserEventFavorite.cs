using EventTick.Model.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectDemo.Entity.Models
{
    [Table("UserEventFavorite")]   
    public class UserEventFavorite
    {
        
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        public DateTime CreateDt { get; set; }
    }
}

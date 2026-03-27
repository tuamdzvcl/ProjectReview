using EventTick.Model.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectDemo.Entity.Models
{
    [Table("Catetorys")]
    public class Catetory
    {
        [Key]
        public Guid Id { get; set; }
        public string Name{  get; set; }

        public virtual ICollection<Event> Events { get; set; } 
    }
}

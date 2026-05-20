using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTick.Model.asbtract
{
    public abstract class Auditable : IAuditableInterface1
    {
        public DateTimeOffset? CreatedDate { get; set; }

        [MaxLength(255)]
        public string? CreatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }

        [MaxLength(255)]
        public string? UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; } = false;
    }
}

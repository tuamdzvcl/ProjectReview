using projectDemo.Common.PageRequest;

namespace projectDemo.DTO.Request.Promotion
{
    public class PromotionQuery : PageRequest
    {
        public bool? Status { get; set; }
    }
}

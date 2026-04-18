using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTick.Model.Enum;
using EventTick.Model.Models;

namespace projectDemo.DTO.Response
{
    public class OrderResponse
    {
        public Guid OrderID { get; set; }
        public string OrderCode { get; set; }
        public decimal TotalAmount { get; set; }
        public String Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public string FullName { get; set; }
        public string EventName { get; set; }
        public string EventLocation { get; set; }
        public string EventPosterUrl { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public List<OrderDetailResponse> orderDetails { get; set; }
    }
}

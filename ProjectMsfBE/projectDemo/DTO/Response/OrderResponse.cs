using EventTick.Model.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EventTick.Model.Models;

namespace projectDemo.DTO.Response
{
    public class OrderResponse
    {
        public Guid OrderID { get; set; }
        public string OrderCode { get; set; }
        public decimal TotalAmount { get; set; }
        public String Status { get; set; }
        
        public string FullName { get; set; }
        public List<OrderDetailResponse> orderDetails { get; set; }
    }
}

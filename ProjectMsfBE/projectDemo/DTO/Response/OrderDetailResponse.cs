using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Response
{
    public class OrderDetailResponse
    {
        public Guid OrderIdDetail { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal SubTotal { get; set; }

        public string TicketTypeName  { get; set; }

    }
}

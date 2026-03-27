using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projectDemo.config;
using projectDemo.DTO.Request;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Service.OrderService;

namespace projectDemo.Controllers
{
    [Authorize]
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            var result = await _orderService.CreateOrder(request, userId);
            return Ok(result);
        }

        [Permission("USER_CREATE")]
        [HttpGet()]
        public async Task<IActionResult> GetOrder()
        {
            var result = await _orderService.GetOrder();
            return Ok(result);
        }

       
        [HttpGet("user/{userId}")]

        public async Task<IActionResult> GetListOrderByUser(
            [FromQuery] int pageIndex ,
            [FromQuery] int pageSize )
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            var result = await _orderService.GetListOrderbyIdUser(userId,pageIndex,pageSize);
            return Ok(result);
        }

       
        [HttpGet("{orderId}/detail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderDetail(Guid orderId)
        {
            var result = await _orderService.GetListOrderDetail(orderId);
            return Ok(result);
        }

        
        [HttpPut("{orderId}")]
        [AllowAnonymous]

        public async Task<IActionResult> UpdateOrder(Guid orderId, [FromBody] OrderUpdate request)
        {
            var result = await _orderService.UpdateOrder(orderId, request);
            return Ok(result);
        }

       
        [HttpDelete("{orderId}")]
        [AllowAnonymous]

        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            var result = await _orderService.DeleteOrder(orderId);
            return Ok(result);
        }
    }

}

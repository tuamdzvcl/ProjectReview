using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projectDemo.DTO.Request;
using projectDemo.DTO.Response.Momo;
using projectDemo.Service.CatetoryService;
using projectDemo.Service.MomoService;
using projectDemo.Service.OrderService;
using System.Text.Json;

namespace projectDemo.Controllers
{
    [Route("api/momo")]
    [ApiController]
    public class MomoController : ControllerBase
    {
        private readonly IMomoService _momoService;
        private readonly IOrderService _orderService;


        public MomoController(IMomoService momoService, IOrderService orderService)
        {
            _momoService = momoService;
            _orderService = orderService;
        }

        [HttpGet("CallBack")]
        public async Task<IActionResult> CallBack([FromQuery] MomoIpnRequest model)
        {
            var sign = _momoService.IsValidMomoIpnSignature(model);
            if (!sign)
                return BadRequest("lượn nhanh");
            var result =await _momoService.MomoCallBack(model);
            return Redirect(result);
        }
      
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projectDemo.DTO.Request;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Service.TicketTypeService;

namespace projectDemo.Controllers
{
    [Authorize]
    [Route("api/TypeTicket")]
    [ApiController]
    public class TypeTicketController : ControllerBase
    {
        private readonly ITypeTicketService _typeTicketService;

        public TypeTicketController(ITypeTicketService typeTicketService)
        {
            _typeTicketService = typeTicketService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllEvent(Guid id)
        {
            var result = await _typeTicketService.GetTypeTickByEevntID(id);
            return Ok(result);
        }
        [HttpPost()]
        public async Task<IActionResult> Create(TypeTicketRequest request)
        {
            var result = await _typeTicketService.CreateTypeTickect(request);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,TypeTicketUpdate request)
        {
            var result = await _typeTicketService.UpdateTypeTicket(id,request);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _typeTicketService.DeleteTypeTicket(id);
            return Ok(result);
        }



    }
}

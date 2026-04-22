using System.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectDemo.Common.PageRequest;
using projectDemo.DTO.Request;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Service.Auth;
using projectDemo.Service.EventService;
using projectDemo.Service.TicketTypeService;

namespace projectDemo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/event")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ITypeTicketService _typeTicketService;

        public EventController(IEventService eventService, ITypeTicketService ticketService)
        {
            _eventService = eventService;
            _typeTicketService = ticketService;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateEvent(
            [FromForm] CreateEventWithTicketTypesRequest resquest
        )
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            var result = await _eventService.CreateEventWithTicketTypes(resquest, userId);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(
            Guid id,
            [FromForm] EventUpdateRequest resquest
        )
        {
            var result = await _eventService.UpdateEvent(id, resquest);
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateEventStatus(
            Guid id,
            [FromBody] EventStatusUpdateRequest request
        )
        {
            var result = await _eventService.UpdateEventStatus(id, request);
            return Ok(result);
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllEvent()
        {
            var result = await _eventService.GetEventAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventbyId(Guid id)
        {
            var result = await _eventService.GetEventById(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteId(Guid id)
        {
            var isAdmin = User.Identity.IsAuthenticated && User.IsInRole("ADMIN");
            var result = await _eventService.DeleteEvent(id, isAdmin);
            return Ok(result);
        }

        [HttpPost("{id}/duplicate")]
        public async Task<IActionResult> DuplicateEvent(Guid id)
        {
            var result = await _eventService.DuplicateEvent(id);
            return Ok(result);
        }

        [HttpGet("page")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPageEvent(
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            [FromQuery] string? key
        )
        {
            var result = await _eventService.GetListEventPage(pageSize, pageIndex, key);
            return Ok(result);
        }

        [HttpGet("typetick/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllEvent(Guid id)
        {
            var result = await _typeTicketService.GetListTypeTickByEventID(id);
            return Ok(result);
        }

        [HttpGet("page-with-ticket-types")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPageWithTicketTypes([FromQuery] PageRequest query)
        {
            var isAdmin = User.Identity.IsAuthenticated && User.IsInRole("ADMIN");
            var result = await _eventService.GetPageWithTicketTypes(query, isAdmin);
            return Ok(result);
        }

        [HttpGet("page-with-ticket-types-byid")]
        public async Task<IActionResult> GetPageWithTicketTypesbyId([FromQuery] PageRequest query)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            var result = await _eventService.GetPageWithTicketTypesbyId(userId, query);
            return Ok(result);
        }
    }
}

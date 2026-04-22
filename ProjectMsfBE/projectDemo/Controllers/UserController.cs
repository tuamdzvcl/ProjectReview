using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projectDemo.config;
using projectDemo.DTO.Request;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Service.UserService;
using System.Security.Claims;

namespace projectDemo.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = Guid.Parse(User.FindFirstValue("id") ?? "Null");
            var result = await _userService.GetByid(userId);
            return Ok(result);
        }
        [HttpGet("events")]
        public async Task<IActionResult> GetAllEvent()
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            var result = await _userService.GetListEventByUserID(userId);
            return Ok(result);
        }
        [HttpGet("events/{userId}")]
        public async Task<IActionResult> GetEventByUserId(Guid userId)
        {
            var result = await _userService.GetListEventByUserIDCreate(userId);
            return Ok(result);
        }

        [HttpGet("page/user")]
        public async Task<IActionResult> GetallUser([FromQuery] UserQuery query)
        {
            var result = await _userService.GetAll(query);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue("id") ?? "Null");
            var result = await _userService.Delete(id);
            return Ok(result);
        }

        [HttpPost()]
        public async Task<IActionResult> Add(UserRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue("id") ?? "Null");
            var result = await _userService.Create(request, userId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UserUpdateRequest request)
        {
            var result = await _userService.Update(id, request);
            return Ok(result);
        }

        [HttpGet("participants")]
        public async Task<IActionResult> GetParticipantsByOrganizer([FromQuery] projectDemo.Common.PageRequest.PageRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue("id")??"Null");
            var result = await _userService.GetParticipantsByOrganizer(userId, request);
            return Ok(result);
        }

        [HttpPut("avatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            var userId = Guid.Parse(User.FindFirstValue("id") ?? "Null");
            var result = await _userService.UpdateAvatarAsync(userId, file);
            return Ok(result);
        }
        
    }
}

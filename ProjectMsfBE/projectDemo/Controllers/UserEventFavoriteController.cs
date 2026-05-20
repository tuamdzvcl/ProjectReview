using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectDemo.Service.UserEventFavoriteService;
using System.Security.Claims;

namespace projectDemo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserEventFavoriteController : ControllerBase
    {
        private readonly IUserEventFavoriteService _favoriteService;

        public UserEventFavoriteController(IUserEventFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("toggle/{eventId}")]
        public async Task<IActionResult> ToggleFavorite(Guid eventId)
        {
            var userId = Guid.Parse(User.FindFirstValue("id") ?? "Null");
            
            var result = await _favoriteService.ToggleFavoriteAsync(userId, eventId);
            return Ok(result);
        }

        [HttpGet("my-favorites")]
        public async Task<IActionResult> GetMyFavorites()
        {
            var userId = Guid.Parse(User.FindFirstValue("id") ?? "Null");
            
            var result = await _favoriteService.GetUserFavoriteIdsAsync(userId);
            return Ok(result);
        }
    }
}

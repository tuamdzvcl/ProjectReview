using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Service.Auth;
using projectDemo.Service.AuthService;

namespace projectDemo.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly GoogleAuthService _googleAuthService;
        private readonly IMemoryCache _cache;

        public AuthController(
            IAuthService authService,
            GoogleAuthService googleAuthService,
            IMemoryCache cache
        )
        {
            _authService = authService;
            _googleAuthService = googleAuthService;
            _cache = cache;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.AuthenCase(request);

            return Ok(result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest resquest)
        {
            var result = await _authService.Regiter(resquest);

            return Ok(result);
        }

        [HttpGet("google-login-url")]
        public IActionResult GetGoogleLoginUrl()
        {
            var url = _googleAuthService.GetLoginUrl();
            return Ok(url);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code)
        {
            var result = await _googleAuthService.HandleGoogleCallback(code);

            var key = Guid.NewGuid().ToString();

            _cache.Set(key, result, TimeSpan.FromMinutes(5));

            var frontendUrl = $"http://localhost:4200/login-success?key={key}";
            return Redirect(frontendUrl);
        }

        [HttpPost("google-result")]
        [AllowAnonymous]
        public IActionResult GetGoogleResult([FromBody] KeyResquest resquest)
        {
            var key = resquest.key;
            if (!_cache.TryGetValue(key, out var data))
            {
                return BadRequest("Key expired or invalid");
            }

            // xóa sau khi dùng (one-time)
            _cache.Remove(key);

            return Ok(data);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authService.ForgotPasswordAsync(request.Email);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request);
            if (result.StatusCode == Entity.Enum.EnumStatusCode.SUCCESS)
                return Ok(result);

            return BadRequest(result);
        }
    }
}

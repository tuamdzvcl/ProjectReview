using EventTick.Model.Models;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;

namespace projectDemo.Service.Auth
{
    public interface IAuthService
    {
         string GenerateRefreshToken();
        Task<ApiResponse<LoginResponse>> AuthenCase(LoginRequest resquest);
        Task<ApiResponse<UserResponse>> Regiter(RegisterRequest resquest);

        string GenerateToken(User user, List<PermissionResponse> perResponse);
    }
}

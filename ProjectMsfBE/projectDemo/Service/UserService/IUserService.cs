using Microsoft.AspNetCore.Http;
using projectDemo.DTO.Request;
using projectDemo.DTO.Request.Upgrade;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.DTO.UpdateRequest;

namespace projectDemo.Service.UserService
{
    public interface IUserService
    {
        Task<ApiResponse<UserEventsResponse>> GetListEventByUserID(Guid guid);
        Task<ApiResponse<UserProfile>> GetListEventByUserIDCreate(Guid guid);
        Task<ApiResponse<UserResponse>> Create(UserRequest request, Guid userid);
        Task<ApiResponse<UserResponse>> Update(Guid id, UserUpdateRequest request);
        Task<ApiResponse<string>> Delete(Guid id);
        Task<PageResponse<UserResponse>> GetAll(UserQuery param);
        Task<ApiResponse<UserResponse>> GetByid(Guid id);
        Task<PageResponse<UserInEvent>> GetParticipantsByOrganizer(Guid organizerId, projectDemo.Common.PageRequest.PageRequest request);
        Task<ApiResponse<string>> UpdateAvatarAsync(Guid userId, IFormFile file);
    }
}

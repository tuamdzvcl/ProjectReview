using projectDemo.DTO.Request;
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
        Task<ApiResponse<UserResponse>> Update(Guid id,UserUpdateRequest request);
        Task<ApiResponse<string>> Delete(Guid id);
        Task<PageResponse<UserResponse>> GetAll(UserQuery param);

    }
}

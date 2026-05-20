using projectDemo.DTO.Respone;

namespace projectDemo.Service.UserEventFavoriteService
{
    public interface IUserEventFavoriteService
    {
        Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, Guid eventId);
        Task<ApiResponse<List<Guid>>> GetUserFavoriteIdsAsync(Guid userId);
    }
}

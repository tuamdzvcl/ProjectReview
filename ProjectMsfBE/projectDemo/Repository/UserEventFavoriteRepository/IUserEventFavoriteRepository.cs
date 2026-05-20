using projectDemo.Entity.Models;

namespace projectDemo.Repository.Ipml
{
    public interface IUserEventFavoriteRepository
    {
        Task<bool> AddFavoriteAsync(UserEventFavorite entity);
        Task<bool> RemoveFavoriteAsync(Guid userId, Guid eventId);
        Task<List<UserEventFavorite>> GetFavoritesByUserIdAsync(Guid userId);
        Task<bool> IsFavoriteAsync(Guid userId, Guid eventId);
    }
}

using projectDemo.Entity.Models;

namespace projectDemo.Repository.RefreshRepository
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refresh);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId);
        Task RevokeAllByUserIdAsync(Guid userId);
    }
}

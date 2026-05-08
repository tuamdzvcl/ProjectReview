using Microsoft.EntityFrameworkCore;
using projectDemo.Entity.Models;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.RefreshRepository
{
    public class RefreshTokenRepository : RepositoryLinqBase<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IUnitOfWork uow)
            : base(uow)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbSet.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(rt => rt.UserId == userId && rt.IsActive)
                .ToListAsync();
        }

        public async Task RevokeAllByUserIdAsync(Guid userId)
        {
            var activeTokens = await GetActiveTokensByUserIdAsync(userId);

            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
            }
        }
    }
}

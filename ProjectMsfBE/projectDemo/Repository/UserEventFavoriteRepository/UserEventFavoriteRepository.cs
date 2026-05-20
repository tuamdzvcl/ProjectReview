using Microsoft.EntityFrameworkCore;
using projectDemo.Entity.Models;
using projectDemo.Repository.BaseData;
using projectDemo.Repository.Ipml;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository
{
    public class UserEventFavoriteRepository : RepositoryLinqBase<UserEventFavorite>, IUserEventFavoriteRepository
    {
        public UserEventFavoriteRepository(IUnitOfWork uow) : base(uow)
        {
        }

        public async Task<bool> AddFavoriteAsync(UserEventFavorite entity)
        {
            var exists = await IsFavoriteAsync(entity.UserId, entity.EventId);
            if (exists) return true;

            await _dbSet.AddAsync(entity);
            return true;
        }

        public async Task<List<UserEventFavorite>> GetFavoritesByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Event)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreateDt)
                .ToListAsync();
        }

        public async Task<bool> IsFavoriteAsync(Guid userId, Guid eventId)
        {
            return await _dbSet.AnyAsync(x => x.UserId == userId && x.EventId == eventId);
        }

        public async Task<bool> RemoveFavoriteAsync(Guid userId, Guid eventId)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId && x.EventId == eventId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                return true;
            }
            return false;
        }
    }
}

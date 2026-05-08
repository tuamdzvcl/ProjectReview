using Microsoft.EntityFrameworkCore;
using projectDemo.Entity.Models;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.EmailVerificationTokenRepository
{
    public class EmailVerificationTokenRepository
        : RepositoryLinqBase<EmailVerificationToken>, IEmailVerificationTokenRepository
    {
        public EmailVerificationTokenRepository(IUnitOfWork uow)
            : base(uow)
        {
        }

        public async Task<EmailVerificationToken?> GetByTokenAsync(string token)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task RemoveUnusedByUserIdAsync(Guid userId)
        {
            var unusedTokens = await _dbSet
                .Where(t => t.UserId == userId && !t.IsUsed)
                .ToListAsync();

            _dbSet.RemoveRange(unusedTokens);
        }

        public new async Task AddAsync(EmailVerificationToken entity)
        {
            await _dbSet.AddAsync(entity);
        }
    }
}

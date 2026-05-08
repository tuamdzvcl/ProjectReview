using projectDemo.Entity.Models;

namespace projectDemo.Repository.EmailVerificationTokenRepository
{
    public interface IEmailVerificationTokenRepository
    {
        Task<EmailVerificationToken?> GetByTokenAsync(string token);
        Task RemoveUnusedByUserIdAsync(Guid userId);
        Task AddAsync(EmailVerificationToken entity);
    }
}

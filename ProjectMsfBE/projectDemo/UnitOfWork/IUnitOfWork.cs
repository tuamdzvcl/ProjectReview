using Microsoft.EntityFrameworkCore;
using System.Data;

namespace projectDemo.UnitOfWorks
{
    public interface IUnitOfWork 
    {
        DbContext context {  get; }
        IDbConnection connection { get; }

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveChangesAsync();
        IDbTransaction GetTransaction();
    }
}

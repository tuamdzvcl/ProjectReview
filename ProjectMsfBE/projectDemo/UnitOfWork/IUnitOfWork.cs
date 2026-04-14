using System.Data;
using Microsoft.EntityFrameworkCore;

namespace projectDemo.UnitOfWorks
{
    public interface IUnitOfWork
    {
        DbContext context { get; }
        IDbConnection connection { get; }

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveChangesAsync();
        IDbTransaction GetTransaction();
    }
}

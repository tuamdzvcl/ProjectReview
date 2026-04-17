using Microsoft.EntityFrameworkCore;
using projectDemo.Data;

namespace projectDemo.Repository.Reset
{
    public class RestRepository
    {
        private readonly EventTickDbContext _dbContext;

        public RestRepository(EventTickDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task RestData()
        {
            await _dbContext.Database.ExecuteSqlRawAsync(@"
            EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'
            EXEC sp_MSforeachtable 'DELETE FROM ?'
            EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'
        ");

            // Reset identity (ID về 1)
            await _dbContext.Database.ExecuteSqlRawAsync(@"
            DBCC CHECKIDENT ('Ticket', RESEED, 0)
            DBCC CHECKIDENT ('Payment', RESEED, 0)
            DBCC CHECKIDENT ('UserLogin', RESEED, 0)
            DBCC CHECKIDENT ('ApiLog', RESEED, 0)
            DBCC CHECKIDENT ('AuditLog', RESEED, 0)
            DBCC CHECKIDENT ('Permissions', RESEED, 0)
            DBCC CHECKIDENT ('Role', RESEED, 0)
            

            
        ");
        }
    }
}

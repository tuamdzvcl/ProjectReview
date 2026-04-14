using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using projectDemo.Data;
using projectDemo.UnitOfWorks;

namespace projectDemo.UnitOfWork
{
    public class UnitOfWorkk : IUnitOfWork
    {
        private readonly EventTickDbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWorkk(EventTickDbContext context)
        {
            _context = context;
        }

        public DbContext context => _context;

        public IDbConnection connection => _context.Database.GetDbConnection();

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        public IDbTransaction GetTransaction()
        {
            return _transaction?.GetDbTransaction();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

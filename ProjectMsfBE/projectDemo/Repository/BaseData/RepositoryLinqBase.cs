using Microsoft.EntityFrameworkCore;
using projectDemo.UnitOfWorks;
using System.Linq.Expressions;

namespace projectDemo.Repository.BaseData
{
    public class RepositoryLinqBase<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly IUnitOfWork _uow;

        public RepositoryLinqBase(IUnitOfWork uow)
        {
            _dbContext = uow.context;
            _dbSet = _dbContext.Set<TEntity>();
            _uow = uow;
        }
        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(List<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void RemoveRange(List<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

       

       
    }
}


using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.PaymentRepository
{
    public class PaymentRepository : RepositoryLinqBase<Payment>, IPaymentRepository
    {
        private readonly RepositoryProcBase _proc;
        public PaymentRepository(IUnitOfWork uow) : base(uow)
        {
            _proc = new RepositoryProcBase(_uow);
        }

        public async Task<Payment> Create(Payment entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public void Delete(Payment entity)
        {
             _dbSet.Remove(entity);
        }

        public async Task<List<Payment>> FinAll()
        {
           return await _dbSet.ToListAsync();
        }

        public async Task<Payment> FinById(int Id)
        {
            return await _dbSet.FindAsync(Id);
        }

        public async Task<Payment?> FindByOrderId(Guid orderId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.OrderID == orderId);
        }

        public void Update(Payment entity)
        {
            _dbSet.Update(entity);
        }
    }
}

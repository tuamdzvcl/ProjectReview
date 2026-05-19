using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.OrderRepository
{
    public class OrderDetailRepository : RepositoryLinqBase<OrderDetail>, IOrderDetailRepository
    {
        private readonly RepositoryProcBase _proc;

        public OrderDetailRepository(IUnitOfWork uow)
            : base(uow)
        {
            _proc = new RepositoryProcBase(uow);
        }

        public async Task<OrderDetail> Createorderdetail(OrderDetail orderDetail)
        {
            await AddAsync(orderDetail);
            return orderDetail;
        }

        public int DeleteOrderdetail(OrderDetail orderDetail)
        {
            Remove(orderDetail);
            return 1;
        }

        public async Task<List<OrderDetail>> GetlistdetailbyOderId(Guid orderId)
        {
            return await _dbSet.Where(x => x.OrderID == orderId).ToListAsync();
        }

        public int UpdateOrderdetail(OrderDetail orderDetail)
        {
            Update(orderDetail);
            return 1;
        }

        Task<OrderDetail> IOrderDetailRepository.GetlistdetailbyOderId(Guid orderId)
        {
            throw new NotImplementedException();
        }
    }
}

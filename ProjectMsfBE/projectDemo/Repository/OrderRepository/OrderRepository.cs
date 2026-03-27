using Dapper;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using projectDemo.DTO.Projection;
using projectDemo.DTO.Response;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;
using System.Data;

namespace projectDemo.Repository.OrderRepository
{
    public class OrderRepository : RepositoryLinqBase<Order>, IOrderRepository
    {
        private readonly RepositoryProcBase _proc;

        public OrderRepository(IUnitOfWork uow) : base(uow)
        {
            _proc = new RepositoryProcBase(uow);

        }

        public async Task<Order> CreateOrder(Order order)
        {
           await AddAsync(order);
            return order;
        }

        
        public  int DeleteOrder(Order order)
        {
            Remove(order);
             _uow.SaveChangesAsync();

            return 1;
        }

        public async Task<List<Order>> GetALl()
        {
            var order = await _dbSet.Where(x=>x.IsDeleted==false).ToListAsync();
            return order;
        }

        public async Task<(List<Guid> Orders, int TotalCount)> GetListOrderByUserId(Guid userId, int pageNumber, int pageSize)
        {
            var query = Find(x => x.UserID == userId).AsNoTracking();

            var total = await query.CountAsync();

            var orders = await query
                .OrderByDescending(x => x.CreatedDate)
                .Where(x=>x.IsDeleted == false)
                .Select(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, total);
        }
        public async Task<Order?> GetOrderbyID(Guid orderID)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == orderID && x.IsDeleted == false);
        }

        public async Task<(OrderProjection?,int statuss, string messager)> GetOrderListOrderDetail(Guid orderID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@orderID", orderID);
                param.Add("@code", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@messger", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                using var multi = await _uow.connection.QueryMultipleAsync(
                     "GetListdetaiByOrderID",
                     param,
                     commandType: CommandType.StoredProcedure
                     );
                var order = await multi.ReadFirstOrDefaultAsync<OrderProjection>();

                var orderDetail = (await multi.ReadAsync<OrderDetialProjection>()).ToList();


                if (order != null)
                {
                    order.orderDetails = orderDetail;
                }

                var status = param.Get<int>("@code");
                var message = param.Get<string>("@messger");

                return (order, status, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (null, 404, ex.Message);
            }
        }

        public int UpdateOrder(Order order)
        {
            Update(order);
            _uow.SaveChangesAsync();
            return 1;
        }


       
    }
}

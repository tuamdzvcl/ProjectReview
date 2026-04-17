using System.Data;
using Dapper;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using projectDemo.DTO.Projection;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.OrderRepository
{
    public class OrderRepository : RepositoryLinqBase<Order>, IOrderRepository
    {
        private readonly RepositoryProcBase _proc;

        public OrderRepository(IUnitOfWork uow)
            : base(uow)
        {
            _proc = new RepositoryProcBase(uow);
        }

        public async Task<Order> CreateOrder(Order order)
        {
            await AddAsync(order);
            return order;
        }

        public int DeleteOrder(Order order)
        {
            Remove(order);
            return 1;
        }

        public async Task<List<Order>> GetALl()
        {
            var order = await _dbSet.Where(x => x.IsDeleted == false).ToListAsync();
            return order;
        }

        public async Task<bool> HasOrderByUserId(Guid userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.UserID == userId && o.IsDeleted == false)
                .AnyAsync();
        }

        public async Task<Order?> GetOrderbyID(Guid orderID)
        {
            return await _dbSet
                .Include(x => x.OrderDetails)
                    .ThenInclude(x => x.TicketTypes)
                    .Include(o => o.OrderDetails)
    .ThenInclude(od => od.Ticket)
                .Include(x => x.Payment)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == orderID && x.IsDeleted == false);
        }

        /// <summary>
        /// Read-only query cho email: load đầy đủ Event + Ticket info, không tracking.
        /// </summary>
        public async Task<Order?> GetOrderForEmailAsync(Guid orderID)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.OrderDetails)
                    .ThenInclude(x => x.TicketTypes)
                        .ThenInclude(x => x.Event)
                .Include(x => x.OrderDetails)
                    .ThenInclude(x => x.Ticket)
                .FirstOrDefaultAsync(x => x.Id == orderID && x.IsDeleted == false);
        }

        public async Task<(OrderProjection?, int statuss, string messager)> GetOrderListOrderDetail(
            Guid orderID
        )
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@orderID", orderID);
                param.Add("@code", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add(
                    "@messger",
                    dbType: DbType.String,
                    size: 250,
                    direction: ParameterDirection.Output
                );

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
            return 1;
        }
    }
}

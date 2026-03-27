using EventTick.Model.Models;
using projectDemo.DTO.Projection;
using projectDemo.DTO.Response;

namespace projectDemo.Repository.OrderRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetALl();
         Task<Order> CreateOrder(Order order);
         int UpdateOrder(Order order);
        int DeleteOrder(Order order);
        Task<Order> GetOrderbyID(Guid orderID);

        Task<(List<Guid> Orders, int TotalCount)> GetListOrderByUserId(Guid userId, int pageNumber, int pageSize);

        Task<(OrderProjection?, int statuss, string messager)> GetOrderListOrderDetail(Guid orderID);

    }
}

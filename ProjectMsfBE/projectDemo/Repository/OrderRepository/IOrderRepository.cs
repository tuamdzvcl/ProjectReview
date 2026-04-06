using EventTick.Model.Models;
using projectDemo.DTO.Projection;

namespace projectDemo.Repository.OrderRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetALl();
         Task<Order> CreateOrder(Order order);
         int UpdateOrder(Order order);
        int DeleteOrder(Order order);
        Task<Order> GetOrderbyID(Guid orderID);

        Task<bool> HasOrderByUserId(Guid userId);

        Task<(OrderProjection?, int statuss, string messager)> GetOrderListOrderDetail(Guid orderID);

    }
}

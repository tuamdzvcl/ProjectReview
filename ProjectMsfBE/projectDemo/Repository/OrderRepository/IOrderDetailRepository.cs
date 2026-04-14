using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace projectDemo.Repository.OrderRepository
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail> Createorderdetail(OrderDetail orderDetail);
        int UpdateOrderdetail(OrderDetail orderDetail);
        int DeleteOrderdetail(OrderDetail orderDetail);
    }
}

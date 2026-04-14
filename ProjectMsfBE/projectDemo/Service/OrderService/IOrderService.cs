using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.DTO.Response.Momo;
using projectDemo.DTO.UpdateRequest;

namespace projectDemo.Service.OrderService
{
    public interface IOrderService
    {
        Task<ApiResponse<MomoCreatePaymentResponseModel>> CreateOrder(
            CreateOrderRequest request,
            Guid userid
        );
        Task<ApiResponse<string>> UpdateOrder(Guid orderID, OrderUpdate request);
        Task<ApiResponse<string>> DeleteOrder(Guid OrderID);
        Task<ApiResponse<List<OrderResponse>>> GetOrder();
        Task<PageResponse<OrderEventResponse>> GetListOrderbyIdUser(
            Guid UserID,
            int pageindex,
            int pagesize
        );
        Task<ApiResponse<OrderResponse>> GetListOrderDetail(Guid OrderID);
    }
}

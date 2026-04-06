using projectDemo.DTO.Query;

namespace projectDemo.Repository.OrderQuery
{
    public interface IOrderQuery
    {
        Task<(List<OrderEventFlatRow> Items, int TotalCount)> GetListOrderByUserId(Guid userId, int pageNumber, int pageSize);
    }
}

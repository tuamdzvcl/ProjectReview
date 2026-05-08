using projectDemo.DTO.Query;

namespace projectDemo.Query.OrderQuery
{
    public interface IOrderQuery
    {
        Task<(List<OrderEventFlatRow> Items, int TotalCount)> GetListOrderByUserId(
            Guid userId,
            int pageNumber,
            int pageSize
        );
    }
}

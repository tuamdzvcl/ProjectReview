using Dapper;
using projectDemo.DTO.Query;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.OrderQuery
{
    public class OrderQuery : IOrderQuery
    {
        private readonly IUnitOfWork _uow;

        public OrderQuery(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<(List<OrderEventFlatRow> Items, int TotalCount)> GetListOrderByUserId(
            Guid userId,
            int pageNumber,
            int pageSize
        )
        {
            if (pageNumber <= 0)
                pageNumber = 1;

            if (pageSize <= 0)
                pageSize = 10;

            var skip = (pageNumber - 1) * pageSize;

            const string sql =
                @"
SELECT COUNT(1)
FROM Orders o
WHERE o.UserID = @userId
  AND o.IsDeleted = 0;

WITH PagedOrders AS
(
    SELECT o.Id, o.CreatedDate
    FROM Orders o
    WHERE o.UserID = @userId
      AND o.IsDeleted = 0
    ORDER BY o.CreatedDate DESC
    OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY
)
SELECT
    o.Id AS OrderId,
    o.OrderCode AS OrderCode,
    o.TotalAmount AS TotalAmount,
    o.CreatedDate AS CreatedDate,
    CAST(o.Status AS INT) AS Status,
    e.Id AS EventId,
    e.Title AS EventTitle,
    e.Description AS EventDescription,
    e.Location AS EventLocation,
    e.StartDate AS EventStartDate,
    e.EndDate AS EventEndDate,
    e.PosterUrl AS EventPosterUrl,
    tt.Id AS TicketTypeId,
    tt.Name AS TicketTypeName,
    od.Price AS TicketPrice,
    od.Quantity AS TicketQuantity,od.Id as OrderDetailId
FROM PagedOrders po
INNER JOIN Orders o ON o.Id = po.Id
LEFT JOIN OrderDetail od ON od.OrderID = o.Id
LEFT JOIN TicketType tt ON tt.Id = od.TicketTypeId AND tt.IsDeleted = 0
LEFT JOIN Event e ON e.Id = tt.EventID
ORDER BY po.CreatedDate DESC;";

            using var multi = await _uow.connection.QueryMultipleAsync(
                sql,
                new
                {
                    userId,
                    skip,
                    pageSize,
                }
            );

            var totalCount = await multi.ReadFirstAsync<int>();
            var items = (await multi.ReadAsync<OrderEventFlatRow>()).ToList();

            return (items, totalCount);
        }
    }
}

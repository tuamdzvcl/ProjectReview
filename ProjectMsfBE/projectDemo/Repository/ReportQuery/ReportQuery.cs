using Dapper;
using projectDemo.DTO.Query;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.ReportQuery
{
    public class ReportQuery : IReportQuery
    {
        private readonly IUnitOfWork _uow;

        public ReportQuery(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<RevenueReportFlatRow>> GetRevenueRowsAsync(
            Guid userId,
            DateTime fromDate,
            DateTime toDateExclusive
        )
        {
            const string sql =
                @"
SELECT
    o.Id AS OrderId,
    p.PaidDate AS PaidDate,
    (od.Price * od.Quantity) AS Revenue,
    od.Quantity AS TicketQuantity
FROM Orders o
INNER JOIN Payment p ON p.OrderID = o.Id
INNER JOIN OrderDetail od ON od.OrderID = o.Id
INNER JOIN TicketType tt ON tt.Id = od.TicketTypeId
INNER JOIN Event e ON e.Id = tt.EventID
WHERE e.UserID = @userId
  AND o.IsDeleted = 0
  AND tt.IsDeleted = 0
  AND o.Status = @paidOrderStatus
  AND p.Status = @successPaymentStatus
  AND p.PaidDate IS NOT NULL
  AND p.PaidDate >= @fromDate
  AND p.PaidDate < @toDateExclusive
ORDER BY p.PaidDate ASC;";

            var rows = await _uow.connection.QueryAsync<RevenueReportFlatRow>(
                sql,
                new
                {
                    userId,
                    fromDate,
                    toDateExclusive,
                    paidOrderStatus = 2,
                    successPaymentStatus = 2,
                }
            );

            return rows.ToList();
        }
    }
}

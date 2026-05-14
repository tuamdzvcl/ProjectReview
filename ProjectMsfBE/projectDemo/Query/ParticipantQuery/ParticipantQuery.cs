using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using projectDemo.DTO.Query;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.ParticipantQuery
{
    public class ParticipantQuery : IParticipantQuery
    {
        private readonly IUnitOfWork _uow;

        public ParticipantQuery(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<(
            List<ParticipantFlatRow> Items,
            int TotalCount
        )> GetParticipantsByOrganizerAsync(Guid organizerId, int pageIndex, int pageSize)
        {
            var skip = (pageIndex - 1) * pageSize;

            const string sql =
                @"
                SELECT COUNT(DISTINCT u.Id)
                FROM [User] u
                INNER JOIN Orders o ON o.UserID = u.Id
                INNER JOIN OrderDetail od ON od.OrderID = o.Id
                INNER JOIN TicketType tt ON tt.Id = od.TicketTypeId
                INNER JOIN Event e ON e.Id = tt.EventID
                WHERE e.UserID = @organizerId 
                  AND u.IsDeleted = 0 
                  AND o.IsDeleted = 0;

                SELECT 
                    u.Id, 
                    u.Email, 
                    u.Username, 
                    u.FirstName, 
                    u.LastName, 
                    u.AvatarUrl,
                    e.Id as EventID,
                    e.Title AS EventTitle,
                    tt.Name AS TicketName,
                   	tt.Price As TickPrice,
                     p.Amount As TotalAmount,
                    od.Quantity AS TicketQuantity
                FROM [User] u
                INNER JOIN Orders o ON o.UserID = u.Id
                INNER JOIN OrderDetail od ON od.OrderID = o.Id
                INNER JOIN TicketType tt ON tt.Id = od.TicketTypeId
                INNER JOIN Event e ON e.Id = tt.EventID
                INNER JOIN Payment p on p.OrderID=o.Id
                WHERE e.UserID = @organizerId 
                  AND u.IsDeleted = 0 
                  AND o.IsDeleted = 0
                   and p.Status='SUCCESS'
                ORDER BY o.Id DESC
                OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY;";

            using var multi = await _uow.connection.QueryMultipleAsync(
                sql,
                new
                {
                    organizerId,
                    skip,
                    pageSize,
                },
                transaction: _uow.GetTransaction()
            );

            var totalCount = await multi.ReadFirstAsync<int>();
            var items = (await multi.ReadAsync<ParticipantFlatRow>()).ToList();

            return (items, totalCount);
        }

        // 1205/2026-thay đổi
        public async Task<(
            List<ParticipantFlatRow> Items,
            int TotalCount
        )> GetParticipantsByEventAsync(Guid organizerId, Guid eventId, int pageIndex, int pageSize)
        {
            var skip = (pageIndex - 1) * pageSize;

            const string sql =
                @"
                SELECT COUNT(DISTINCT u.Id)
                FROM [User] u
                INNER JOIN Orders o ON o.UserID = u.Id
                INNER JOIN OrderDetail od ON od.OrderID = o.Id
                INNER JOIN TicketType tt ON tt.Id = od.TicketTypeId
                INNER JOIN Event e ON e.Id = tt.EventID
                WHERE e.UserID = @organizerId 
                  AND e.Id = @eventId
                  AND u.IsDeleted = 0 
                  AND o.IsDeleted = 0;
SELECT 
    u.Id, 
    u.Email, 
    u.Username, 
    u.FirstName, 
    u.LastName, 
    u.AvatarUrl,
    e.Id as EventID,
    e.Title AS EventTitle,
    tt.Name AS TicketName,
    tt.Price As TickPrice,
    p.Amount As TotalAmount,
    od.Quantity AS TicketQuantity
FROM [User] u
INNER JOIN Orders o ON o.UserID = u.Id
INNER JOIN OrderDetail od ON od.OrderID = o.Id
INNER JOIN TicketType tt ON tt.Id = od.TicketTypeId
INNER JOIN Event e ON e.Id = tt.EventID
INNER JOIN Payment p ON p.OrderID=o.Id
WHERE e.UserID = @organizerId 
  AND e.Id = @eventId
  AND u.IsDeleted = 0 
  AND o.IsDeleted = 0
  AND p.Status='SUCCESS'
ORDER BY o.Id DESC
OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY;";

            using var multi = await _uow.connection.QueryMultipleAsync(
                sql,
                new
                {
                    organizerId,
                    eventId,
                    skip,
                    pageSize,
                },
                transaction: _uow.GetTransaction()
            );

            var totalCount = await multi.ReadFirstAsync<int>();
            var items = (await multi.ReadAsync<ParticipantFlatRow>()).ToList();

            return (items, totalCount);
        }
    }
}

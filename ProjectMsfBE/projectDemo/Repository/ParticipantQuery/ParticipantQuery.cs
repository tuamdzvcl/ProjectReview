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
                    e.Title AS EventTitle,
                    tt.Name AS TicketName,
                    o.TotalAmount,
                    od.Quantity AS TicketQuantity
                FROM [User] u
                INNER JOIN Orders o ON o.UserID = u.Id
                INNER JOIN OrderDetail od ON od.OrderID = o.Id
                INNER JOIN TicketType tt ON tt.Id = od.TicketTypeId
                INNER JOIN Event e ON e.Id = tt.EventID
                WHERE e.UserID = @organizerId 
                  AND u.IsDeleted = 0 
                  AND o.IsDeleted = 0
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
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using projectDemo.DTO.Query;

namespace projectDemo.Repository.ParticipantQuery
{
    public interface IParticipantQuery
    {
        Task<(List<ParticipantFlatRow> Items, int TotalCount)> GetParticipantsByOrganizerAsync(Guid organizerId, int pageIndex, int pageSize);
        // 1205/2026-thay đổi
        Task<(List<ParticipantFlatRow> Items, int TotalCount)> GetParticipantsByEventAsync(Guid organizerId, Guid eventId, int pageIndex, int pageSize);
    }
}

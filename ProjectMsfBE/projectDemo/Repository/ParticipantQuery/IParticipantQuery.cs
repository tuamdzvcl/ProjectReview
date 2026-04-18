using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using projectDemo.DTO.Query;

namespace projectDemo.Repository.ParticipantQuery
{
    public interface IParticipantQuery
    {
        Task<(List<ParticipantFlatRow> Items, int TotalCount)> GetParticipantsByOrganizerAsync(Guid organizerId, int pageIndex, int pageSize);
    }
}

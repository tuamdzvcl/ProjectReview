using System.Data;
using Dapper;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Entity.Models;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.CatetoryRepository
{
    public class CatetoryRepository : RepositoryLinqBase<Catetory>, ICatetoryReposioty
    {
        private readonly RepositoryProcBase _proc;

        public CatetoryRepository(IUnitOfWork uow)
            : base(uow)
        {
            _proc = new RepositoryProcBase(uow);
        }

        public async Task<Catetory> Create(Catetory entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<string> delete(Guid id)
        {
            var events = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
            _dbSet.Remove(events);

            return "xóa thành công";
        }

        public async Task<Catetory?> GetbyId(Guid id)
        {
            return await _dbSet.Include(x => x.Events).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Catetory?> GetByName(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<List<Catetory>> GetListCatetory()
        {
            return await _dbSet.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<PageResponse<CatetoryResponse>> PageCatetoryEvent(
            int PageSize,
            int PageIndex,
            string? key
        )
        {
            var param = new DynamicParameters();

            param.Add("@PageIndex", PageIndex);
            param.Add("@PageSize", PageSize);
            param.Add("@key", key);
            param.Add("@totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var data = await _uow.connection.QueryAsync<CatetoryEventRaw>(
                "PageCatetoryEvent",
                param,
                transaction: _uow.GetTransaction(),
                commandType: CommandType.StoredProcedure
            );

            var totalRow = param.Get<int>("@totalRow");

            var result = data.GroupBy(x => new { x.CatetoryId, x.CatetoryName })
                .Select(g => new CatetoryResponse
                {
                    Name = g.Key.CatetoryName,
                    listEvent = g.Select(e => new EventResponse
                        {
                            EventID = e.EventID,
                            Title = e.Title,
                            Description = e.Description,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate,
                            Location = e.Location,
                            PosterUrl = e.PosterUrl,
                            Status = e.EventStatus.ToString(),
                        })
                        .ToList(),
                })
                .ToList();

            return new PageResponse<CatetoryResponse>
            {
                Items = result,
                TotalRecords = totalRow,
                PageIndex = PageIndex,
                PageSize = PageSize,
                Success = true,
            };
        }
    }
}

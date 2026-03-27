using AutoMapper;
using Dapper;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using projectDemo.Common.PageRequest;
using AutoMapper.QueryableExtensions;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Repository.BaseData;
using projectDemo.Repository.Ipml;
using projectDemo.UnitOfWorks;
using System.Data;
using System.Management;

namespace projectDemo.Repository
{
    public class EventRepository :RepositoryLinqBase<Event>, IEventRepository

    {
        private readonly RepositoryProcBase _proc;
        private readonly IMapper _mapper;


        public EventRepository(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _proc = new RepositoryProcBase(uow);
            _mapper = mapper;

        }
        //add event
        public async Task CreateEvent(Event entity)
        {
            await _dbSet.AddAsync(entity);
        }
        //delete evnt
        public void DeleteEvent(Event entity)
        {
            _dbSet.Remove(entity);
        }
        //get event
        public async Task<List<Event>> GetAllEvent()
        {
            return await _dbSet
                .Where(e => e.Status != EnumStatusEvent.CANNEL && e.IsDeleted==false)
                .AsNoTracking()
                .ToListAsync();
        }
        //get event anh typeticj
        public async Task<PageResponse<EventTypeTickResponses>> GetAllWithTicketTypesAsync(PageRequest request)
        {
            var query =  _dbSet
                .AsNoTracking()
                .Where(e => e.IsDeleted==false && e.Status != EnumStatusEvent.CANNEL);
            if (!string.IsNullOrWhiteSpace(request.key))
            {
                var key = request.key.Trim();
                query = query
                    .Where(e =>
                    e.Title.Contains(key) ||
                    e.Location.Contains(key));

            }
            var totolRecords = await query.CountAsync();

            var items = await query
       .OrderByDescending(e => e.CreatedDate)
       .Skip((request.PageIndex - 1) * request.PageSize)
       .Take(request.PageSize)
       .ProjectTo<EventTypeTickResponses>(_mapper.ConfigurationProvider)
       .ToListAsync();

            return new PageResponse<EventTypeTickResponses>
            {
                Items = items,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalRecords = totolRecords,
                TotalPages = (int)Math.Ceiling((double)totolRecords / request.PageSize),
                Success = true,
                Message = "Lấy danh sách event thành công"
            };

        }        //get Event
        public async Task<Event> GetEventById(Guid eventId)
        {
            
            try
            {
                return await _dbSet
                .FirstOrDefaultAsync(e => e.Id == eventId && e.Status != EnumStatusEvent.CANNEL)
                ?? new Event();
                 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Event();
            }
        }

        //get event tyoeticket--> done
        public async Task<EventTypeTickResponses?> GetEventDetailById(Guid eventId)
        {
            return await _dbSet
         .AsNoTracking()
         .Where(e => e.Id == eventId && e.IsDeleted ==false)
         .Select(e => new EventTypeTickResponses
         {
             Id = e.Id,
             Title = e.Title,
             Description = e.Description,
             Location = e.Location,
             StartDate = e.StartDate,
             EndDate = e.EndDate,
             SaleStartDate = e.SaleStartDate,
             SaleEndDate = e.SaleEndDate,
             PosterUrl = e.PosterUrl,
             Status = e.Status.ToString(),
             UserName = e.User.Username,
             CatetoryName = e.Catetory.Name,
             ListTypeTick = e.TicketTypes.Select(t => new TypeTickResponse
             {
                 Id = t.Id,
                 Name = t.Name.ToString(),
                 Price = t.Price,
                 TotalQuantity = t.TotalQuantity,
                 SoldQuantity = t.SoldQuantity,
                 Status = t.Status.ToString()
             }).ToList()
         })
         .FirstOrDefaultAsync();
        }

        //get page event->>done
        public async Task<PageResponse<EventResponse>> GetPageEvent(int pageIndex, int pageSize, string key)
        {
            var param = new DynamicParameters();
            param.Add("@PageIndex", pageIndex);
            param.Add("@PageSize", pageSize);
            param.Add("@key", key);
            param.Add("@totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var result = await _uow.connection.QueryAsync<EventResponse>(
                "GetEventPaging",
                param,
                transaction: _uow.GetTransaction(),
                commandType: CommandType.StoredProcedure);
            int totalRow = param.Get<int>("@totalRow");
            return new PageResponse<EventResponse>
            {
                Items = result.ToList(),
                TotalRecords = totalRow,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Success = true
                
            };
        }
        // update ->> thiếu update typetick
        public async Task<bool> UpdateEvent(Guid EventID, EventUpdateRequest request)
        {
           
            try
            {
                var param = new DynamicParameters();

                param.Add("@Id", EventID);
                param.Add("@Title", request.Title);
                param.Add("@Description", request.Description);
                param.Add("@Location", request.Location);
                param.Add("@StartDate", request.StartDate);
                param.Add("@EndDate", request.EndDate);
                param.Add("@SaleStartDate", request.SaleStartDate);
                param.Add("@SaleEndDate", request.SaleEndDate);
                param.Add("@PosterUrl", request.PosterUrl);
                param.Add("@Status", request.Status);
                param.Add("@RowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await _uow.connection.ExecuteAsync(
                   "UpdateEvent",
                   param,
                   transaction:_uow.GetTransaction(),
                   commandType: CommandType.StoredProcedure
               );
                var rows = param.Get<Int32>("@RowsAffected");
                return rows > 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"lỗi{ ex.ToString()}");
                return false;
            }
        }
      
    }
}

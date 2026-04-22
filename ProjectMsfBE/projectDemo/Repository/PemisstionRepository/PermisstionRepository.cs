using System.Data;
using System.Net.WebSockets;
using Dapper;
using Microsoft.EntityFrameworkCore;
using projectDemo.DTO.Projection;
using projectDemo.DTO.Request;
using projectDemo.DTO.Response;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Entity.Models;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.PemisstionRepository
{
    public class PermisstionRepository : RepositoryLinqBase<Permissions>, IPermisstionRepository
    {
        private readonly RepositoryProcBase _proc;

        public PermisstionRepository(IUnitOfWork uow)
            : base(uow)
        {
            _proc = new RepositoryProcBase(uow);
        }

        public async Task<Permissions> Create(Permissions entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public string Delete(int permissionID)
        {
            var update = _dbSet.FirstOrDefault(x => x.Id == permissionID);
            update.IsDeleted = true;
            return "deleted";
        }

        public async Task<Permissions?> GetByID(int permissionID)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x =>
                x.Id == permissionID && x.IsDeleted == false
            );
            return entity;
        }

        public async Task<IEnumerable<Permissions>> GetAllAsync()
        {
            return await _dbSet.Where(x => x.IsDeleted == false).ToListAsync();
        }

        public async Task<(RoleProjecttion, int status, string messger)> GetPermissionByRoleId(
            int RoleId
        )
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@roleID", RoleId);
                param.Add("@status", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add(
                    "@messges",
                    dbType: DbType.String,
                    size: 250,
                    direction: ParameterDirection.Output
                );
                using var multi = await _uow.connection.QueryMultipleAsync(
                    "GetListPerssionByRoleID",
                    param,
                    commandType: CommandType.StoredProcedure
                );
                var role = await multi.ReadFirstOrDefaultAsync<RoleProjecttion>();
                var per = (await multi.ReadAsync<PermissionProjection>()).ToList();
                if (role != null)
                {
                    role.ListPermissions = per;
                }
                var status = param.Get<int>("@status");
                var message = param.Get<string>("@messges");

                return (role, status, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (null, 404, ex.Message);
            }
        }

        
    }
}

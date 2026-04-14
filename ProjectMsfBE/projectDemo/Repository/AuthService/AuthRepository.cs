using System.Data;
using Dapper;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using projectDemo.Data;
using projectDemo.DTO.Response;
using projectDemo.Repository.BaseData;
using projectDemo.Repository.Ipml;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository
{
    public class AuthRepository : RepositoryLinqBase<User>, IAuthRepository
    {
        private readonly RepositoryProcBase _proc;

        public AuthRepository(IUnitOfWork uow)
            : base(uow)
        {
            _proc = new RepositoryProcBase(uow);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Guid> InsertAsync(User user)
        {
            _dbSet.Add(user);
            return user.Id;
        }

        public async Task<List<PermissionResponse>> GetPermissionNameAsyncByUserId(Guid UserID)
        {
            var param = new DynamicParameters();

            param.Add("@UserID", UserID);

            var result = await _uow.connection.QueryAsync<PermissionResponse>(
                "GetPermissonNameByid",
                param,
                commandType: CommandType.StoredProcedure
            );
            return result.ToList();
        }

        public async Task AddAsync()
        {
            await _dbSet.AddRangeAsync();
        }

        public async Task<List<PermissionResponse>> GetPermissionsbyRoleName(string roleName)
        {
            var param = new DynamicParameters();
            param.Add("@RoleName", roleName);
            param.Add("@status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            param.Add(
                "@messges",
                dbType: DbType.String,
                size: 250,
                direction: ParameterDirection.Output
            );

            var result = await _uow.connection.QueryAsync<PermissionResponse>(
                "GetListPerssionByRoleName",
                param,
                transaction: _uow.GetTransaction(),
                commandType: CommandType.StoredProcedure
            );
            var status = param.Get<int>("@status");
            var message = param.Get<string>("@messges");
            if (status != 200)
            {
                return null;
            }
            return result.ToList();
        }
    }
}

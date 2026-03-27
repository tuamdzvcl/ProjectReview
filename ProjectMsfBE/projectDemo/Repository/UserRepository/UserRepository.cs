using Dapper;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using projectDemo.Data;
using projectDemo.DTO.Request;
using projectDemo.DTO.Response;
using projectDemo.Repository.BaseData;
using projectDemo.Repository.Ipml;
using projectDemo.UnitOfWorks;
using System.Data;

namespace projectDemo.Repository
{
    public class UserRepository :  RepositoryLinqBase<User>,IUserReposiotry
    {
        private readonly RepositoryProcBase _proc;

        public UserRepository(IUnitOfWork uow): base(uow)
        {
            _proc = new RepositoryProcBase(uow);
        }

        public async Task<User> Create(User user)
        {
            await _dbSet.AddAsync(user);
            return user;
        }
        //xóa mềm 
        public string Delete(User user)
        {
            _dbSet.RemoveRange(user);
            return "Deleted";
        }
        //get ds event theo userid 
        public async Task<(List<Event>,int status,string messager)> GetListEventByUserID(Guid userID)
        {
            try 
            {
                var param = new DynamicParameters();
                param.Add("@userid", userID);
                param.Add("@status", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@messges", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);


                var listevent = await _uow.connection.QueryAsync<Event>(
                    "GetListEventByUserID",
                    param,
                    commandType:CommandType.StoredProcedure
                    );

                    var status = param.Get<int>("@status");
                     var messager = param.Get<string>("@messges");
                
                    return (listevent.ToList(), status, messager);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (new List<Event>(), 404, ex.Message);
            }
            
        }
        //get tên role by id
        public async Task<List<string>> GetRoleByUser(Guid Userid)
        {
            try
            {
                //gọi connection
                var param = new DynamicParameters();
                param.Add("@UserID", Userid);
                // gọi lệnh quey
                var result = await _uow.connection.QueryAsync<string>(
                    "GetRoleNameByUserID",
                    param,
                    transaction: _uow.GetTransaction(),
                    commandType: System.Data.CommandType.StoredProcedure
                    );
                return result.ToList(); ;
            }
            catch( Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (null);
            }
        }
        // thông tin user
        public async Task<User?> GetUserByid(Guid id)
        {
            return await _dbSet.Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted==false); ;
        }
        //update
        public User Update(User user)
        {
            _dbSet.Update(user);
            return user;
        }
        //10 bản ghi
        public async Task<(List<User>,int)> GetAll(int pageIndex, int pageSize, string key, string role)
        {
            var query = _dbSet
                .Include(u=>u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(x => x.IsDeleted==false)
                .AsQueryable();

            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(x => x.Username.Contains(key) ||x.Email.Contains(key));
            }
            if(!string.IsNullOrEmpty(role))
            {
                query = query.Where(x=>x.UserRoles.Any(ur=>ur.Role.RoleName== role));
            }    

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.CreatedDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, total);
        }

        
    }
}

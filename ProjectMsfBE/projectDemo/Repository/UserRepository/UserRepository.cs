using Dapper;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using projectDemo.Repository.BaseData;
using projectDemo.Repository.Ipml;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository
{
    public class UserRepository : RepositoryLinqBase<User>, IUserReposiotry
    {
        private readonly RepositoryProcBase _proc;

        public UserRepository(IUnitOfWork uow)
            : base(uow)
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
        public async Task<(
            User? user,
            List<Event> events,
            int status,
            string messager
        )> GetListEventByUserID(Guid userID)
        {
            try
            {
                var user = await _dbContext
                    .Set<User>()
                    .AsNoTracking()
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .Include(u =>
                        u.Events.Where(e =>
                            e.IsDeleted == false && e.Status != EnumStatusEvent.CANNEL
                        )
                    )
                    .FirstOrDefaultAsync(u => u.Id == userID && u.IsDeleted == false);

                if (user == null)
                {
                    return (null, new List<Event>(), 404, "Không tìm thấy user");
                }

                var events = user.Events.OrderByDescending(e => e.CreatedDate).ToList();

                return (user, events, 200, "Lấy danh sách event thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (null, new List<Event>(), 500, ex.Message);
            }
        }

        //get tên role by id
        public async Task<List<string>> GetRoleByUser(Guid Userid)
        {
            try
            {
                return await _dbSet
                    .Where(u => u.Id == Userid && u.IsDeleted == false)
                    .SelectMany(u => u.UserRoles)
                    .Select(ur => ur.Role.RoleName.ToUpper())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        // thông tin user
        public async Task<User?> GetUserByid(Guid id)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == false);
        }

        //update
        public User Update(User user)
        {
            _dbSet.Update(user);
            return user;
        }

        //10 bản ghi
        public async Task<(List<User>, int)> GetAll(
            int pageIndex,
            int pageSize,
            string key,
            string role
        )
        {
            var query = _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(x => x.IsDeleted == false && x.IsAdmin==false)
                .AsQueryable();

            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(x => x.Username.Contains(key) || x.Email.Contains(key));
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(x => x.UserRoles.Any(ur => ur.Role.RoleName == role));
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

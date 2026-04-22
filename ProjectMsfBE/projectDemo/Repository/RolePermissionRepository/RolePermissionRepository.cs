using Microsoft.EntityFrameworkCore;
using projectDemo.DTO.Request;
using projectDemo.DTO.Response;
using projectDemo.Entity.Models;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.RolePermissionRepository
{
    public class RolePermissionRepository
        : RepositoryLinqBase<RolePermissions>,
            IRolePermissionRepository
    {
        public readonly RepositoryProcBase _proc;

        public RolePermissionRepository(IUnitOfWork uow)
            : base(uow)
        {
            _proc = new RepositoryProcBase(uow);
        }

        public async Task<List<PermisstionRoleResponse>> GetListPermissionRole()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.Role.IsAdmin == false)
                .Include(x => x.Permissions)
                .GroupBy(x => x.Role)
                .Select(g => new PermisstionRoleResponse
                {
                    Id = g.Key.Id,
                    RoleName = g.Key.RoleName,
                    CreateDate = g.Key.CreatedDate,
                    IsSystem = g.Key.IsSystem,
                    Permissions = g.Select(x => new PermissionResponse
                        {
                            Id = x.Permissions.Id,
                            PermissonsName = x.Permissions.PermissonsName,
                            PermissonsDescription = x.Permissions.PermissonsDescription,
                        })
                        .ToList(),
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PermisstionRoleResponse>> GetPermissionByRoleId(int RoleId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Role)
                .Include(x => x.Permissions)
                .Where(pr => pr.RoleId == RoleId)
                .GroupBy(x => x.Role)
                .Select(g => new PermisstionRoleResponse
                {
                    RoleName = g.Key.RoleName,
                    CreateDate = g.Key.CreatedDate,
                    IsSystem = g.Key.IsSystem,
                    Permissions = g.Select(x => new PermissionResponse
                        {
                            Id = x.Permissions.Id,
                            PermissonsName = x.Permissions.PermissonsName,
                            PermissonsDescription = x.Permissions.PermissonsDescription,
                        })
                        .ToList(),
                })
                .ToListAsync();
        }

        public Task<List<PermissionResponse>> GetPermissionsbyRoleName(string roleName)
        {
            return _dbSet
                .AsNoTracking()
                .Include(x => x.Role)
                .Include(x => x.Permissions)
                .Where(x => x.Role.RoleName == roleName)
                .Select(x => new PermissionResponse
                {
                    Id = x.Permissions.Id,
                    PermissonsName = x.Permissions.PermissonsName,
                    PermissonsDescription = x.Permissions.PermissonsDescription,
                })
                .ToListAsync();
        }

        public Task<List<RolePermissions>> GetByRoleIdAsync(int roleId)
        {
            var test = _dbSet.Where(x => x.RoleId == roleId).ToListAsync();
            Console.WriteLine(test.Result);
            return _dbSet.Where(x => x.RoleId == roleId).ToListAsync();
        }

        public void RemoveRange(IEnumerable<RolePermissions> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}

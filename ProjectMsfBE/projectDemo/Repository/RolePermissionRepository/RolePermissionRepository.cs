using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.RolePermissionRepository
{
    public class RolePermissionRepository : RepositoryLinqBase<RoleRepository>, IRolePermissionRepository
    {
        public readonly RepositoryProcBase _proc;
        public RolePermissionRepository(IUnitOfWork uow) : base(uow)
        {
            _proc=new RepositoryProcBase(uow);
        }

    }
}

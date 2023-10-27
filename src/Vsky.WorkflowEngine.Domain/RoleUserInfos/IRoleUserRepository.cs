using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Vsky.WorkflowEngine.WorkflowEngineSettingsManages;

namespace Vsky.WorkflowEngine.RoleUserInfos
{
    public interface IRoleUserRepository : IRepository<IdentityUserRole>
    {
        Task<List<IdentityUserRole>> GetListAsync();

        Task<List<Guid>> GetPersonIdListByRoleIdList(List<Guid> roleIds);
    }
}

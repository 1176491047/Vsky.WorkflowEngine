using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Vsky.WorkflowEngine.EntityFrameworkCore;
using Vsky.WorkflowEngine.WorkflowEngineSettingsManages;

namespace Vsky.WorkflowEngine.RoleUserInfos
{
    public class EfCoreRoleUserRepository : EfCoreRepository<WorkflowEngineDbContext, IdentityUserRole>,IRoleUserRepository
    {
        public EfCoreRoleUserRepository(IDbContextProvider<WorkflowEngineDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<IdentityUserRole>> GetListAsync()
        {
            var result =await GetQueryableAsync();

            return result.ToList();

        }

        public async Task<List<Guid>> GetPersonIdListByRoleIdList(List<Guid> roleIds)
        {
            var roles = await GetQueryableAsync();
            List<Guid> result = roles.Where(x => roleIds.Contains(x.RoleId)).Select(x=>x.UserId).ToList();
            return result;
        }
    }
}

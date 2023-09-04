using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Vsky.WorkflowEngine.EntityFrameworkCore;

namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{
    public class EfCoreWorkflowEngineSettingsManageRepository : EfCoreRepository<WorkflowEngineDbContext, WorkflowEngineSettingsManage, Guid>, IWorkflowEngineSettingsManageRepository
    {
        public EfCoreWorkflowEngineSettingsManageRepository(IDbContextProvider<WorkflowEngineDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<WorkflowEngineSettingsManage>> GetListAsync(
            string filterText = null,
            string projectName = null,
            string description = null,
            int? historySaveDaysMin = null,
            int? historySaveDaysMax = null,
            string dBInfoDescription = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, projectName, description, historySaveDaysMin, historySaveDaysMax, dBInfoDescription);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? WorkflowEngineSettingsManageConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string projectName = null,
            string description = null,
            int? historySaveDaysMin = null,
            int? historySaveDaysMax = null,
            string dBInfoDescription = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, projectName, description, historySaveDaysMin, historySaveDaysMax, dBInfoDescription);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<WorkflowEngineSettingsManage> ApplyFilter(
            IQueryable<WorkflowEngineSettingsManage> query,
            string filterText,
            string projectName = null,
            string description = null,
            int? historySaveDaysMin = null,
            int? historySaveDaysMax = null,
            string dBInfoDescription = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.ProjectName.Contains(filterText) || e.Description.Contains(filterText) || e.DBInfoDescription.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(projectName), e => e.ProjectName.Contains(projectName))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(historySaveDaysMin.HasValue, e => e.HistorySaveDays >= historySaveDaysMin.Value)
                    .WhereIf(historySaveDaysMax.HasValue, e => e.HistorySaveDays <= historySaveDaysMax.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(dBInfoDescription), e => e.DBInfoDescription.Contains(dBInfoDescription));
        }
    }
}
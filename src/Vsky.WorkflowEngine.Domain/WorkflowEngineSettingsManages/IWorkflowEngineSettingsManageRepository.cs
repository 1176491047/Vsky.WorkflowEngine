using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{
    public interface IWorkflowEngineSettingsManageRepository : IRepository<WorkflowEngineSettingsManage, Guid>
    {
        Task<List<WorkflowEngineSettingsManage>> GetListAsync(
            string filterText = null,
            string projectName = null,
            string description = null,
            int? historySaveDaysMin = null,
            int? historySaveDaysMax = null,
            string dBInfoDescription = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string projectName = null,
            string description = null,
            int? historySaveDaysMin = null,
            int? historySaveDaysMax = null,
            string dBInfoDescription = null,
            CancellationToken cancellationToken = default);
    }
}
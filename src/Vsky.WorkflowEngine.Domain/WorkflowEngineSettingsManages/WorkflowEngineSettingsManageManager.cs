using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{
    public class WorkflowEngineSettingsManageManager : DomainService
    {
        private readonly IWorkflowEngineSettingsManageRepository _workflowEngineSettingsManageRepository;

        public WorkflowEngineSettingsManageManager(IWorkflowEngineSettingsManageRepository workflowEngineSettingsManageRepository)
        {
            _workflowEngineSettingsManageRepository = workflowEngineSettingsManageRepository;
        }

        public async Task<WorkflowEngineSettingsManage> CreateAsync(
        string projectName, string description, int historySaveDays, string dBInfoDescription)
        {
            Check.Range(historySaveDays, nameof(historySaveDays), WorkflowEngineSettingsManageConsts.HistorySaveDaysMinLength, WorkflowEngineSettingsManageConsts.HistorySaveDaysMaxLength);

            var workflowEngineSettingsManage = new WorkflowEngineSettingsManage(
             GuidGenerator.Create(),
             projectName, description, historySaveDays, dBInfoDescription
             );

            return await _workflowEngineSettingsManageRepository.InsertAsync(workflowEngineSettingsManage);
        }

        public async Task<WorkflowEngineSettingsManage> UpdateAsync(
            Guid id,
            string projectName, string description, int historySaveDays, string dBInfoDescription, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.Range(historySaveDays, nameof(historySaveDays), WorkflowEngineSettingsManageConsts.HistorySaveDaysMinLength, WorkflowEngineSettingsManageConsts.HistorySaveDaysMaxLength);

            var workflowEngineSettingsManage = await _workflowEngineSettingsManageRepository.GetAsync(id);

            workflowEngineSettingsManage.ProjectName = projectName;
            workflowEngineSettingsManage.Description = description;
            workflowEngineSettingsManage.HistorySaveDays = historySaveDays;
            workflowEngineSettingsManage.DBInfoDescription = dBInfoDescription;

            workflowEngineSettingsManage.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _workflowEngineSettingsManageRepository.UpdateAsync(workflowEngineSettingsManage);
        }

    }
}
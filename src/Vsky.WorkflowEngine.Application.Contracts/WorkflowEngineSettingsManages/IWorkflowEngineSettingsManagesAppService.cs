using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Vsky.WorkflowEngine.Shared;

namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{
    public interface IWorkflowEngineSettingsManagesAppService : IApplicationService
    {
        Task<PagedResultDto<WorkflowEngineSettingsManageDto>> GetListAsync(GetWorkflowEngineSettingsManagesInput input);

        Task<WorkflowEngineSettingsManageDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<WorkflowEngineSettingsManageDto> CreateAsync(WorkflowEngineSettingsManageCreateDto input);

        Task<WorkflowEngineSettingsManageDto> UpdateAsync(Guid id, WorkflowEngineSettingsManageUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(WorkflowEngineSettingsManageExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}
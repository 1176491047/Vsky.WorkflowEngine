using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Vsky.WorkflowEngine.Permissions;
using Vsky.WorkflowEngine.WorkflowEngineSettingsManages;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Vsky.WorkflowEngine.Shared;

namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{

    [Authorize(WorkflowEnginePermissions.WorkflowEngineSettingsManages.Default)]
    public class WorkflowEngineSettingsManagesAppService : ApplicationService, IWorkflowEngineSettingsManagesAppService
    {
        private readonly IDistributedCache<WorkflowEngineSettingsManageExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IWorkflowEngineSettingsManageRepository _workflowEngineSettingsManageRepository;
        private readonly WorkflowEngineSettingsManageManager _workflowEngineSettingsManageManager;

        public WorkflowEngineSettingsManagesAppService(IWorkflowEngineSettingsManageRepository workflowEngineSettingsManageRepository, WorkflowEngineSettingsManageManager workflowEngineSettingsManageManager, IDistributedCache<WorkflowEngineSettingsManageExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _workflowEngineSettingsManageRepository = workflowEngineSettingsManageRepository;
            _workflowEngineSettingsManageManager = workflowEngineSettingsManageManager;
        }

        public virtual async Task<PagedResultDto<WorkflowEngineSettingsManageDto>> GetListAsync(GetWorkflowEngineSettingsManagesInput input)
        {
            var totalCount = await _workflowEngineSettingsManageRepository.GetCountAsync(input.FilterText, input.ProjectName, input.Description, input.HistorySaveDaysMin, input.HistorySaveDaysMax, input.DBInfoDescription);
            var items = await _workflowEngineSettingsManageRepository.GetListAsync(input.FilterText, input.ProjectName, input.Description, input.HistorySaveDaysMin, input.HistorySaveDaysMax, input.DBInfoDescription, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<WorkflowEngineSettingsManageDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<WorkflowEngineSettingsManage>, List<WorkflowEngineSettingsManageDto>>(items)
            };
        }

        public virtual async Task<WorkflowEngineSettingsManageDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<WorkflowEngineSettingsManage, WorkflowEngineSettingsManageDto>(await _workflowEngineSettingsManageRepository.GetAsync(id));
        }

        [Authorize(WorkflowEnginePermissions.WorkflowEngineSettingsManages.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _workflowEngineSettingsManageRepository.DeleteAsync(id);
        }

        [Authorize(WorkflowEnginePermissions.WorkflowEngineSettingsManages.Create)]
        public virtual async Task<WorkflowEngineSettingsManageDto> CreateAsync(WorkflowEngineSettingsManageCreateDto input)
        {

            var workflowEngineSettingsManage = await _workflowEngineSettingsManageManager.CreateAsync(
            input.ProjectName, input.Description, input.HistorySaveDays, input.DBInfoDescription
            );

            return ObjectMapper.Map<WorkflowEngineSettingsManage, WorkflowEngineSettingsManageDto>(workflowEngineSettingsManage);
        }

        [Authorize(WorkflowEnginePermissions.WorkflowEngineSettingsManages.Edit)]
        public virtual async Task<WorkflowEngineSettingsManageDto> UpdateAsync(Guid id, WorkflowEngineSettingsManageUpdateDto input)
        {

            var workflowEngineSettingsManage = await _workflowEngineSettingsManageManager.UpdateAsync(
            id,
            input.ProjectName, input.Description, input.HistorySaveDays, input.DBInfoDescription, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<WorkflowEngineSettingsManage, WorkflowEngineSettingsManageDto>(workflowEngineSettingsManage);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(WorkflowEngineSettingsManageExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _workflowEngineSettingsManageRepository.GetListAsync(input.FilterText, input.ProjectName, input.Description, input.HistorySaveDaysMin, input.HistorySaveDaysMax, input.DBInfoDescription);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<WorkflowEngineSettingsManage>, List<WorkflowEngineSettingsManageExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "WorkflowEngineSettingsManages.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new WorkflowEngineSettingsManageExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}
using Volo.Abp.Application.Dtos;
using System;

namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{
    public class WorkflowEngineSettingsManageExcelDownloadDto
    {
        public string DownloadToken { get; set; }

        public string? FilterText { get; set; }

        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        public int? HistorySaveDaysMin { get; set; }
        public int? HistorySaveDaysMax { get; set; }
        public string? DBInfoDescription { get; set; }

        public WorkflowEngineSettingsManageExcelDownloadDto()
        {

        }
    }
}
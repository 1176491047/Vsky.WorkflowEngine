using Volo.Abp.Application.Dtos;
using System;

namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{
    public class GetWorkflowEngineSettingsManagesInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        public int? HistorySaveDaysMin { get; set; }
        public int? HistorySaveDaysMax { get; set; }
        public string? DBInfoDescription { get; set; }

        public GetWorkflowEngineSettingsManagesInput()
        {

        }
    }
}
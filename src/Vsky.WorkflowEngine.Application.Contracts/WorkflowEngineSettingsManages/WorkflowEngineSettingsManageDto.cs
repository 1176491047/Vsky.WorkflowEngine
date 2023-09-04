using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{
    public class WorkflowEngineSettingsManageDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        public int HistorySaveDays { get; set; }
        public string? DBInfoDescription { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}
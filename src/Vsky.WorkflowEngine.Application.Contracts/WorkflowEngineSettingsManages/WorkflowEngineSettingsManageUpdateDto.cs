using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{
    public class WorkflowEngineSettingsManageUpdateDto : IHasConcurrencyStamp
    {
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        [Required]
        [Range(WorkflowEngineSettingsManageConsts.HistorySaveDaysMinLength, WorkflowEngineSettingsManageConsts.HistorySaveDaysMaxLength)]
        public int HistorySaveDays { get; set; }
        public string? DBInfoDescription { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}
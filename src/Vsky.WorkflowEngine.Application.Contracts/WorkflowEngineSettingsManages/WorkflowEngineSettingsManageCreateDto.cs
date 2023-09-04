using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{
    public class WorkflowEngineSettingsManageCreateDto
    {
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        [Required]
        [Range(WorkflowEngineSettingsManageConsts.HistorySaveDaysMinLength, WorkflowEngineSettingsManageConsts.HistorySaveDaysMaxLength)]
        public int HistorySaveDays { get; set; } = 30;
        public string? DBInfoDescription { get; set; }
    }
}
using System;

namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{
    public class WorkflowEngineSettingsManageExcelDto
    {
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        public int HistorySaveDays { get; set; }
        public string? DBInfoDescription { get; set; }
    }
}
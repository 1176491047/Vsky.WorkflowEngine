namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{
    public static class WorkflowEngineSettingsManageConsts
    {
        private const string DefaultSorting = "{0}ProjectName asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "WorkflowEngineSettingsManage." : string.Empty);
        }

        public const int HistorySaveDaysMinLength = 5;
        public const int HistorySaveDaysMaxLength = 60;
    }
}
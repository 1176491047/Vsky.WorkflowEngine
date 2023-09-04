namespace Vsky.WorkflowEngine.Permissions;

public static class WorkflowEnginePermissions
{
    public const string GroupName = "WorkflowEngine";

    public const string ElsaDashboard = GroupName + ".ElsaDashboard";
    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";


    public static class WorkflowEngineSettingsManages
    {
        public const string Default = GroupName + ".WorkflowEngineSettingsManages";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}

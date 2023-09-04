using Vsky.WorkflowEngine.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Vsky.WorkflowEngine.Permissions;

public class WorkflowEnginePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(WorkflowEnginePermissions.GroupName);




        myGroup.AddPermission(WorkflowEnginePermissions.ElsaDashboard, L("Permission:ElsaDashboard"));
        //Define your own permissions here. Example:
        //myGroup.AddPermission(WorkflowEnginePermissions.MyPermission1, L("Permission:MyPermission1"));

        var workflowEngineSettingsManagePermission = myGroup.AddPermission(WorkflowEnginePermissions.WorkflowEngineSettingsManages.Default, L("Permission:WorkflowEngineSettingsManages"));
        workflowEngineSettingsManagePermission.AddChild(WorkflowEnginePermissions.WorkflowEngineSettingsManages.Create, L("Permission:Create"));
        workflowEngineSettingsManagePermission.AddChild(WorkflowEnginePermissions.WorkflowEngineSettingsManages.Edit, L("Permission:Edit"));
        workflowEngineSettingsManagePermission.AddChild(WorkflowEnginePermissions.WorkflowEngineSettingsManages.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkflowEngineResource>(name);
    }
}

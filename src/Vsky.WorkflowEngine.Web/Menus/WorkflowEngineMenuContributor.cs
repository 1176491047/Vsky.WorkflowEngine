using System.Threading.Tasks;
using Vsky.WorkflowEngine.Localization;
using Vsky.WorkflowEngine.MultiTenancy;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Vsky.WorkflowEngine.Permissions;

namespace Vsky.WorkflowEngine.Web.Menus;

public class WorkflowEngineMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<WorkflowEngineResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                WorkflowEngineMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );
        //add Workflow menu-item
        context.Menu.Items.Insert(
            1,
            new ApplicationMenuItem(
                WorkflowEngineMenus.Home,
                "Workflow",
                "~/elsa",
                icon: "fas fa-code-branch",
                order: 1,
                requiredPermissionName: WorkflowEnginePermissions.ElsaDashboard
            )
        );
        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);


        context.Menu.AddItem(
            new ApplicationMenuItem(
                WorkflowEngineMenus.WorkflowEngineSettingsManages,
                l["Menu:WorkflowEngineSettingsManages"],
                url: "/WorkflowEngineSettingsManages",
                icon: "fa fa-file-alt",
                requiredPermissionName: WorkflowEnginePermissions.WorkflowEngineSettingsManages.Default)
        );

        return Task.CompletedTask;
    }
}

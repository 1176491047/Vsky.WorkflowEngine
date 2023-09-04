using Volo.Abp.Settings;

namespace Vsky.WorkflowEngine.Settings;

public class WorkflowEngineSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(WorkflowEngineSettings.MySetting1));
    }
}

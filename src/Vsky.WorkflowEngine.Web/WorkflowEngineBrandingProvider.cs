using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Vsky.WorkflowEngine.Web;

[Dependency(ReplaceServices = true)]
public class WorkflowEngineBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "WorkflowEngine";
}

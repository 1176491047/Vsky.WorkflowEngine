using Vsky.WorkflowEngine.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Vsky.WorkflowEngine.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class WorkflowEngineController : AbpControllerBase
{
    protected WorkflowEngineController()
    {
        LocalizationResource = typeof(WorkflowEngineResource);
    }
}

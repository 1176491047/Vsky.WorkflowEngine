using Vsky.WorkflowEngine.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Vsky.WorkflowEngine.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class WorkflowEnginePageModel : AbpPageModel
{
    protected WorkflowEnginePageModel()
    {
        LocalizationResourceType = typeof(WorkflowEngineResource);
    }
}

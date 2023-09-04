using System;
using System.Collections.Generic;
using System.Text;
using Vsky.WorkflowEngine.Localization;
using Volo.Abp.Application.Services;

namespace Vsky.WorkflowEngine;

/* Inherit your application services from this class.
 */
public abstract class WorkflowEngineAppService : ApplicationService
{
    protected WorkflowEngineAppService()
    {
        LocalizationResource = typeof(WorkflowEngineResource);
    }
}

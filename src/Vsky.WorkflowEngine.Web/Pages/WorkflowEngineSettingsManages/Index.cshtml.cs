using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Vsky.WorkflowEngine.WorkflowEngineSettingsManages;
using Vsky.WorkflowEngine.Shared;

namespace Vsky.WorkflowEngine.Web.Pages.WorkflowEngineSettingsManages
{
    public class IndexModel : AbpPageModel
    {
        public string? ProjectNameFilter { get; set; }
        public string? DescriptionFilter { get; set; }
        public int? HistorySaveDaysFilterMin { get; set; }

        public int? HistorySaveDaysFilterMax { get; set; }
        public string? DBInfoDescriptionFilter { get; set; }

        private readonly IWorkflowEngineSettingsManagesAppService _workflowEngineSettingsManagesAppService;

        public IndexModel(IWorkflowEngineSettingsManagesAppService workflowEngineSettingsManagesAppService)
        {
            _workflowEngineSettingsManagesAppService = workflowEngineSettingsManagesAppService;
        }

        public async Task OnGetAsync()
        {

            await Task.CompletedTask;
        }
    }
}
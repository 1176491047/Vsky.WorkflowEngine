using Vsky.WorkflowEngine.Shared;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vsky.WorkflowEngine.WorkflowEngineSettingsManages;

namespace Vsky.WorkflowEngine.Web.Pages.WorkflowEngineSettingsManages
{
    public class CreateModalModel : WorkflowEnginePageModel
    {
        [BindProperty]
        public WorkflowEngineSettingsManageCreateViewModel WorkflowEngineSettingsManage { get; set; }

        private readonly IWorkflowEngineSettingsManagesAppService _workflowEngineSettingsManagesAppService;

        public CreateModalModel(IWorkflowEngineSettingsManagesAppService workflowEngineSettingsManagesAppService)
        {
            _workflowEngineSettingsManagesAppService = workflowEngineSettingsManagesAppService;

            WorkflowEngineSettingsManage = new();
        }

        public async Task OnGetAsync()
        {
            WorkflowEngineSettingsManage = new WorkflowEngineSettingsManageCreateViewModel();

            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync()
        {

            await _workflowEngineSettingsManagesAppService.CreateAsync(ObjectMapper.Map<WorkflowEngineSettingsManageCreateViewModel, WorkflowEngineSettingsManageCreateDto>(WorkflowEngineSettingsManage));
            return NoContent();
        }
    }

    public class WorkflowEngineSettingsManageCreateViewModel : WorkflowEngineSettingsManageCreateDto
    {
    }
}
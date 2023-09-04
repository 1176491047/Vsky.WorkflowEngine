using Vsky.WorkflowEngine.Shared;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using Vsky.WorkflowEngine.WorkflowEngineSettingsManages;

namespace Vsky.WorkflowEngine.Web.Pages.WorkflowEngineSettingsManages
{
    public class EditModalModel : WorkflowEnginePageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public WorkflowEngineSettingsManageUpdateViewModel WorkflowEngineSettingsManage { get; set; }

        private readonly IWorkflowEngineSettingsManagesAppService _workflowEngineSettingsManagesAppService;

        public EditModalModel(IWorkflowEngineSettingsManagesAppService workflowEngineSettingsManagesAppService)
        {
            _workflowEngineSettingsManagesAppService = workflowEngineSettingsManagesAppService;

            WorkflowEngineSettingsManage = new();
        }

        public async Task OnGetAsync()
        {
            var workflowEngineSettingsManage = await _workflowEngineSettingsManagesAppService.GetAsync(Id);
            WorkflowEngineSettingsManage = ObjectMapper.Map<WorkflowEngineSettingsManageDto, WorkflowEngineSettingsManageUpdateViewModel>(workflowEngineSettingsManage);

        }

        public async Task<NoContentResult> OnPostAsync()
        {

            await _workflowEngineSettingsManagesAppService.UpdateAsync(Id, ObjectMapper.Map<WorkflowEngineSettingsManageUpdateViewModel, WorkflowEngineSettingsManageUpdateDto>(WorkflowEngineSettingsManage));
            return NoContent();
        }
    }

    public class WorkflowEngineSettingsManageUpdateViewModel : WorkflowEngineSettingsManageUpdateDto
    {
    }
}
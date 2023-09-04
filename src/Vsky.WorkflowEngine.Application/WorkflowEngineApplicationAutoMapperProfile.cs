using AutoMapper;
using Vsky.WorkflowEngine.WorkflowEngineSettingsManages;

namespace Vsky.WorkflowEngine;

public class WorkflowEngineApplicationAutoMapperProfile : Profile
{
    public WorkflowEngineApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<WorkflowEngineSettingsManage, WorkflowEngineSettingsManageDto>();
        CreateMap<WorkflowEngineSettingsManage, WorkflowEngineSettingsManageExcelDto>();
    }
}

using Volo.Abp.Application.Dtos;

namespace Vsky.WorkflowEngine.Shared
{
    public class LookupRequestDto : PagedResultRequestDto
    {
        public string? Filter { get; set; }

        public LookupRequestDto()
        {
            MaxResultCount = MaxMaxResultCount;
        }
    }
}
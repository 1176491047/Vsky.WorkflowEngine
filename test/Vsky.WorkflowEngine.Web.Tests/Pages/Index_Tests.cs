using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Vsky.WorkflowEngine.Pages;

public class Index_Tests : WorkflowEngineWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}

using Elsa.Design;
using Elsa.Metadata;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Vsky.WorkflowEngine.Web.Activities.Bussness.DynamicFormBind
{
    public class GetDynamicFormData : IActivityPropertyOptionsProvider
    {
        private IHttpClientFactory _httpClientFactory;
        private IConfiguration _configuration;
        public GetDynamicFormData(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private async Task<List<SelectListItem>> GetDynamicFormInfo() {
            string url = _configuration["DynamicFormServiceURL"];
            var httpClient = _httpClientFactory.CreateClient();
            var result = await httpClient.GetAsync(url).Result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<SelectListItem>>(result);
        }
        public object GetOptions(PropertyInfo property)
        {
            //List<SelectListItem> selectListItems = GetDynamicFormInfo().Result;
            return new object();

        }
    }
}

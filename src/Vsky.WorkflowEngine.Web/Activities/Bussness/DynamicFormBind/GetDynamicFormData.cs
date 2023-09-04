using Elsa.Design;
using Elsa.Metadata;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace Vsky.WorkflowEngine.Web.Activities.Bussness.DynamicFormBind
{
    public class GetDynamicFormData : IActivityPropertyOptionsProvider
    {
        private IHttpClientFactory _httpClientFactory;
        public GetDynamicFormData(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public object? GetOptions(PropertyInfo property)
        {
            return new List<SelectListItem>()
        {
            new SelectListItem("模板A","TempA"),
            new SelectListItem("模板B","TempBA"),
        };
        }
    }
}

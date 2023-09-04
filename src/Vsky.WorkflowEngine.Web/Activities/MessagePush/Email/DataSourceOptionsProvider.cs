using Confluent.Kafka;
using Elsa.Design;
using Elsa.Metadata;
using System.Collections.Generic;
using System.Reflection;

namespace Vsky.WorkflowEngine.Web.Activities.MessagePush.Email
{
    public class DataSourceOptionsProvider : IActivityPropertyOptionsProvider
    {
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

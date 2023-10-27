using Elsa.Metadata;
using System.Collections.Generic;
using System.Reflection;

namespace Vsky.WorkflowEngine.Web.Activities.Bussness.DynamicFormBind
{
    public class PersonDefaultMethodsProvider : IActivityPropertyDefaultValueProvider
    {
        public object GetDefaultValue(PropertyInfo property)
        {
            return new HashSet<string> { "" };
        }
    }
}

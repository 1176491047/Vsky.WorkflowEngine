using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Vsky.WorkflowEngine.Web.Activities
{
    public class HttpResponseResult
    {
        public string Msg { get; set; }
        public long Code { get; set; }
        public bool Success { get; set; }
        public object Data { get; set; }
    }
}

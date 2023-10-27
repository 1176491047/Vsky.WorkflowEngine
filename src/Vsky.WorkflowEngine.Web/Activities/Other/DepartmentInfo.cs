using Elsa.Server.Api.Endpoints.Activities;
using System.Collections.Generic;

namespace Vsky.WorkflowEngine.Web.Activities.Other
{
    public class DepartmentInfo
    {
        public string DepartMentName { get; set; }
        public string DepartmentId { get; set; }

        public List<UserInfo> UserInfos { get; set; }
    }
    public class UserInfo
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
    }
}

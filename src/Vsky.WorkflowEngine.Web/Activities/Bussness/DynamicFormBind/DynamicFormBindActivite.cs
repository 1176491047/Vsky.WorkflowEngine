using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elsa;
using Elsa.Activities.Email;
using Elsa.Activities.Email.Options;
using Elsa.Activities.Email.Services;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Providers.WorkflowStorage;
using Elsa.Serialization;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Options;
using MimeKit;


namespace Vsky.WorkflowEngine.Web.Activities.Bussness.DynamicFormBind
{
    [Action(
        Category = "业务组件",
        DisplayName = "绑定动态表单",
        Description = "绑定第三方动态表单和角色",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class DynamicFormBindActivite : Activity
    {



        [ActivityInput(
                Label = "选择表单",
                Hint = "请选择节点对应的表单",
              UIHint = ActivityInputUIHints.Dropdown,
              OptionsProvider = typeof(DataSourceOptionsProvider),
              Category = PropertyCategories.Configuration,
              Order = 3
          )]
        public string? Test { get; set; }

    }
}

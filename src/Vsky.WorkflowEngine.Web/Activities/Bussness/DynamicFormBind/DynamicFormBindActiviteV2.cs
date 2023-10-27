using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Elsa;
using Elsa.Activities.Email;
using Elsa.Activities.Email.Options;
using Elsa.Activities.Email.Services;
using Elsa.Activities.Http;
using Elsa.Activities.Http.Models;
using Elsa.Activities.Primitives;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Events;
using Elsa.Expressions;
using Elsa.Metadata;
using Elsa.Providers.WorkflowStorage;
using Elsa.Serialization;
using Elsa.Services;
using Elsa.Services.Models;
using MediatR;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MimeKit;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Volo.Abp.Identity;
using Vsky.WorkflowEngine.RoleUserInfos;
using Vsky.WorkflowEngine.Web.Activities.Other;
using HttpRequestHeaders = Elsa.Activities.Http.Models.HttpRequestHeaders;

namespace Vsky.WorkflowEngine.Web.Activities.Bussness.DynamicFormBind
{
    [Action(
        Category = "业务组件",
        DisplayName = "审批节点",
        Description = "定义好流程后供动态表单绑定",
        Outcomes = new[] { OutcomeNames.Done,"通过", "一类驳回", "二类驳回" }
    )]
    public class DynamicFormBindActiviteV2 : Activity
    {
        private readonly Random _random;
        private HttpClient _httpClient;
        private IConfiguration _configuration;
        public DynamicFormBindActiviteV2(HttpClient httpClient, IConfiguration configuration)
        {
            _random = new Random();
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [ActivityInput(
                Category = "基础配置",
                Label = "选择责任部门",
            UIHint = ActivityInputUIHints.Dropdown,
            OptionsProvider = typeof(GetABPRoles),
            DefaultSyntax = SyntaxNames.Literal,
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid }
        )]
        public string? Department { get; set; }

        [ActivityInput(
                Category = "基础配置",
                Label = "输入审批人",
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = SyntaxNames.Json,
            SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript })]
        public ICollection<string> Persons { get; set; } = new List<string>();

        #region 请求配置
        /// <summary>
        /// The HTTP method to use.
        /// </summary>
        [ActivityInput(
                Category = "请求配置",
                Hint = "选择HTTP method",
            UIHint = ActivityInputUIHints.Dropdown,
            Options = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS", "HEAD" },
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid }
        )]
        public string? Method { get; set; } = "POST";

        [ActivityInput(
                Category = "请求配置",
            Label = "授权",
            Hint = "请求的Authorization标头值",
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid }
        )]
        public string? Authorization { get; set; }


        [ActivityInput(
                Category = "请求配置",
             Hint = "要随请求一起发送的其他标头",
             UIHint = ActivityInputUIHints.MultiLine, DefaultSyntax = SyntaxNames.Json,
             SupportedSyntaxes = new[] { SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid }
         )]
        public HttpRequestHeaders RequestHeaders { get; set; } = new();

        [ActivityInput(
                Category = "请求配置",
         UIHint = ActivityInputUIHints.Dropdown,
         Hint = "请求的内容类型",
         Options = new[] { "", "text/plain", "text/html", "application/json", "application/xml", "application/x-www-form-urlencoded" },
         SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid }
     )]
        public string? ContentType { get; set; } = "application/json";

        #endregion


        /// <summary>
        /// 触发时拼接参数 推送给第三方系统 等待回调
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            string formId = context.GetVariable("FormId")?.ToString();
            if (string.IsNullOrEmpty(formId))
            {
                var sourceActivity = context.WorkflowExecutionContext.WorkflowBlueprint.Connections.Where(x => x.Target.Activity.Id == context.ActivityId).FirstOrDefault().Source.Activity;
                //端点类型触发 第三方调用
                if (sourceActivity.Type == "HttpEndpoint")
                {
                    res resust = new res();
                    HttpRequestModel requestModel = (HttpRequestModel)context.Input;
                    formId = requestModel.QueryString["FormInstanceId"].ToString();
                    if (string.IsNullOrEmpty(formId))
                    {
                        return Fault("未查询到模板实例ID 检查流程启动入参");
                    }

                    context.SetVariable("FormId", formId);
                }
            }
            //读取业务服务接口地址
            string serviceUrl = _configuration["DynamicFormServiceMessagePushURL"];
            DynamicFormServiceInput dynamicFormServiceInput = new DynamicFormServiceInput()
            {
                CurrentNodeId = Guid.Parse(context.ActivityId),
                Department = Guid.Parse(Department),
                Users = Persons.ToList(),
                WorkflowDefinitionId = Guid.Parse(context.WorkflowInstance.DefinitionId),
                WorkflowInstanseId=Guid.Parse(context.WorkflowInstance.Id),
                CurrentNodeName = DisplayName,
                FormInstanceId = Guid.Parse(formId)
            };
            //发送给业务服务后阻塞流程等待回调
            var request = CreateRequest(serviceUrl, dynamicFormServiceInput);
            await _httpClient.SendAsync(request);
            context.SetVariable($"{Guid.NewGuid()}",$"{context.ActivityBlueprint.Name}节点由{Persons.JoinAsString("、")},触发");
            return Suspend();
        }

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            context.SetVariable($"{Guid.NewGuid()}", $"{context.ActivityBlueprint.Name},给出结果{context.Input.ToString()}");
            if (string.IsNullOrEmpty(context.Input.ToString()))
            {
                return Done();
            }

            return Outcome(context.Input.ToString());
        }

        private HttpRequestMessage CreateRequest(string url,object? content)
        {
            var method = Method ?? HttpMethods.Get;
            var methodSupportsBody = GetMethodSupportsBody(method);
            var request = new HttpRequestMessage(new HttpMethod(method), url);
            var authorizationHeaderValue = Authorization;
            var requestHeaders = new HeaderDictionary(RequestHeaders.ToDictionary(x => x.Key, x => new StringValues(x.Value.Split(','))));

            if (methodSupportsBody)
            {
                var bodyAsString = content as string;
                var bodyAsBytes = content as byte[];
                var contentType = ContentType;

                if (!string.IsNullOrWhiteSpace(bodyAsString))
                    request.Content = new StringContent(bodyAsString, Encoding.UTF8, contentType);
                else if (bodyAsBytes != null)
                {
                    request.Content = new ByteArrayContent(bodyAsBytes);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                }
            }

            if (!string.IsNullOrWhiteSpace(authorizationHeaderValue))
                request.Headers.Authorization = AuthenticationHeaderValue.Parse(authorizationHeaderValue);

            foreach (var header in requestHeaders)
                request.Headers.Add(header.Key, header.Value.AsEnumerable());

            return request;
        }


        private static bool GetMethodSupportsBody(string method)
        {
            var methods = new[] { "POST", "PUT", "PATCH", "DELETE" };
            return methods.Contains(method, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}

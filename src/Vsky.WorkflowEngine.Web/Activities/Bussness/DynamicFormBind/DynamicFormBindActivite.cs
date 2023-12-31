﻿using System;
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
        Description = "绑定第三方动态表单和角色",
        Outcomes = new[] { OutcomeNames.Done,"通过","驳回" }
    )]
    public class DynamicFormBindActivite : Activity
    {
        private readonly Random _random;
        private HttpClient _httpClient;
        private IConfiguration _configuration;
        public DynamicFormBindActivite(HttpClient httpClient, IConfiguration configuration)
        {
            _random = new Random();
            _httpClient = httpClient;
            _configuration = configuration;
        }





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
        public string? Method { get; set; }

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
        public string? ContentType { get; set; }

        /// <summary>
        /// 表单模板编码
        /// </summary>
        [ActivityInput(
                Category = "基础配置",
                Label = "选择表单",
                Hint = "请选择节点对应的表单",
              UIHint = ActivityInputUIHints.Dropdown,
              OptionsProvider = typeof(GetDynamicFormData)
          )]
        public string? FormTemplate { get; set; }


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
                Label = "选择责任人",
            UIHint = ActivityInputUIHints.CheckList,
            //Options = new[] {"1","2","3" },
            OptionsProvider = typeof(GetABPUsers),
            //DefaultValueProvider =typeof(PersonDefaultMethodsProvider),
            DefaultSyntax = SyntaxNames.Literal,
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid }
        )]
        public HashSet<string> Persons
        {
            get => GetState<HashSet<string>>(() => new HashSet<string>());
            set => SetState(value);
        }

        [ActivityInput(
                Label = "输入审批人",
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = SyntaxNames.Literal,
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid }
        )]
        public IList<string> Persons2
        {
            get => GetState<IList<string>>(() => new List<string>());
            set => SetState(value);
        }

        /// <summary>
        /// 触发时拼接参数 推送给第三方系统 等待回调
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            //读取业务服务接口地址
            string serviceUrl = _configuration["DynamicFormServiceMessagePushURL"];
            DynamicFormServiceInput dynamicFormServiceInput = new DynamicFormServiceInput()
            {
                //CurrentNodeId = context.ActivityId,
                //Department = Department,
                //Users = Persons.ToList(),
                //TempCode = FormTemplate,
                //WorkflowDefinitionId = context.WorkflowInstance.DefinitionId,
                //WorkflowInstanseId = context.WorkflowInstance.Id
            };
            //发送给业务服务后阻塞流程等待回调
            var request = CreateRequest(serviceUrl, dynamicFormServiceInput);
            var response = await _httpClient.SendAsync(request);
            //context.SetVariable(context.ActivityId, response);

            context.SetVariable($"{Guid.NewGuid()}",$"{context.ActivityBlueprint.Name}节点由{Persons.JoinAsString("、")}-{Persons2.JoinAsString("、")},触发");
            return Suspend();
        }

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            context.SetVariable($"{Guid.NewGuid()}", $"{context.ActivityBlueprint.Name},给出结果{context.Input.ToString()}");
            if (context.Input.ToString()=="通过")
            {
                return Outcome("通过");
            }
            if (context.Input.ToString() == "驳回")
            {
                return Outcome("驳回");
            }
            return Done();
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

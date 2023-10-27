using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Metadata;
using Elsa.Services;
using Newtonsoft.Json.Linq;

namespace Vsky.WorkflowEngine.Web.Activities.Other
{
    /// <summary>
    /// 下拉框级联
    /// </summary>
    public class DynamicVehicleActivity : Activity, IActivityPropertyOptionsProvider, IRuntimeSelectListProvider
    {
        private readonly Random _random;

        public DynamicVehicleActivity()
        {
            _random = new Random();
        }

        [ActivityInput(
            UIHint = ActivityInputUIHints.Dropdown,
            OptionsProvider = typeof(DynamicVehicleActivity),
            DefaultSyntax = SyntaxNames.Literal,
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid }
        )]
        public string? Department { get; set; }

        [ActivityInput(
            UIHint = ActivityInputUIHints.Dropdown,
            OptionsProvider = typeof(DynamicVehicleActivity),
            DefaultSyntax = SyntaxNames.Literal,
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.Json, SyntaxNames.JavaScript, SyntaxNames.Liquid },
            DependsOnEvents = new[] { "Department" },
            DependsOnValues = new[] { "Department" }
        )]
        public string? Person { get; set; }

        [ActivityOutput] public string? Output { get; set; }

        /// <summary>
        /// Return options to be used by the designer. The designer will pass back whatever context is provided here.
        /// </summary>
        public object GetOptions(PropertyInfo property) => new RuntimeSelectListProviderSettings(GetType(),
            new CascadingDropDownContext(property.Name,
                property.GetCustomAttribute<ActivityInputAttribute>()!.DependsOnEvents!,
                property.GetCustomAttribute<ActivityInputAttribute>()!.DependsOnValues!
                , new Dictionary<string, string>(), new DynamicVehicleContext(_random.Next(100))));

        /// <summary>
        /// Invoked from an API endpoint that is invoked by the designer every time an activity editor for this activity is opened.
        /// </summary>
        /// <param name="context">The context from GetOptions</param>
        public ValueTask<SelectList> GetSelectListAsync(object? context = default, CancellationToken cancellationToken = default)
        {
            var cascadingDropDownContext = (CascadingDropDownContext)context!;
            var dataSource = GetDataSource();
            var dependencyValues = cascadingDropDownContext.DepValues;

            switch (cascadingDropDownContext.Name)
            {
                case "Department":
                    {
                        var brands = dataSource.Select(x => new SelectListItem(x.DepartMentName,x.DepartmentId));
                        return new ValueTask<SelectList>(new SelectList(brands.ToList()));
                    }

                case "Person" when dependencyValues.TryGetValue("Department", out var brandValue):
                    {
                        var brandText = brandValue;
                        var models = dataSource.First(x => x.DepartmentId == brandText).UserInfos;
                        var modelsItems = models?.Select(x => new SelectListItem(x.UserName, x.UserId));
                        return new ValueTask<SelectList>(new SelectList(modelsItems.ToList()));
                    }
                default:
                    return new ValueTask<SelectList>();
            }
        }

        private List<DepartmentInfo> GetDataSource()
        {
            List<DepartmentInfo> departmentInfos = new List<DepartmentInfo>();
            departmentInfos.Add(new DepartmentInfo()
            {
                DepartMentName = "生产部",
                DepartmentId = "1",
                UserInfos = new List<UserInfo>() {
            new UserInfo{UserName="生产部-张三",UserId="1.1" },
            new UserInfo{UserName="生产部-李四",UserId="1.2" },
            new UserInfo{UserName="生产部-王五",UserId="1.3" }
            }

            });

            departmentInfos.Add(new DepartmentInfo()
            {
                DepartMentName = "质量部",
                DepartmentId = "2",
                UserInfos = new List<UserInfo>() {
            new UserInfo{UserName="质量部-张三",UserId="2.1" },
            new UserInfo{UserName="质量部-李四",UserId="2.2" },
            new UserInfo{UserName="质量部-王五",UserId="2.3" }
            }

            });

            departmentInfos.Add(new DepartmentInfo()
            {
                DepartMentName = "工艺部",
                DepartmentId = "3",
                UserInfos = new List<UserInfo>() {
            new UserInfo{UserName="工艺部-张三",UserId="3.1" },
            new UserInfo{UserName="工艺部-李四",UserId="3.2" },
            new UserInfo{UserName="工艺部-王五",UserId="3.3" }
            }

            });


            return departmentInfos;
        }
        protected override IActivityExecutionResult OnExecute()
        {
            Output = Department;
            return Done();
        }

    }

    public record DynamicVehicleContext(int RandomNumber);

    public record CascadingDropDownContext(string Name, string[] DependsOnEvent, string[] DependsOnValue, IDictionary<string, string> DepValues, object? Context);
}

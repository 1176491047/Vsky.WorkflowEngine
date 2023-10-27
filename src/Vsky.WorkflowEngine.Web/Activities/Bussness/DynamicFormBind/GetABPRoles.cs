using Elsa.Attributes;
using Elsa.Design;
using Elsa.Metadata;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Vsky.WorkflowEngine.Web.Activities.Other;
using System.Linq;
using System.Net.Http;
using Volo.Abp.Identity;
using Vsky.WorkflowEngine.RoleUserInfos;
using Microsoft.Extensions.Configuration;

namespace Vsky.WorkflowEngine.Web.Activities.Bussness.DynamicFormBind
{
    public class GetABPRoles : IActivityPropertyOptionsProvider, IRuntimeSelectListProvider
    {
        private readonly Random _random;
        private IHttpClientFactory _httpClientFactory;
        private IConfiguration _configuration;
        private IIdentityRoleRepository _roleRepository;
        private IIdentityUserRepository _identityUserRepository;
        private IRoleUserRepository _roleUserRepository;

        public GetABPRoles(IRoleUserRepository roleUserRepository, IIdentityUserRepository identityUserRepository, IIdentityRoleRepository identityRoleRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _random = new Random();
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _roleRepository = identityRoleRepository;
            _identityUserRepository = identityUserRepository;
            _roleUserRepository = roleUserRepository;
        }
        public object GetOptions(PropertyInfo property)
        {

            return new RuntimeSelectListProviderSettings(GetType(),
              new CascadingDropDownContext(property.Name,
                  property.GetCustomAttribute<ActivityInputAttribute>()!.DependsOnEvents!,
                  property.GetCustomAttribute<ActivityInputAttribute>()!.DependsOnValues!
                  , new Dictionary<string, string>(), new DynamicVehicleContext(_random.Next(100))));
        }

        public ValueTask<SelectList> GetSelectListAsync(object? context = default, CancellationToken cancellationToken = default)
        {
            var cascadingDropDownContext = (CascadingDropDownContext)context!;
            var dataSource = GetDataSource();
            var dependencyValues = cascadingDropDownContext.DepValues;

            switch (cascadingDropDownContext.Name)
            {
                case "Department":
                    {
                        var brands = dataSource.Select(x => new SelectListItem(x.DepartMentName, x.DepartmentId));
                        return new ValueTask<SelectList>(new SelectList(brands.ToList()));
                    }

                //case "Person" when dependencyValues.TryGetValue("Department", out var brandValue):
                //    {
                //        var brandText = brandValue;
                //        var models = dataSource.First(x => x.DepartmentId == brandText).UserInfos;
                //        var modelsItems = models?.Select(x => new SelectListItem(x.UserName, x.UserId));
                //        return new ValueTask<SelectList>(new SelectList(modelsItems.ToList()));
                //    }
                default:
                    return new ValueTask<SelectList>();
            }
        }

        private List<DepartmentInfo> GetDataSource()
        {
            var roles = _roleRepository.GetListAsync().Result;
            var persons = _identityUserRepository.GetListAsync().Result;
            var relations = _roleUserRepository.GetListAsync().Result;

            List<DepartmentInfo> departmentInfos = new List<DepartmentInfo>();
            foreach (var item in roles)
            {
                DepartmentInfo departmentInfo = new DepartmentInfo()
                {
                    DepartMentName = item.Name,
                    DepartmentId = item.Id.ToString()
                };
                List<Guid> personIds = relations.Where(x=>x.RoleId==item.Id).Select(x=>x.UserId).ToList();
                departmentInfo.UserInfos = persons.Where(x=>personIds.Contains(x.Id)).Select(x=>new UserInfo() {UserId=x.Id.ToString(),UserName=$"{x.Surname}{x.Name}" }).ToList();
                departmentInfos.Add(departmentInfo);
            }

            //departmentInfos.Add(new DepartmentInfo()
            //{
            //    DepartMentName = "生产部",
            //    DepartmentId = "1",
            //    UserInfos = new List<UserInfo>() {
            //new UserInfo{UserName="生产部-张三",UserId="1.1" },
            //new UserInfo{UserName="生产部-李四",UserId="1.2" },
            //new UserInfo{UserName="生产部-王五",UserId="1.3" }
            //}

            //});

            //departmentInfos.Add(new DepartmentInfo()
            //{
            //    DepartMentName = "质量部",
            //    DepartmentId = "2",
            //    UserInfos = new List<UserInfo>() {
            //new UserInfo{UserName="质量部-张三",UserId="2.1" },
            //new UserInfo{UserName="质量部-李四",UserId="2.2" },
            //new UserInfo{UserName="质量部-王五",UserId="2.3" }
            //}

            //});

            //departmentInfos.Add(new DepartmentInfo()
            //{
            //    DepartMentName = "工艺部",
            //    DepartmentId = "3",
            //    UserInfos = new List<UserInfo>() {
            //new UserInfo{UserName="工艺部-张三",UserId="3.1" },
            //new UserInfo{UserName="工艺部-李四",UserId="3.2" },
            //new UserInfo{UserName="工艺部-王五",UserId="3.3" }
            //}

            //});
            return departmentInfos;
        }
    }



    public record DynamicVehicleContext(int RandomNumber);

    public record CascadingDropDownContext(string Name, string[] DependsOnEvent, string[] DependsOnValue, IDictionary<string, string> DepValues, object? Context);
}

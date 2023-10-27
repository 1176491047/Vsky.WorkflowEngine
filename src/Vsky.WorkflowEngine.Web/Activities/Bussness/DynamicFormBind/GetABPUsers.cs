using Elsa.Attributes;
using Elsa.Design;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System;
using Volo.Abp.Identity;
using Vsky.WorkflowEngine.RoleUserInfos;
using Vsky.WorkflowEngine.Web.Activities.Other;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Elsa.Metadata;

namespace Vsky.WorkflowEngine.Web.Activities.Bussness.DynamicFormBind
{
    public class GetABPUsers : IActivityPropertyOptionsProvider
    {
        private IHttpClientFactory _httpClientFactory;
        private IConfiguration _configuration;
        private IIdentityRoleRepository _roleRepository;
        private IIdentityUserRepository _identityUserRepository;
        private IRoleUserRepository _roleUserRepository;

        public GetABPUsers(IRoleUserRepository roleUserRepository, IIdentityUserRepository identityUserRepository, IIdentityRoleRepository identityRoleRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _roleRepository = identityRoleRepository;
            _identityUserRepository = identityUserRepository;
            _roleUserRepository = roleUserRepository;
        }
        public object GetOptions(PropertyInfo property)
        {
            var persons = _identityUserRepository.GetListAsync().Result;
            HashSet<string> result = new HashSet<string>();
            result = persons.Select(x=>$"{x.Surname}{x.Name}[{x.UserName}]").ToHashSet();
            return result;
        }
    }
}

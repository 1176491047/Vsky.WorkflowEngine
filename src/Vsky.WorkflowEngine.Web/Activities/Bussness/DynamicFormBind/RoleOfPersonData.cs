using Elsa.Design;
using Elsa.Metadata;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Vsky.WorkflowEngine.RoleUserInfos;

namespace Vsky.WorkflowEngine.Web.Activities.Bussness.DynamicFormBind
{

    /// <summary>
    /// 获取人员角色
    /// </summary>
    public class RoleOfPersonData : IActivityPropertyOptionsProvider
    {
        private IHttpClientFactory _httpClientFactory;
        private IConfiguration _configuration;
        private IIdentityRoleRepository _roleRepository;
        private IIdentityUserRepository _identityUserRepository;
        private IRoleUserRepository _roleUserRepository;
        public RoleOfPersonData(IRoleUserRepository roleUserRepository, IIdentityUserRepository identityUserRepository, IIdentityRoleRepository identityRoleRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _roleRepository = identityRoleRepository;
            _identityUserRepository = identityUserRepository;
            _roleUserRepository= roleUserRepository;
        }
        public object GetOptions(PropertyInfo property)
        {
            var roles = _roleRepository.GetListAsync().Result;

            var peosons = _identityUserRepository.GetListAsync().Result;

            var relations = _roleUserRepository.GetListAsync().Result;

            List<SelectListItem> selectListItems = new List<SelectListItem>();
            return selectListItems;

        }
    }
}

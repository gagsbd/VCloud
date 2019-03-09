using InfraManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraManagement.Services
{
    public interface ICloudService
    {
        string GetServerUrl();
        string Authentiate();
        string CreateOrg(OrgInfo newOrg);
        string CreateAdminUser(string tenantId, string emailAddress,string userName,string password);
        bool IsOrgNameAvailable(string orgName);
        string CreateVDC(string tenantId);
        string GetVDC(string tenantId);
        string CreateCatalog(string tenantId);
        Task<string> UpdateEdgeGateWayToAdvanced(string tenantId);
        string GetTaskStatus(string taskEndPoint);
        bool IsAdminUserAvaialbe(string adminUsername);
        void EnableOrg(string cloudTenantId);
    }
}

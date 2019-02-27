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
        string Authentiate();
        string CreateOrg(OrgInfo newOrg);
        string CreateAdminUser(string orgHref, string emailAddress);
        bool IsOrgNameAvailable(string orgName);
        string CreateVDC(string orgHref);
        string GetVDC(string orgHref);
        string CreateCatalog(string orgHref);
        Task<string> UpdateEdgeGateWayToAdvanced(string orgHref);
        string GetTaskStatus(string taskEndPoint);

    }
}

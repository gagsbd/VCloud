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
        string CreateOrg(Org newOrg);
        string CreateAdminUser(string orgHref, string emailAddress);
        bool IsOrgNameAvailable(string orgName);
        string CreatedVDC(string orgHref);
        string GetVDC(string orgHref);
        string CreateCatalog(string orgHref);
        string UpdateEdgeGateWayToAdvanced(string edgeGatewayEndPoint);
        string GetTaskStatus(string taskEndPoint);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using InfraManagement.Models;

namespace InfraManagement.Services
{
    public class MockService : IPaymentGateway, ICloudService
    {
        public string Authentiate()
        {
            return "token";
        }

        public AuthResult Authorize(PaymentCard card)
        {
            return new AuthResult { IsAuthorized = true, PaymentProfileId = "1234", ProfileId = "098" }  ;
        }

        public string CreateAdminUser(string orgHref, string emailAddress)
        {
            return "done";
        }

        public string CreateCatalog(string orgHref)
        {
            return "done";
        }

        public string CreateOrg(OrgInfo newOrg)
        {
            return "done";
        }

        public string CreateVDC(string orgHref)
        {
            return "done";
        }

        public string GetTaskStatus(string taskEndPoint)
        {
            return "Completed";
        }

        public string GetVDC(string orgHref)
        {
            return "done";
        }

        public bool IsOrgNameAvailable(string orgName)
        {
            return true;
        }

        public Task<string> UpdateEdgeGateWayToAdvanced(string orgHref)
        {
            return Task.FromResult("done");
        }
    }
}
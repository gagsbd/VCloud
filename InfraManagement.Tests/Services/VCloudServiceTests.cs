using System;
using InfraManagement.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfraManagement.Tests.Services
{
    [TestClass]
    public class VCloudServiceTests
    {
        [TestMethod]
        public void TestCreateOrg()
        {
           
            var login = System.Configuration.ConfigurationManager.AppSettings["VCloud.Login"];
            var password = System.Configuration.ConfigurationManager.AppSettings["VCloud.Password"];
            var endPoint = System.Configuration.ConfigurationManager.AppSettings["VCloud.EndPoint"];
            var version = System.Configuration.ConfigurationManager.AppSettings["VCloud.ApiVersion"];
            ICloudService testService = new VCloudService(endPoint,version,login,password);
            testService.Authentiate();

        }
    }
}

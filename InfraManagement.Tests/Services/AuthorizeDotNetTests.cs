using System;
using System.Configuration;
using InfraManagement.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfraManagement.Tests.Services
{
    [TestClass]
    public class AuthorizeDotNetTests
    {
        [TestMethod]
        public void AuthorizeTest()
        {
            var subject = new AuthorizeDotNetService(ConfigurationManager.AppSettings.Get("PaymentGateway.Login"),
                ConfigurationManager.AppSettings.Get("PaymentGateway.Password"),
                ConfigurationManager.AppSettings.Get("PaymentGateway.EndPoint"));

            var result = subject.Authorize(new Models.PaymentCard {
                CCCVS="123", CCExpMonth=11, CCExpYear=2022, CCnumber= "4111111111111111", EmailAddress="myemail@mydomain.com"
            });

            Assert.IsTrue(String.IsNullOrEmpty(result.Error));
            Assert.IsTrue(result.IsAuthorized);
            Assert.IsTrue(!String.IsNullOrEmpty(result.ProfileId));

        }
    }
}

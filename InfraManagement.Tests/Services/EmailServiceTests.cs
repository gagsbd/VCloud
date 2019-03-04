using InfraManagement.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraManagement.Tests.Services
{
    [TestClass]
    public class EmailServiceTests
    {
        [TestMethod]
        public void SendTest()
        {
            var subject = new EmailService();
            subject.Send("Test","Test body","gags@mailinator.com") ;
        }
    }
}

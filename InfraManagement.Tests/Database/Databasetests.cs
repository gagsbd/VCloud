using System;
using InfraManagement.Database;
using InfraManagement.Database.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfraManagement.Tests.Database
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void CreateOrgTest()
        {
            using (var contx = new TenantDatabase())
            {
                var id = contx.CreateOrg(new InfraManagement.Database.Entity.OrgEntity()
                {
                    Address = new AddressEntity { Address1 = "Line1", City = "Cville", Country = "USA", State = "VA", Zip = "22222" },
                    CompanyFullName = "My company is fully named",
                    CompanyShortName = "my_company_short_name",
                    CustomerPaymentProfileId = "234",
                    CustomerProfileId = "543",
                    EmailAddress = "email@address.com",
                    Url = "https//vdirect.url"
                });

                Assert.IsTrue(id > 0);

                var newOrg = contx.GetOrgById(id);
                Assert.IsNotNull(newOrg);
            }
        }

        [TestMethod]
        public void CreateTaskTest()
        {
            using (var contx = new TenantDatabase())
            {
                var id = contx.CreateTask(new InfraManagement.Database.Entity.TaskEntity()
                {
                    IsLRP = false,
                    Name = "Create Admin",
                    Notes = "Some Notes",
                    OrgId = 1,
                    Status = "Not Started",
                    StatusUrl = "https://",
                    TaskType = 300
                });

                Assert.IsTrue(id > 0);

                var newTask = contx.GetOrgTasks(1).Find(t=>t.Id == id);
                Assert.IsNotNull(newTask);
            }
        }

        [TestMethod]
        public void CreateVdcTest()
        {
            using (var contx = new TenantDatabase())
            {
                var id = contx.CreateVdc(new InfraManagement.Database.Entity.VdcEntity()
                {
                     AdminUserHref="http",
                     Href="https://href"
                     
                });

                Assert.IsTrue(id > 0);

                var newTask = contx.GetOrgTasks(1).Find(t => t.Id == id);
                Assert.IsNotNull(newTask);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InfraManagement.Database.Entity;

namespace InfraManagement.Database
{
    public class MockDb : ITenantDatabase
    {
        public int CreateOrg(OrgEntity org)
        {
            return 10001;
        }

        public int CreateTask(TaskEntity task)
        {
            return 2001;
        }

        public int CreateVdc(VdcEntity vdc)
        {
            return 30001;
        }

        public OrgEntity GetOrgById(int orgId)
        {
            return new OrgEntity
            {
                CompanyFullName = "Mocked Org",
                CompanyShortName = "mock",
                CustomerPaymentProfileId = "1",
                CustomerProfileId = "1",
                EmailAddress = "mocked@moked.com",
                Id = 1,
                Url = "https://org/1"
            };
        }

        public List<TaskEntity> GetOrgTasks(int orgId)
        {
            var result = new List<TaskEntity> {
                new TaskEntity{ Id =1, Name ="Create Admin User", Status="Completed" },
                new TaskEntity{ Id =2, Name ="Create VDC", Status="Running" },
                new TaskEntity{ Id =1, Name ="Create Catalog", Status="Not Strted" },
                new TaskEntity{ Id =1, Name ="Update Edge gateway", Status="Not Strted" },

            };
            return result;
        }

        public void UpdateTask(TaskEntity task)
        {
            return;
        }

        public void UpdateTaskStatus(int orgId, int taskType, string newStatus)
        {
            return;
        }
    }
}
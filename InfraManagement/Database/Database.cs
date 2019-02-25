using InfraManagement.Database.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace InfraManagement.Database
{
    public class TenantDatabase : DbContext
    {
        public TenantDatabase() : base()
        { }

        public DbSet<OrgEntity> Org { get; set; }
        public DbSet<VdcEntity> Vdc { get; set; }
        public DbSet<TaskEntity> Task { get; set; }

        public int CreateOrg(OrgEntity org)
        {
            this.Org.Add(org);
            return this.SaveChanges();
        }

        public int CreateVdc(VdcEntity vdc)
        {
            this.Vdc.Add(vdc);
            return this.SaveChanges();
        }
        public int CreateTask(TaskEntity task)
        {
            
            this.Task.Add(task);
            return this.SaveChanges();
        }

        public List<TaskEntity> GetOrgTasks(int orgId)
        {
           //var result =  this.Task.SelectMany(t => t.OrdId == orgId);
            return null;
        }
    }
}
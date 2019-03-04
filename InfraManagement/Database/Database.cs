using InfraManagement.Database.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace InfraManagement.Database
{
    public class TenantDatabase : DbContext, ITenantDatabase
    {
        public TenantDatabase() : base("tenantdb")
        { }

        public virtual DbSet<OrgEntity> Org { get; set; }
        public virtual DbSet<VdcEntity> Vdc { get; set; }
        public virtual DbSet<TaskEntity> Task { get; set; }

        //public virtual DbSet<Org> Orgs { get; set; }
        //public virtual DbSet<Task> Tasks { get; set; }
        //public virtual DbSet<Vdc> Vdcs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }

        public int CreateOrg(OrgEntity org)
        {
           
            this.Org.Add(org);
            this.SaveChanges();
            return org.Id;
        }

        public int CreateVdc(VdcEntity vdc)
        {
            this.Vdc.Add(vdc);
            this.SaveChanges();
            return vdc.Id;
        }
        public int CreateTask(TaskEntity task)
        {
            
            this.Task.Add(task);
            this.SaveChanges();
            return task.Id;
        }

        public void UpdateTaskStatus(int orgId,int taskType,string newStatus)
        {
            var task = this.Task.Where(t=>t.TaskType == taskType && t.OrgId == orgId)?.First();

            if (task != null)
            {
                task.Status = newStatus;
            }
            this.SaveChanges();

        }

        public void UpdateTask(TaskEntity task)
        {
            
            if (task != null)
            {
               var taskToUpdate = this.Task.SingleOrDefault(t => t.Id == task.Id);

                if (taskToUpdate != null)
                {
                    taskToUpdate.IsLRP = taskToUpdate.IsLRP;
                    taskToUpdate.Name = task.Name;
                    taskToUpdate.Notes = task.Notes;
                    taskToUpdate.Status = task.Status;
                    taskToUpdate.StatusUrl = task.StatusUrl;
                    taskToUpdate.TaskType = task.TaskType;
                    
                }

                //this.Task.Add(task);  
                this.SaveChanges();
            }
            

        }

        public List<TaskEntity> GetOrgTasks(int orgId)
        {
          
           var result =  this.Task.Where(t => t.OrgId == orgId)?.ToList();
           return result;
        }

        public System.Data.Entity.DbSet<InfraManagement.Models.ServiceTask> ServiceTasks { get; set; }

        public OrgEntity GetOrgById(int orgId)
        {
            return this.Org.FirstOrDefault<OrgEntity>(o=>o.Id == orgId);
        }

        public OrgEntity GetOrgByTenantId(string tenantId)
        {
            return this.Org.FirstOrDefault<OrgEntity>(o => o.TenantId == tenantId);
        }

        public void UpdateOrg(OrgEntity org)
        {
            this.Org.Attach(org);
            this.Entry<OrgEntity>(org).Property(nameof(org.Cloud_TenantId)).IsModified = true;
            this.SaveChanges();
        }
    }
}
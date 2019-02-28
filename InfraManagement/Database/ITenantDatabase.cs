using System.Collections.Generic;
using InfraManagement.Database.Entity;

namespace InfraManagement.Database
{
    public interface ITenantDatabase
    {
        int CreateOrg(OrgEntity org);
        int CreateTask(TaskEntity task);
        int CreateVdc(VdcEntity vdc);
        OrgEntity GetOrgById(int orgId);
        List<TaskEntity> GetOrgTasks(int orgId);
        void UpdateTask(TaskEntity task);
        void UpdateTaskStatus(int orgId, int taskType, string newStatus);
    }
}
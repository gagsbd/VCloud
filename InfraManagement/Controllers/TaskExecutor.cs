using InfraManagement.Database.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfraManagement.Controllers
{
    public class TaskExecutor
    {
        TaskEntity _task;
        public TaskExecutor(TaskEntity task)
        {
            _task = task;
        }

    }
}
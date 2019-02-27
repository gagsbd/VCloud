using InfraManagement.Database.Entity;
using InfraManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfraManagement.App_Start
{
    public static class AutomapperConfig
    {

        public static void Config()
        {

          
            AutoMapper.Mapper.Initialize(cfg => {
                cfg.CreateMap<OrgInfo, OrgEntity>();
                cfg.CreateMap<ServiceTask, TaskEntity>();
            });

         
        }
    }
}
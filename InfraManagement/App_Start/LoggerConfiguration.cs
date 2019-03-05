using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfraManagement.App_Start
{
    public static class LoggerConfiguration
    {
        public static void Config()
        {
            //This will read the configuration form the web.config fie  sets up the log4net frame for use.
            XmlConfigurator.Configure();
        }
    }
}
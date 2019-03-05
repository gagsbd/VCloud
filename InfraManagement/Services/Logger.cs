using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfraManagement.Services
{
    public class Logger : ILogger
    {
        private readonly ILog _internalLogger;
        public Logger()
        {
            _internalLogger = LogManager.GetLogger("WebLogs");
        }
        public void Error(object message, Exception exception)
        {
            _internalLogger.Error(message, exception);
        }
    }
}
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraManagement.Services
{
    public interface ILogger 
    {
        void Error(object message, Exception exception);
    }
}

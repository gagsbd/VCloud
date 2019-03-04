using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraManagement.Services
{
    public interface INotificationService
    {
        void Send(string subject,string message, string sendTo);
    }
}

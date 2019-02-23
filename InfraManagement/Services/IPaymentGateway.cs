using InfraManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraManagement.Services
{
    public interface IPaymentGateway
    {
        AuthResult Authorize(PaymentCard card);
    }

    public class AuthResult
    {
        public bool IsAuthorized { get; set; }
        public string ProfileId { get; set; }
        public string Error { get; set; }
        public bool IsError { get; internal set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfraManagement.Models
{
    public class ServiceTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StatusUrl { get; set; }
        public string Status { get; set; }
    }
}
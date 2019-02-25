using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfraManagement.Models
{
    public class VdcInfo
    {
        public int OrgId { get; set; }
        public string OrdVdc { get; set; }
        public string OrgType { get; set; }
        public string Href { get; set; }
        public string OrdEdgeGateWay { get; set; }
        public string OrgAdminRole { get; set; }
        public string AdminUserHref { get; set; }
    }
}
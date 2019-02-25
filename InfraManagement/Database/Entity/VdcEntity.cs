using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InfraManagement.Database.Entity
{
    [Table("Vdc")]
    public class VdcEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrgId { get; set; }
        public string OrdVdc { get; set; }
        public string OrgType { get; set; }
        public string Href { get; set; }
        public string OrdEdgeGateWay { get; set; }
        public string OrgAdminRole { get; set; }
        public string AdminUserHref { get; set; }
    }
}
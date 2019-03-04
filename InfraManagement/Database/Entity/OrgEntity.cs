using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InfraManagement.Database.Entity
{
    [Table("Org")]
    public class OrgEntity
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  //This si the primary key just for the program to identify the org, do not expose it on URLs or like
       // public string Url { get; set; }
        public string TenantId { get; set; }     //This would be the Guid to identify the org in db
                                                 //This is the id exposed publically in urls etc
        public string Cloud_TenantId { get; set; }  //This is the id for the org created in cloud
        public AddressEntity Address { get; set; }
        public string CustomerPaymentProfileId { get; set; }
        public string CustomerProfileId { get; set; }
        [DisplayName("Company Short Name:")]
        public string CompanyShortName { get; set; }
        public string CompanyFullName { get; set; }
        public string EmailAddress { get; set; }
        public string AdminName { get; set; }
        public string AdminPassword { get; set; }
    }
}
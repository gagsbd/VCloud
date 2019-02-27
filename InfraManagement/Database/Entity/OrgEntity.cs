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
        public int Id { get; set; }
        public string Url { get; set; }
       
        public AddressEntity Address { get; set; }
        public string CustomerPaymentProfileId { get; set; }
        public string CustomerProfileId { get; set; }
        [DisplayName("Company Short Name:")]
        public string CompanyShortName { get; set; }
        public string CompanyFullName { get; set; }
        public string EmailAddress { get; set; }
    }
}
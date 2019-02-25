using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace InfraManagement.Models
{
    public class OrgInfo
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public PaymentCard Card { get; set; }
        public Address Address { get; set; }
        public string CustomerPaymentProfileId{ get; set; }
        public string CustomerProfileID{ get; set; }
       
        public string AdminOps{ get; set; }
        [DisplayName("Company Short Name:")]
        public string CompanyShortName { get; set; }
        public string CompanyFullName { get; set; }
        public string EmailAddress { get; set; }
             
        
    }
}
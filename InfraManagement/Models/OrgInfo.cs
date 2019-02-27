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
        public PaymentCard Card { get; set; }
        public Address Address { get; set; }
        public string CustomerPaymentProfileId{ get; set; }
      
        [DisplayName("Company Short Name:")]
        public string CompanyShortName { get; set; }

        [DisplayName("Company Full Name:")]
        public string CompanyFullName { get; set; }

        [DisplayName("Email:")]
        public string EmailAddress { get; set; }
             
        
    }
}
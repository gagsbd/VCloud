using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfraManagement.Models
{
    public class OrgInfo
    {
        public string Id { get; set; }
       
        public Address Address { get; set; }
        public string CustomerPaymentProfileId{ get; set; }
      
        [DisplayName("Company Short Name:")]
        [Required(ErrorMessage="Company Short Name is required.")]
        public string CompanyShortName { get; set; }

        [DisplayName("Company Full Name:")]
        [Required(ErrorMessage = "Company Full Name is required.")]
        public string CompanyFullName { get; set; }

        [DisplayName("Admin User Name:")]
        [Required(ErrorMessage = "Admin User Name is required.")]
        public string AdminName { get; set; }

        [DisplayName("Admin User Password:")]
        [Required(ErrorMessage = "Admin User Password is required.")]
        public string AdminPassword { get; set; }

        [DisplayName("Email:")]
        [Required(ErrorMessage = "Email is required.")]
        public string EmailAddress { get; set; }
        public string CustomerProfileId { get;  set; }
    }
}
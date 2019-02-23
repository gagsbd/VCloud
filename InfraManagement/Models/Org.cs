using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace InfraManagement.Models
{
    public class Org
    {
        public string Url { get; set; }
        [DisplayName("Credit Card Number:")]
        public string CCnumber{ get; set; }
        [DisplayName("Card Expiry Month:")]
        public string CCExpMonth{ get; set; }
        [DisplayName("Card Expiry Year:")]
        public int CCExpYear{ get; set; }
        [DisplayName("CCV:")]
        public string CCCVS{ get; set; }
        [DisplayName("First Name:")]
        public string FirstNam{ get; set; }
        [DisplayName("Last Name:")]
        public string LastName{ get; set; }
        public string Zip{ get; set; }
        public string Address1{ get; set; }
        public string Address2{ get; set; }
        public string State{ get; set; }
        public string Country{ get; set; }
        public string VCloudShortName{ get; set; }
        public string VCloudLongName{ get; set; }
        public bool Production{ get; set; }
        public string RefId{ get; set; }
        public string Company{ get; set; }
        public string City{ get; set; }
        public string CustomerProfilePaymentId{ get; set; }
        public bool Authorized{ get; set; }
        public string CustomerProfileID{ get; set; }
        public bool AlreadyExists{ get; set; }
        public string AdminOps{ get; set; }
        [DisplayName("Company Short Name:")]
        public string CompanyShortName { get; set; }
        public string Id{ get; set; }
        public string OrdVdc{ get; set; }
        public string OrgType{ get; set; }
        public string AuthString{ get; set; }
        public string CompanyFullName{ get; set; }
        public string EmailAddress{ get; set; }
        public string AdminPassword{ get; set; }
        public string AdminName{ get; set; }
        public string AuthToken{ get; set; }
        public string Href{ get; set; }
        public bool Verified{ get; set; }
        public string OrdEdgeGateWay{ get; set; }
        public string OrgAdminRole{ get; set; }
        public string Task{ get; set; }
        public string TaskHref{ get; set; }
        public string AdminUserHref{ get; set; }
    }
}
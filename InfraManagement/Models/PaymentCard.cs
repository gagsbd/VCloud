using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace InfraManagement.Models
{
    public class PaymentCard
    {
        [DisplayName("Card Number:")]
        public string CCnumber { get; set; }
        [DisplayName("Card Expiry Month:")]
        public string CCExpMonth { get; set; }
        [DisplayName("Card Expiry Year:")]
        public int CCExpYear { get; set; }
        [DisplayName("CCV:")]
        public string CCCVS { get; set; }
        [DisplayName("First Name:")]
        public string FirstName { get; set; }
        [DisplayName("Last Name:")]
        public string LastName { get; set; }
        [DisplayName("Email:")]
        public string EmailAddress { get; internal set; }
        public Address BillingAddress { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InfraManagement.Models
{
    public class PaymentCard
    {
        [DisplayName("Card Number:")]
        [Required(ErrorMessage ="Card number is required."),CreditCard(ErrorMessage = "Card number is not valid.")]
        public string CCnumber { get; set; }

        [DisplayName("Card Expiry Month:")]
        [Required(ErrorMessage = "Card Expiry month is required.")]
        [ MaxLength(2,ErrorMessage ="Enter 2 digit month")]
        public string CCExpMonth { get; set; }

        [DisplayName("Card Expiry Year:")]
        [Required(ErrorMessage = "Card Expiry year is required.")]
        [MaxLength(4, ErrorMessage = "Enter 4 digit year")]
        public int CCExpYear { get; set; }

        [DisplayName("CCV:")]
        [Required(ErrorMessage = "Card CCV required.")]
        public string CCCVS { get; set; }


        [DisplayName("First Name:")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }


        [DisplayName("Last Name:")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [DisplayName("Email:")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is not valid.")]
        public string EmailAddress { get; internal set; }

        public Address BillingAddress { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfraManagement.Models
{
    public class Address
    {
        public string Zip { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}
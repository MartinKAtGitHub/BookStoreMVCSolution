﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookStoreMVC.Models
{
    public class Company
    {
        public int Id { get; set; }
       
        [Required]
        public string Name { get; set; }
       
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        
        public string City { get; set; }
        
        public string State { get; set; }
        
        [Display(Name = "Street Address")]
        public string PostalCode { get; set; }
        
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Is Authorized Company")]
        public bool IsAuthorizedCompany { get; set; }
    }
}

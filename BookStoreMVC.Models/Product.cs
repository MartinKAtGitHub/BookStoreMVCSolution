﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;
using System.Text;

namespace BookStoreMVC.Models
{
    public class Product
    {
        public int Id { get; set; } // No need for [Key] attribute
        [Required]
        public string Title { get; set; } 
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        public string ImageUrl { get; set; }

        [Required]
        [Range(1,10000)]
        public double ListPrice { get; set; }

        [Required]
        [Range(1, 10000)]
        public double Price { get; set; } // Use this if order is less then 50

        [Required]
        [Range(1, 10000)]
        public double Price50 { get; set; } // use this is order is 50-99
        
        [Required]
        [Range(1, 10000)]
        public double Price100 { get; set; }// use this if order is 100+


        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required]
        public int CoverTypeId { get; set; }

        [ForeignKey("CoverTypeId ")]
        public CoverType CoverType { get; set; }
    }
}

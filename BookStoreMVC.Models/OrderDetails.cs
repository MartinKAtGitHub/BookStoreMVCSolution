﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BookStoreMVC.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }

        public int Count { get; set; }
        public double Price { get; set; }
        
        /// <summary>
        /// OrderId is the order header id
        /// </summary>
        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public OrderHeader OrderHeader { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}

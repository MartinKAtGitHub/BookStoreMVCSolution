using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BookStoreMVC.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }

        public int Id { get; set; }

        /// <summary>
        /// How many of the product this cart is holding.
        /// </summary>
        [Range(1,1000, ErrorMessage ="Please enter a value between 1 and 1000")]
        public int Count { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId ")]
        public ApplicationUser ApplicationUser{ get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId ")]
        public Product Product { get; set; }

        [NotMapped]
        public double Price { get; set; }
    }
}

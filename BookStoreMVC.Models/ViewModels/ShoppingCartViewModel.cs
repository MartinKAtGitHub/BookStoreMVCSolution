﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreMVC.Models.ViewModels
{
   public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }

    }
}

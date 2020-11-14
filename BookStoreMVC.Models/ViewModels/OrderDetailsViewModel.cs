using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreMVC.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetailsObjects { get; set; }
    }
}

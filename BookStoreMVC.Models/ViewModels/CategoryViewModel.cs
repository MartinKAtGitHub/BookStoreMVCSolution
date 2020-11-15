using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreMVC.Models.ViewModels
{
    public class CategoryViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}

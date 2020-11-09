using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreMVC.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; } // SelectListItem needs a Nuget to work | Enables dropdown 
        public IEnumerable<SelectListItem> CoverTypeList { get; set; } // SelectListItem needs a Nuget to work | Enables dropdown 
    }
}

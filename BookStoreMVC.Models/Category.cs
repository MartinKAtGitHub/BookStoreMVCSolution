using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookStoreMVC.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name= "Category name")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}

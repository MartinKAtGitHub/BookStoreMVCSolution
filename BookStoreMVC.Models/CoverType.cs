using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookStoreMVC.Models
{
    public class CoverType
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Cover Type")]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}

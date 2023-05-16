﻿using System.ComponentModel.DataAnnotations;

namespace MyWork.Models
{
    public class Verification
    {
        [Required]
        [Display(Name = "Email* : ")]
        public string Email { get; set; }
         
        [Display(Name = "Code : ")]
        public string Code { get; set; }
    }
}

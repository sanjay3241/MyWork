using MyWork.Controllers;
using System.ComponentModel.DataAnnotations;

namespace MyWork.Models;

public class Login 
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Full Name* : ")]
    public string Username { get; set; }

    [Required]
    [Display(Name = "Email* : ")]
    public string Email { get; set; }

    [Required]
    [Display(Name = "Password* : ")]
    public string Password { get; set; }
    [Required]
    [Display(Name = "Confirm Password* : ")]
    public string Confirm_Password { get; set; }
    [Required]
    [Display(Name = "Full Name* : ")]
    public string Name { get; set; }
    [Required]
    [Display(Name = "Country* : ")]
    public string Country { get; set; }
    public string Token { get; set; }
}


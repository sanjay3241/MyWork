using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyWork.Models
{
    public class FullDetails
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name* : ")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name* : ")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email* : ")]
        public string Email { get; set; }

        public string Password { get; set; }

        [Required]
        [Display(Name = "Full Name : ")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Country : ")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Account Type : ")]
        public string Account_Type { get; set; }

        [Display(Name = "Contact No: ")]
        public string PhoneNo { get; set; }

        [Display(Name = "City : ")]
        public string City { get; set; }

        [Display(Name = "Post Code : ")]
        public string Post_Code { get; set; }

        [Display(Name = "Address1 : ")]
        public string Address1 { get; set; }

        [Display(Name = "Address2 : ")]
        public string Address2 { get; set; }

        [Display(Name = "Skill [Seperate By Space] : ")]
        public string RecomendationDesc { get; set; }

        [Display(Name = "Recomendation: ")]
        public string Recomendation { get; set; }
        public string Token { get; set; }
        public string IsActive { get; set; }
        public string Username { get; set; }

         
        [Display(Name = "Level: ")]
        public string Level { get; set; }
         
        [Display(Name = "Description: ")]
        public string Description { get; set; }
         
        [Display(Name = "Join Date : ")]
        public string JoinDate { get; set; }
         
        [Display(Name = "Course Interval : ")]
        public string CourseInterval { get; set; } 
    }

    public class RecomendDetails
    { 
        public int FreelancerId { get; set; }
        public string ClientName { get; set; }
        public int JobId { get; set; }
        public string Title { get; set; }
        public string Skill { get; set; }
    }
}

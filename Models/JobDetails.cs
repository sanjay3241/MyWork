using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyWork.Models
{
    public class JobDetails
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title* : ")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description* : ")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Interval* : ")]
        public string Interval { get; set; }

        [Required]
        [Display(Name = "Budget* : ")]
        public string Budget { get; set; }

        [Required]
        [Display(Name = "Skill [Note: Seperate By Space]* ")]
        public string Skill { get; set; }
        public string PostedDate { get; set; }
        public int UserId { get; set; }
        public int FreelancerId { get; set; }

    }
}

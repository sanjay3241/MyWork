using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyWork.Models
{
    public class dbUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Account_Type { get; set; }
        public string PhoneNo { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string RecomendationDesc { get; set; }
        public string Recomendation { get; set; }
        public string Token { get; set; }
        public string IsActive { get; set; }
        public string Username { get; set; }
    }
    public class CourseDetails
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public string JoinDate { get; set; }
        public int CourseInterval { get; set; }
    }

}

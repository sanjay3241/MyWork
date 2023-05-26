using Microsoft.AspNetCore.Mvc;
using MyWork.Models;
using MyWork.Support;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using Dapper;


namespace MyWork.Controllers
{
    public class UsersController : MasterController
    {
        [SessionCheck]
        public IActionResult Index()
        {
            return View();
        }

        [SessionCheck]
        public IActionResult UserProfileDetails()
        {
            return View();
        }

        [SessionCheck]
        public JsonResult loadProfileDetails()
        {
            var userDetails = new dbUser();
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    userDetails = con.Query<dbUser>("Select * From dbo.[User] Where Email=@Email", new { @Email = HttpContext.Session.GetString("Email") }).FirstOrDefault();

                }
                catch (Exception Ex)
                {
                    return Json(new
                    {
                        msg = Ex.Message,
                        success = false
                    });
                };
            }

            return Json(new
            {
                data = userDetails,
                success = true
            });
        }

        [SessionCheck]
        public JsonResult saveCourseDetails(string Level, string Description, string JoinDate, string Interval)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                using (var tran = con.BeginTransaction())
                {

                    try
                    {
                        con.Execute(@"Insert Into dbo.CourseDetails(UserId,Level,Description,JoinDate,CourseInterval) 
Values(@UserId,@Level,@Description,@JoinDate,@CourseInterval)", new
                        {
                            @UserId = HttpContext.Session.GetInt32("Id"),
                            @Level = Level,
                            @Description = Description,
                            @JoinDate = JoinDate,
                            @CourseInterval = Interval
                        }, transaction: tran);

                        tran.Commit();
                    }
                    catch (Exception Ex)
                    {
                        tran.Rollback();
                        return Json(new
                        {
                            msg = Ex.Message,
                            success = false
                        });
                    };
                }
            }

            return Json(new
            {
                msg = "Course Added Successfully !",
                success = true
            });
        }

        [SessionCheck]
        public JsonResult loadCourseDetails()
        {
            var courseDetails = new List<CourseDetails>();
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    courseDetails = con.Query<CourseDetails>("Select * From dbo.[CourseDetails] Where UserId=@Id", new { @Id = HttpContext.Session.GetInt32("Id") }).ToList();

                }
                catch (Exception Ex)
                {
                    return Json(new
                    {
                        msg = Ex.Message,
                        success = false
                    });
                };
            }

            return Json(new
            {
                data = courseDetails,
                success = true
            });
        }

        [SessionCheck]
        public JsonResult userLogout()
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    HttpContext.Session.Clear();
                }
                catch (Exception Ex)
                {
                    return Json(new
                    {
                        msg = Ex.Message,
                        success = false
                    });
                };
            }

            return Json(new
            {
                msg = "Session Cleared Successfully",
                success = true
            });
        }

        [SessionCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult updateUserDetails(FullDetails model)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    con.Execute(@"Update [dbo].[User] Set FirstName=@FirstName,LastName=@LastName,Email=@Email,
Country=@Country,City=@City,Address1=@Address1,Address2=@Address2,PostCode=@PostCode,PhoneNo=@PhoneNo,Recomendation=@Recomendation,RecomendationDesc=@RecomendationDesc Where Id=@Id", new
                    {
                        @Id = HttpContext.Session.GetInt32("Id"),
                        @FirstName = model.FirstName,
                        @LastName = model.LastName,
                        @Email = model.Email,
                        @Country = model.Country,
                        @City = model.City,
                        @Address1 = model.Address1,
                        @Address2 = model.Address2,
                        @PostCode = model.Post_Code,
                        @PhoneNo = model.PhoneNo,
                        @Recomendation= model.Recomendation,
                        @RecomendationDesc=model.RecomendationDesc
                    });
                }
                catch (Exception Ex)
                {
                    return Json(new
                    {
                        msg = Ex.Message,
                        success = false
                    });
                };

                return Json(new
                {
                    msg = "Account Updated Successfully",
                    success = true
                });
            }
        }
    }

}

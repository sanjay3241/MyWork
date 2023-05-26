using Microsoft.AspNetCore.Mvc;
using MyWork.Models;
using MyWork.Support;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace MyWork.Controllers
{
    public class FreelancerController : MasterController
    {

        [SessionCheck]
        public IActionResult Index()
        {
            return View();
        }

        [SessionCheck]
        public IActionResult FindJob()
        {
            return View();
        }

        [SessionCheck]
        public IActionResult MyJob()
        {
            return View();
        }

        [SessionCheck]
        public IActionResult Notification()
        {
            return View();
        }

        [SessionCheck]
        public IActionResult Message()
        {
            return View();
        }

        [SessionCheck]
        public JsonResult loadJobDetails()
        {
            var jobDetails = new List<JobDetails>();
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    jobDetails = con.Query<JobDetails>("Select * From dbo.[JobDetails]").ToList();

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
                data = jobDetails,
                success = true
            });
        }

        [SessionCheck]
        public JsonResult applyJob(string UserId, string Id)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    if (con.Query<bool>("Select * from dbo.TransactionInfo Where FreelancerId=@FreelancerId and @JobId=JobId", new { @FreelancerId = HttpContext.Session.GetInt32("Id"), @JobId = Id }).FirstOrDefault())
                    {
                        return Json(new
                        {
                            msg = "Already Applied For the Job!",
                            success = false
                        });
                    }
                    else
                    {
                        con.Execute(@"Insert Into dbo.TransactionInfo (FreelancerId,ClientId,JobId,RegisteredDate,TranType) Values(@FreelancerId,@ClientId,@JobId,@RegisteredDate,@TranType)", new
                        {
                            @FreelancerId = HttpContext.Session.GetInt32("Id"),
                            @ClientId = UserId,
                            @JobId = Id,
                            @RegisteredDate = DateTime.Now,
                            @TranType = "Apply"
                        });
                    }
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
                msg = "Applied Successfully",
                success = true
            });
        }

        [SessionCheck]
        public JsonResult loadAppliedJob()
        {
            var jobDetails = new List<JobDetails>();
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    jobDetails = con.Query<JobDetails>(" Select JD.* From dbo.[JobDetails] JD Inner Join dbo.TransactionInfo TI on TI.JobId=JD.Id Inner Join dbo.[User] U On U.Id=@Id Where TI.TranType='Apply' ", new { @Id = HttpContext.Session.GetInt32("Id") }).ToList();

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
                data = jobDetails,
                success = true
            });
        }

        [SessionCheck]
        public JsonResult loadAssignedJob()
        {
            var jobDetails = new List<JobDetails>();
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    jobDetails = con.Query<JobDetails>(" Select JD.* From dbo.[JobDetails] JD Inner Join dbo.TransactionInfo TI on TI.JobId=JD.Id Inner Join dbo.[User] U On U.Id=@Id Where TI.TranType='Assign' ", new { @Id = HttpContext.Session.GetInt32("Id") }).ToList();

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
                data = jobDetails,
                success = true
            });
        }

        [SessionCheck]
        public JsonResult loadNotification()
        {
            var allNotification = new List<Notification>();
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    allNotification = con.Query<Notification>("Select N.Description,U.FirstName as FreelancerName,N.RegisteredDate  from dbo.Notification N Inner Join dbo.[User] U On U.Id=N.FreelancerId Where FreelancerId=@Id", new { @Id = HttpContext.Session.GetInt32("Id") }).ToList();

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
                data = allNotification,
                success = true
            });
        }
    }
}

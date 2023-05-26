using Microsoft.AspNetCore.Mvc;
using MyWork.Models;
using MyWork.Support;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using System.Reflection;

namespace MyWork.Controllers
{
    public class ClientController : MasterController
    {
        [SessionCheck]
        public IActionResult Index()
        {
            return View();
        }

        [SessionCheck]
        public IActionResult PostJob()
        {
            return View("Post-Job");
        }

        [SessionCheck]
        public IActionResult AllJob()
        {
            return View("All-Job");
        }

        [SessionCheck]
        public IActionResult Message()
        {
            return View();
        }

        [SessionCheck]
        public IActionResult Notification()
        {
            return View();
        }

        [SessionCheck]
        public IActionResult TalentRequest()
        {
            return View();
        }

        [SessionCheck]
        public IActionResult TalentList()
        {
            return View();
        }

        [SessionCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult saveJobDetails(JobDetails model)
        {
            var userListForRecomendation = new List<FullDetails>();
            var toBeRecomended = new List<RecomendDetails>();
            var skillList = new List<string>();
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    var jobId = con.Query<int>("Select Id From dbo.JobDetails Order By Id Desc").FirstOrDefault();
                    userListForRecomendation=con.Query<FullDetails>("Select * from dbo.[User] Where Recomendation='On'").ToList();
                    
                    foreach(var item in userListForRecomendation)
                    {
                        var skills = item.RecomendationDesc.Split(" ");

                        for(int i = 0; i < skills.Length; i++)
                        {
                            skillList.Add(skills[i]);
                        }
                         
                        foreach(var skill in skillList)
                        {
                            if (model.Skill.Contains(skill) && skill !="")
                            {
                                toBeRecomended.Add(new RecomendDetails { FreelancerId = item.Id, ClientName = HttpContext.Session.GetString("FirstName"), Skill = skill, Title = model.Title, JobId = (jobId + 1) });
                            }
                        }

                    }

                    foreach(var item in toBeRecomended)
                    { 
                        con.Execute("Insert Into Dbo.Notification(FreelancerId,ClientId,RegisteredDate,Description,JobId) Values(@FreelancerId,@ClientId,@RegisteredDate,@Description,@JobId)",
                            new
                            {
                                @FreelancerId=item.FreelancerId,
                                @ClientId=HttpContext.Session.GetInt32("Id"),
                                @RegisteredDate=DateTime.Now,
                                @JobId = item.JobId,
                                @Description ="You Have Recomendation For "+ item.Title+" In "+item.Skill+" By "+item.ClientName,
                            });

                    }

                    con.Execute(@"INSERT INTO [dbo].[JobDetails](UserId,Title,Description,Interval,Budget,Skill,IsAssigned,PostedDate) 
Values(@UserId,@Title,@Description,@Interval,@Budget,@Skill,@IsAssigned,@PostedDate)", new
                    {
                        @UserId = HttpContext.Session.GetInt32("Id"),
                        @Title = model.Title,
                        @Description = model.Description,
                        @Interval = model.Interval,
                        @Budget = model.Budget,
                        @Skill = model.Skill,
                        @IsAssigned = 0,
                        @PostedDate = DateTime.Now
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
                    msg = "Job Posted Successfully",
                    success = true
                });
            }
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
                    jobDetails = con.Query<JobDetails>("Select * From dbo.[JobDetails] Where UserId=@Id", new { @Id = HttpContext.Session.GetInt32("Id") }).ToList();

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
        public JsonResult updateJobDetails(int Id, string Title, string Description, string Interval, string Budget, string Skill)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                using (var tran = con.BeginTransaction())
                {

                    try
                    {
                        con.Execute(@"Update dbo.JobDetails Set  Title=@Title,Description=@Description,Interval=@Interval,Budget=@Budget,Skill=@Skill Where Id=@Id ", new
                        {
                            @Id = Id,
                            @Title = Title,
                            @Description = Description,
                            @Interval = Interval,
                            @Budget = Budget,
                            @Skill = Skill,
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
                msg = "Job Updated Successfully !",
                success = true
            });
        }
        [SessionCheck]
        public JsonResult deleteJobDetails(int Id)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                using (var tran = con.BeginTransaction())
                {

                    try
                    {
                        con.Execute(@"Delete From dbo.JobDetails Where Id=@Id ", new { @Id = Id }, transaction: tran);

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
                msg = "Job Deleted Successfully !",
                success = true
            });
        }

        [SessionCheck]
        public JsonResult assignJob(int Id, string FreelancerId)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                using (var tran = con.BeginTransaction())
                {

                    try
                    {
                        if (!con.Query<bool>("Select * from dbo.TransactionInfo Where FreelancerId=@FreelancerId and JobId=@JobId and ClientId=@ClientId and TranType='Assign'", new { @FreelancerId =FreelancerId,@JobId=Id,@ClientId=HttpContext.Session.GetInt32("Id")}, transaction: tran).FirstOrDefault())
                        {
                            con.Execute(@"Update dbo.TransactionInfo Set  TranType='Assign' Where JobId=@Id and FreelancerId=@FId ", new
                            {
                                @Id = Id,
                                @FId = FreelancerId
                            }, transaction: tran);

                        }
                        else
                        { 
                            tran.Rollback();
                            return Json(new
                            {
                                msg ="Job Already Assigned",
                                success = false
                            });
                        }
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
                msg = "Job Assigned Successfully !",
                success = true
            });
        }

        [SessionCheck]
        public JsonResult loadAllRequestedJob()
        {
            var jobDetails = new List<JobDetails>();
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    jobDetails = con.Query<JobDetails>("Select JD.*,TI.FreelancerId  From dbo.[JobDetails] JD Inner Join dbo.TransactionInfo TI on TI.JobId=JD.Id Where TI.TranType='Apply' and TI.ClientId=@Id ", new { @Id = HttpContext.Session.GetInt32("Id") }).ToList();

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
        public JsonResult rejectJob(int Id, string FreelancerId)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                using (var tran = con.BeginTransaction())
                {

                    try
                    {
                        if (!con.Query<bool>("Select * from dbo.TransactionInfo Where FreelancerId=@FreelancerId and JobId=@JobId and ClientId=@ClientId and TranType='Assign'", new { @FreelancerId = FreelancerId, @JobId = Id, @ClientId = HttpContext.Session.GetInt32("Id") }, transaction: tran).FirstOrDefault())
                        {
                            con.Execute(@"Update dbo.TransactionInfo Set  TranType='Reject' Where JobId=@Id and FreelancerId=@FId ", new
                            {
                                @Id = Id,
                                @FId = FreelancerId
                            }, transaction: tran);

                        }
                        else
                        {
                            tran.Rollback();
                            return Json(new
                            {
                                msg = "Job Already Assigned Cananot Reject",
                                success = false
                            });
                        }
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
                msg = "Job Rejected Successfully !",
                success = true
            });
        }


        [SessionCheck]
        public IActionResult saveInterviewDetails(string Id,string FreelancerId, string Date, string Time)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    con.Execute(@"INSERT INTO [dbo].[InterviewInfo](FreelancerId,ClientId,JobId,InterviewTime,InterviewDate,RegisteredDate) 
Values(@FreelancerId,@ClientId,@JobId,@InterviewTime,@InterviewDate,@RegisteredDate)", new
                    {
                        @FreelancerId=FreelancerId,
                        @ClientId= HttpContext.Session.GetInt32("Id"),
                        @JobId=Id,
                        @InterviewTime=Date,
                        @InterviewDate=Time, 
                        @RegisteredDate = DateTime.Now
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
                    msg = "Interview Fixed Successfully",
                    success = true
                });
            }
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
                    allNotification = con.Query<Notification>("Select N.Description,U.FirstName as FreelancerName,N.RegisteredDate  from dbo.Notification N Inner Join dbo.[User] U On U.Id=N.FreelancerId Where ClientId=@Id", new { @Id = HttpContext.Session.GetInt32("Id") }).ToList();

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

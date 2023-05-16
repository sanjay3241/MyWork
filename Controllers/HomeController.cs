using Dapper;
using Microsoft.AspNetCore.Mvc;
using MyWork.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography.Xml;

namespace MyWork.Controllers
{
    public class HomeController : MasterController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        public IActionResult ClientRegister()
        {
            return View();
        }
        public IActionResult FreelancerRegister()
        {
            return View();
        }
        public IActionResult VerificationPage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterFreelancer(Login model)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open(); 

                try
                {
                    con.Execute(@"INSERT INTO [dbo].[User](FullName,Email,Password,Username,Token,Account_Type, IsActive,Registered_Date) Values(@FullName,@Email,
@Password,@Username,@Token,@Account_Type, @IsActive,@Registered_Date)", new
                    {
                        @FullName = model.Name,
                        @Email = model.Email,
                        @Password = model.Password,
                        @Username = model.Name,
                        @Token = model.Token,
                        @Account_Type = model.Account_Type,
                        @IsActive = 0,
                        @Registered_Date = DateTime.Now
                    });
                    if (CommonFunction.SendMail(model.Email, "Registration Code", model.Token))
                    {
                        return Json(new
                        {
                            msg = "Email Sent Successfully",
                            success = true
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            msg = "Error ! Sending Email",
                            success = false
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
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EmailConfirmation(Verification model)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    if (con.Query<string>("Select Token From dbo.[User] Where Email=@Email", new { @Email = model.Email }).FirstOrDefault() == model.Code)
                    {
                        con.Execute(@"Update dbo.[User] set IsActive=1 Where Email=@Email", new { @Email = model.Email });
                    }
                    else
                    {
                        return Json(new
                        {
                            msg = "Enter Valid Token",
                            success = false
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
                msg = "Email Verified Successfully",
                success = true
            });
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RegisterFreelancer(Login model)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                con.Execute(@"INSERT INTO [dbo].[User](FullName,Email,Password,Username,Token, IsActive,Registered_Date) Values(@FullName,@Email,
@Password,@Username,@Token, @IsActive,@Registered_Date)", new
                {
                    @FullName=model.Name,
                    @Email=model.Email,
                    @Password=model.Password,
                    @Username=model.Name,
                    @Token=model.Token,
                    @IsActive=0,
                    @Registered_Date=DateTime.Now
                });
            }

            return Json(new
            {
                msg = "Registerd Successfully",
                success = true
            });
        }

        public JsonResult Login()
        {
            return Json(new
            {
                Name = ""
            });
        }
    }
}
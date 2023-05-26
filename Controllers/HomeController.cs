using Dapper;
using Microsoft.AspNetCore.Mvc;
using MyWork.Models;
using MyWork.Support;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
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
        public IActionResult ReseatPassword()
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
                    if (!con.Query<bool>("Select IsActive From dbo.[User] Where Email=@Email And IsActive=0", new { @Email = model.Email }).FirstOrDefault())
                        con.Execute("Delete from dbo.[User] Where Email=@Email", new { @Email = model.Email });
                    else
                    {
                        return Json(new
                        {
                            msg = "User Exist With This Email, Try Another Email",
                            success = false
                        });
                    }

                    if (CommonFunction.SendMail(model.Email, "Registration Code", model.Token))
                    {
                        con.Execute(@"INSERT INTO [dbo].[User](FirstName,LastName,Email,Password,Username,Country,Token,Account_Type, IsActive,Registered_Date) 
Values(@FirstName,@LastName,@Email,@Password,@Username,@Country,@Token,@Account_Type, @IsActive,@Registered_Date)", new
                        {
                            @FirstName = model.FirstName,
                            @LastName = model.LastName,
                            @Email = model.Email,
                            @Password = model.Password,
                            @Country = model.Country,
                            @Username = model.FirstName + model.Token,
                            @Token = model.Token,
                            @Account_Type = model.Account_Type,
                            @IsActive = 0,
                            @Registered_Date = DateTime.Now
                        });

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

        public IActionResult ForgotPassword(string email)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                try
                {
                    Random random = new Random();
                    int randomNumber = random.Next(10000, 100000);
                    string randomNumberString = randomNumber.ToString();

                    while (randomNumberString.Length != 5)
                    {
                        randomNumber = random.Next(10000, 100000);
                        randomNumberString = randomNumber.ToString();

                    }
                    if (CommonFunction.SendMail(email, "Registration Code", randomNumberString))
                    {
                        con.Execute(@"Update [dbo].[User] Set Token=@Token, IsActive=@IsActive Where Email=@Email", new
                        {
                            @Email = email,
                            @Token = randomNumberString,
                            @IsActive = 0,
                        });
                    }
                    return Json(new
                    {
                        msg = "Email Sent Successfully",
                        success = true
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
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult reseatPassword(Verification model)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                try
                { 
                    con.Execute(@"Update [dbo].[User] Set Password=@Password Where Email=@Email", new
                    {
                        @Email = model.Email,
                        @Password = model.Password
                    });

                    return Json(new
                    {
                        msg = "Password Reseat Successfully",
                        success = true
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserLogin(Verification model)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    if (HttpContext.Session != null && HttpContext.Session.GetString("Email") != null)
                    {
                        return Json(new
                        {
                            msg = "Session is Already active",
                            success = true
                        });
                    }
                    else
                    {
                        if (con.Query<string>("Select Password From dbo.[User] Where Email=@Email", new { @Email = model.Email }).FirstOrDefault() == model.Password)
                        {
                            if (con.Query<bool>("Select IsActive From dbo.[User] Where Email=@Email", new { @Email = model.Email }).FirstOrDefault())
                            {
                                var userDetails = con.Query<dbUser>("Select * From dbo.[User] Where Email=@Email And Password=@Password", new { @Email = model.Email, @Password = model.Password }).FirstOrDefault();

                                HttpContext.Session.SetString("Email", userDetails.Email);
                                HttpContext.Session.SetInt32("Id", userDetails.Id);
                                HttpContext.Session.SetString("Password", userDetails.Password);
                                HttpContext.Session.SetString("Username", userDetails.Username);
                                HttpContext.Session.SetString("FirstName", userDetails.FirstName);
                                HttpContext.Session.SetString("FullName", userDetails.FirstName + " " + userDetails.LastName);
                                HttpContext.Session.SetString("LastName", userDetails.LastName);
                                HttpContext.Session.SetString("AccountType", userDetails.Account_Type);
                                HttpContext.Session.SetString("Status", userDetails.IsActive);

                                return Json(new
                                {
                                    msg = "Logged In Successfully !",
                                    data = userDetails.Account_Type,
                                    success = true
                                });
                            }
                            else
                            {
                                return Json(new
                                {
                                    msg = "Email Not Verified, Proceed To Registration!",
                                    success = true
                                });
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                msg = "Enter Valid Credential!",
                                success = false
                            });
                        }
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


        [SessionCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult updateUserDetails(Login model)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                try
                {
                    if (!con.Query<bool>("Select IsActive From dbo.[User] Where Email=@Email And IsActive=0", new { @Email = model.Email }).FirstOrDefault())
                        con.Execute("Delete from dbo.[User] Where Email=@Email", new { @Email = model.Email });
                    else
                    {
                        return Json(new
                        {
                            msg = "User Exist With This Email, Try Another Email",
                            success = false
                        });
                    }

                    con.Execute(@"INSERT INTO [dbo].[User](FirstName,LastName,Email,Password,Username,Country,Token,Account_Type, IsActive,Registered_Date) 
Values(@FirstName,@LastName,@Email,@Password,@Username,@Country,@Token,@Account_Type, @IsActive,@Registered_Date)", new
                    {
                        @FirstName = model.FirstName,
                        @LastName = model.LastName,
                        @Email = model.Email,
                        @Password = model.Password,
                        @Country = model.Country,
                        @Username = model.FirstName + model.Token,
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
    }
}
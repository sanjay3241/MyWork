using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MyWork.Controllers
{
    public class MasterController : Controller
    {
        public string connectionString;

        public MasterController()
        {
            var bul = WebApplication.CreateBuilder();
            connectionString = bul.Configuration.GetConnectionString("DefaultConnection");
        }
    }
}

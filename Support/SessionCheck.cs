using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyWork.Support
{ 
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Session == null || context.HttpContext.Session.GetString("Email") == null)
            {
                // Session is not active, redirect to a different page or perform any desired action
                context.Result = new RedirectResult("/Home/Register"); // Example: Redirect to the login page
            }

            base.OnActionExecuting(context);
        }
    }

}

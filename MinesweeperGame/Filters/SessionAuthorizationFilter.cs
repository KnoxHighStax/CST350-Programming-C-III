using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MinesweeperGame.Filters
{
    public class SessionAuthorizationFilter : ActionFilterAttribute
    {
        // Checking the session before executing
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("Id");
            // If no user ID in session
            if (userId == null)
            {
                // Redirect to login page
                context.Result = new RedirectToActionResult("Login", "User", null);
            }
        }
    }
}

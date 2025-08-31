using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MinesweeperGame.Filters
{
    public class SessionAuthorizationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
            if (context.HttpContext.Session.GetString("User") == null)
            {
                // Redirect to login page
                context.Result = new RedirectToActionResult("Index", "User", null);
            }
        }
    }
}

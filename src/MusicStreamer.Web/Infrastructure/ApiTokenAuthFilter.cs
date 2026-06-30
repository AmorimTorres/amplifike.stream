using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MusicStreamer.Web.Infrastructure;

public class ApiTokenAuthFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var token = context.HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            // Redirect to login for protected actions
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var actionName = context.RouteData.Values["action"]?.ToString();

            if (controllerName is not ("Home" or "Auth"))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}

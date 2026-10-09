using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace CME.Infrastructure
{
    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["LoginUserId"])))
            {
                FormsAuthentication.SignOut();
                //Redirecting the user to the Home controller  
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary { { "controller", "Home" }, { "action", "SessionExpire" } });
                return;
            }
        }

    }
}
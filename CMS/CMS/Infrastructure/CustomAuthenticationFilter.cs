using System;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace CME.Infrastructure
{
    public class CustomAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["LoginUserId"])))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            if (!string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["LoginUserStatus"])))
            {
                if (Convert.ToString(filterContext.HttpContext.Session["LoginUserStatus"]).ToUpper() != "TRUE")
                {
                    //Redirecting the user to the Home controller  
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary { { "controller", "Home" }, { "action", "Deactivate" } });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                //Redirecting the user to the Home controller  
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
            }
        }
    }
}
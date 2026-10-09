using DataAccessLayer;
using DataAccessLayer.DAL.UserRepository;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CME.Infrastructure
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IUserRepository _userRepository;
        private readonly string[] allowedroles;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roles"></param>
        public CustomAuthorizeAttribute(params string[] roles)
        {
            this._userRepository = new UserRepository(new NovoNordiskEntities());
            this.allowedroles = roles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            var userInitial = Convert.ToString(httpContext.Session["LoginUserInitial"]);
            if (!string.IsNullOrEmpty(userInitial))
            {
                var result = _userRepository.GetRole(userInitial);
                if (result != null)
                {
                    foreach (var role in allowedroles)
                    {
                        if (role.ToUpper() == result.ToUpper()) return true;
                    }
                }
            }
            return authorize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
               {
                    { "controller", "Home" },
                    { "action", "Index" }
               });
        }
    }
}
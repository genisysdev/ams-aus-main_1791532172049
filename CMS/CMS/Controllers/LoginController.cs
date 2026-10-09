using CME.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CME.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            ViewBag.Title = "Login";
            Session.Clear();
            return View();
        }

        [HttpPost]
        public ActionResult Index(Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Session["IsSuperAdmin"] = "";

                    var userCredentials = new Dictionary<string, (string Password, string Initial)>
                {
                    { "administrator", ("Password@11221", "RBDZ") },
                    { "initiator", ("Password@1231", "EYJP") },
                    { "approver", ("Password@1232", "APPR") },
                    { "finalapprover", ("Password@1233", "FINA") },
                    { "moderator", ("Password@1234", "MODR") }
                };

                    if (userCredentials.TryGetValue(login.UserName, out var credentials) && credentials.Password == login.Password)
                    {
                        System.Web.Security.FormsAuthentication.SetAuthCookie(login.UserName, false);
                        Session["LoginUserInitial"] = credentials.Initial;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        Session["LoginUserInitial"] = string.Empty;
                        TempData["MessageClass"] = "alert-danger";
                        TempData["Message"] = "Please enter valid User Name and Password";
                        return RedirectToAction("Index", "LogIn");
                    }
                }
                else
                {
                    return View(login);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}

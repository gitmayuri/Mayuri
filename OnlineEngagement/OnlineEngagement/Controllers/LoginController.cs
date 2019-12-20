using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineEngagement.Models;
using OnlineEngagement.Repository;

namespace OnlineEngagement.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        BasicRepository br = new BasicRepository();
        public ActionResult Index()
        {
            return View();
        }
        [ActionName("AdminLogin")]
        [HttpGet]
        public ActionResult Login()
        {
            Session.Clear();
            return View();
        }

        [ActionName("AdminLogin")]
        [HttpPost]
        public ActionResult Login(AdminModel lg)
        {
            ActionResult URL = null;
            TryUpdateModel(lg);
            if (ModelState.IsValid)
            {
                // List<AdminDetail> li = br.AdminLogin(lg);
                //lg.Password = EncryptPassword(lg);
                List<UserDetail> li = br.EngagementAdminLogin(lg);

                string message = string.Empty;

                if (li != null)
                {
                    switch (li.Count)
                    {
                        case 0:
                            message = "Username or Password is Incorrect.";
                            break;
                    }
                    if (li.Count > 0)
                    {
                        for (int i = 0; i < li.Count; i++)
                        {
                            HttpContext.Session["ClientId"] = li[i].Id;
                            HttpContext.Session["Name"] = li[i].Name;
                            HttpContext.Session["ContactEmail"] = li[i].EmailId;
                            //HttpContext.Session["ContactNo"] = li[i].Contact;
                        }
                        URL = RedirectToAction("Index", "Home");
                        //URL = RedirectToAction("ViewTest", "Client");
                    }
                    else
                    {
                        ViewBag.Message = message;
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = "Username or Password is Incorrect.";
                    return View();
                }
                return URL;
            }
            else
            {
                ViewBag.Message = "Username or Password is Incorrect.";
                return View();
            }
        }


        [ActionName("Logout")]
        [HttpGet]
        public ActionResult Logout()
        {

            //int usertype = Convert.ToInt32(Session["userTypeId"]);
            //if (usertype == 4)
            //{

            Session.Clear();
            return RedirectToAction("AdminLogin", "Login");

        }
    }
}
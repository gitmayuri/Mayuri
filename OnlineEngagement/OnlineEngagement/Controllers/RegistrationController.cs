using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineEngagement.Models;
using OnlineEngagement.Repository;

namespace OnlineEngagement.Controllers
{
    public class RegistrationController : Controller
    {
        // GET: Registration
        BasicRepository BR = new BasicRepository();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ActionName("registration")]

        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        [ActionName("Registration")]
        public ActionResult Registration(UserDetail UD)
        {
            TryUpdateModel(UD);
            bool i = BR.PostRegistration(UD);
            return View();

        }

        [HttpGet]
        [ActionName("GetCity")]
        public ActionResult GetCity()
        {
            return View();
        }

    }
}
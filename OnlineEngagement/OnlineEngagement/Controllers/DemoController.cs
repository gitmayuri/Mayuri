using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineEngagement.Models;
using OnlineEngagement.Repository;

namespace OnlineEngagement.Controllers
{
    public class DemoController : Controller
    {
        // GET: Demo
        BasicRepository BR = new BasicRepository();
        UserDetail UD = new UserDetail();

        List<UserDetail> li = new List<UserDetail>();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DemoModel()
        {
            li = BR.GetRegStudentList();
            return View(li);

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineEngagement.Controllers
{
    public class TestReassignController : Controller
    {
        // GET: TestReassign
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ActionName("TestReassign")]
        public ActionResult TestReassign()
        {
            return View();
        }
    }
}
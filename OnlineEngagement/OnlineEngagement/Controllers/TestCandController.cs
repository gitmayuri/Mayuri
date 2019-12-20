using OnlineEngagement.Models;
using OnlineEngagement.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineEngagement.Controllers
{
    public class TestCandController : Controller
    {
        BasicRepository BR = new BasicRepository();
        UserDetail UD = new UserDetail();
        ViewModelDashboard mymodel = new ViewModelDashboard();

        public ActionResult Index()
        {
            if (Session["ClientId"] != null && Session["Name"] != null)
            {
                authcodeCount();
                return View(UD);
            }
            else
            {
                return RedirectToAction("AdminLogin", "Login");
            }
        }

        public ActionResult authcodeCount()
        {
            int authcodeCount = BR.authcodeCount();
            HttpContext.Session["authcount"] = authcodeCount;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetTestCompletedCandidate()
        {
            List<UserDetail> li = BR.GetTestCompletedCandidateList();
            System.Threading.Thread.Sleep(2000);
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PostApproveGraph(string TCompCandID)
        {
            int ApprovedBy = Convert.ToInt32(Session["ClientId"]);
            Boolean flag = BR.PostApproveGraph(TCompCandID, ApprovedBy);
            string msg;
            if (flag == true)
            {
                msg = "Graph Approved Successfully...";
            }
            else
            {
                msg = "Graph is Not Approved Beacuse Some Test are Reassigned...";
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult BindTest(int candIdTOGetTest)
        {
            List<SelectListItem> li = new List<SelectListItem>();
            UserDetail SD = new UserDetail();

            SD.AllTest = BR.GetDDLAllTest(candIdTOGetTest);
            ViewBag.test = SD.AllTest;
            var test = SD.AllTest;
            return Json(SD.AllTest, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ReassignCandidateTest(int TestId, int TestCompCandId)
        {
            int flag = BR.CandidateTestReassign(TestId, TestCompCandId);

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}
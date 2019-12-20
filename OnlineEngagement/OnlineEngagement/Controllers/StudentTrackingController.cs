using OnlineEngagement.Models;
using OnlineEngagement.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineEngagement.Controllers
{
    public class StudentTrackingController : Controller
    {
        BasicRepository BR = new BasicRepository();
        // GET: StudentTracking
        public ActionResult Index()
        {
            GetSesCDFTrackingDtl();
            // GetTestProdTrackingDtl();
            return View();
        }
        [HttpGet]
        [ActionName("StudentTracking")]
        public ActionResult GetSesCDFTrackingDtl()
        {
            int StudentId = 8;
            StudentTracking ST = new StudentTracking();
            ST = BR.GetSesCDFTrackingDtl(StudentId);
            Session["SessionStatus"] = ST.SesStatus;
            Session["CDFName"] = ST.CDFName;
            GetTestProdTrackingDtl();
            return View();

        }

        [HttpGet]

        public ActionResult GetTestProdTrackingDtl()
        {
            int StudentId1 = 8;
            StudentTracking ST = new StudentTracking();
            ST = BR.GetTestProdTrackingDtl(StudentId1);
            Session["StudentTest"] = ST.TestCompDate;
            Session["ProductPurchase"] = ST.ProdPurchDate;
            return View();
        }
    }
}
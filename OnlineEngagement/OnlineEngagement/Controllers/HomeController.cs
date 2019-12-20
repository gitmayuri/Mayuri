using OnlineEngagement.Models;
using OnlineEngagement.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace OnlineEngagement.Controllers
{
    public class HomeController : Controller
    {
        BasicRepository BR = new BasicRepository();
        UserDetail UD = new UserDetail();
        ViewModelDashboard mymodel = new ViewModelDashboard();

        public ActionResult Index()
        {
            if (Session["ClientId"] != null && Session["Name"] != null)
            {
                // BindSessionArea(Pincode);
                GetTestReassignedCandCount();
                authcodeCount();
                GetCommentAndRatingCount();
                GetProdPurchaseStudCount();
                GetRegisteredStudentCount();
                GetSessionCmpltStudCount();
                GetTestCompletedCandCount();
                GetAsignCDFSessionCount();
                GetSPPFeedbackCount();
                //   List<UserDetail> li = BR.GetRegStudentList();
                mymodel.UserDetail = BR.GetRegStudentList();
                //int pageSize = 3;
                //int pageNumber = (page ?? 1);
                // return View(li.ToPagedList(pageNumber, pageSize));


                //GetTotalRegisteredStudent(page);
                return View(mymodel);
            }
            else {
                return RedirectToAction("AdminLogin", "Login");
            }
        }
       

        [AcceptVerbs(HttpVerbs.Get)]

        public JsonResult GetProductPurchaseStudent()
        {
            List<UserDetail> li = BR.GetProductPurchaseStudentList();
            // mymodel.UserDetail= BR.GetProductPurchaseStudentList();
            return Json(li, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Get)]

        public JsonResult GetTestCompletedCandidate()
        {
            List<UserDetail> li = BR.GetTestCompletedCandidateList();
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]

        public JsonResult GetTestReassignedCandidate()
        {
            List<UserDetail> li = BR.GetTestReassignedCandidateList();
            return Json(li, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Get)]

        public JsonResult GetAsignCDFSessionDtl()
        {
            List<UserDetail> li = BR.GetAsignCDFSessionDtlForCand();
            return Json(li, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetSessionCompleteDetails()
        {
            List<UserDetail> li = BR.GetSessionCompleteDetails();
            //mymodel.UserDetail = BR.GetSessionCompleteDetails();
            return Json(li, JsonRequestBehavior.AllowGet);

        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetStudentCDFDeails(int CandID)
        {
            List<UserDetail> li = BR.GetStudentCDFDeails(CandID);

            // mymodel.UserDetail = BR.GetStudentCDFDeails(CandID);
            return Json(li, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetStudentSessionDeails(int CandID)
        {

            List<UserDetail> li = BR.GetStudentSessionDeails(CandID);
            //  mymodel.UserDetail = BR.GetStudentSessionDeails(CandID);
            return Json(li, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetStudentShadowDeails(int CandID)
        {
            List<UserDetail> li = BR.GetStudentShadowDetails(CandID);
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetStudentDeailsByID(int CandID)
        {
            List<UserDetail> li = BR.GetStudentDeailsByID(CandID);
            // mymodel.UserDetail = BR.GetStudentDeailsByID(CandID);
            return Json(li, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetCDFListToAssign(string lblStudentPinCode)
        {
            List<UserDetail> li = BR.GetCDFListToAssign(lblStudentPinCode);
            //  mymodel.UserDetail = BR.GetCDFListToAssign();
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
                msg = "Graph Not Approved...";
            }
            //  mymodel.UserDetail = BR.GetCDFListToAssign();
            return Json(msg, JsonRequestBehavior.AllowGet);

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ReassignCandidateTest(int TestId,int TestCompCandId)
        {
            int flag = BR.CandidateTestReassign(TestId, TestCompCandId);

            return Json(flag, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult GetDetail(int? page)
        //{
        //    int pageSize = 4;
        //    int pageNumber = (page ?? 1);
        //    return PartialView("Index", li.ToPagedList(pageNumber, pageSize));
        //}



        [HttpGet]

        public ActionResult GetRegisteredStudentCount()
        {
            UD.RegStudentCount = BR.GetRegistredStudentCount();
            HttpContext.Session["TotRegCount"] = UD.RegStudentCount;


            return View(UD);


        }
        [HttpGet]

        public ActionResult authcodeCount()
        {
            int authcodeCount = BR.authcodeCount();
            HttpContext.Session["authcount"] = authcodeCount;

            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAuthcodePrivew()
        {
            List<Authcode> li = BR.GetAuthcodePrivew();
            return Json(li, JsonRequestBehavior.AllowGet);

        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAuthcodePrivewById(int promotorId)
        {
            List<Authcode> li = BR.GetAuthcodePrivewById(promotorId);
            return Json(li, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]

        public ActionResult GetAsignCDFSessionCount()
        {
            int AsgnCDFSenCount = BR.GetAsignCDFSessionForCandCount();
            HttpContext.Session["AsignCDFSesCount"] = AsgnCDFSenCount;
            return View(UD);


        }

        [HttpGet]

        public ActionResult GetProdPurchaseStudCount()
        {
            int ProdPurchaseCount = BR.GetProdPurchaseStudCount();
            HttpContext.Session["TotProdPurchaseCount"] = ProdPurchaseCount;


            return View(UD);


        }


        [HttpGet]

        public ActionResult GetSessionCmpltStudCount()
        {
            int SessionCompleteCount = BR.GetSessionCmpltStudCount();
            HttpContext.Session["SessionCompleteCount"] = SessionCompleteCount;
            return View(UD);
        }

        [HttpGet]

        public ActionResult GetTestCompletedCandCount()
        {
            int Testcount = BR.GetTestCompletedCandCount();
            HttpContext.Session["TestCompleteCount"] = Testcount;

            return View();
        }


        [HttpGet]

        public ActionResult GetTestReassignedCandCount()
        {
            int TestReassigncount = BR.GetTestReassignedCandCount();
            HttpContext.Session["TestReassignedCount"] = TestReassigncount;

            return View();
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

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetUserFeedbackList()
        {
            List<UserDetail> li = BR.GetUserFeedbackDtl();
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSPPFeedbackCount()
        {
            int FeedbackCount = BR.GetSPPFeedbackCount();
            HttpContext.Session["SPPFeedbackCount"] = FeedbackCount;
            return View();
        }

        [HttpGet]

        public ActionResult GetCommentAndRatingCount()
        {
            int commentandRating = BR.GetCommentAndRatingCount();
            HttpContext.Session["commentandRating"] = commentandRating;
            return View(UD);


        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult CommentRating()
        {
            List<CommnetAndRating> li = BR.GetCommentAndRating();
            return Json(li, JsonRequestBehavior.AllowGet);
        }




        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetUserQuesAnsDtlById(int id)
        {
            List<FeedBackDetail> questions = BR.GetFeedBackQuestions(id);
            SPP_FBQuestionSet QuesList = BR.GetUserQuesAnsDtlById(questions);
            return Json(QuesList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetParentFeedbackQuesAnsSet(int id)
        {
            List<FeedBackDetail> questions = BR.GetParentFeedbackQuesAns(id);
            SPP_FBQuestionSet QuesList = BR.GetParentFeedbackQuesAnsById(questions);
            return Json(QuesList, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult PostSessionCDFDetails(SessionDetails obj)
        //{
        //    SessionDetails SD = new SessionDetails();
        //    SD.StudID = obj.StudID;
        //    SD.CDFId = obj.CDFId;
        //    SD.sessionVenue = obj.sessionVenue;
        //    SD.sessionPinCode = obj.sessionPinCode;
        //    SD.sessionArea = obj.sessionArea;
        //    SD.sessionDate = obj.sessionDate;
        //    SD.SessionTime = obj.SessionTime;
        //    bool i = BR.InsertSessionCDFData(SD);
        //    if (i == true)
        //    {
        //        ViewBag.Message = "Data Inserted...";
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Data Not Inserted...";
        //    }

        //    return View();
        //}

        //[AcceptVerbs(HttpVerbs.Get)]
        //[HttpPost]
        //public JsonResult BindSessionArea(string Pincode)
        //{
        //     //TempData["Pincode"] = Pincode;
        //     SessionDetails SD = new SessionDetails();
        //    Pincode = "504307";
        //    SD.sessionArea = BR.GetDDLSessionArea(Pincode);
        //    ViewBag.sessionArea = SD.sessionArea;
        //   // var Area = SD.sessionArea;
        //    return Json(SD.sessionArea, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
        
       
    }
}
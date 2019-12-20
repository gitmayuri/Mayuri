using OnlineEngagement.Models;
using OnlineEngagement.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineEngagement.Controllers
{
   
    public class SessionController : Controller
    {
        public static int id { get; set; }
        BasicRepository BR = new BasicRepository();
        SessionDetailsModel SD = new SessionDetailsModel();
        //SessionModelDashboard mymodel = new SessionModelDashboard();
       
        // GET: Session
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult Details(int uid)
        {
            HttpContext.Session["StdId"] = uid;
            int id =Convert.ToInt32(Session["StdId"]);
            return View();
        }

        [HttpGet]
        public ActionResult SessionDtl()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetSessionDetails()
        {
            int CandID = Convert.ToInt32(Session["StdId"]);
            List<SessionDetailsModel> li = BR.GetSessionDetailsByID(CandID);
            return Json(li,JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetStudentDeailsByID()
        {
            int CandID = Convert.ToInt32(Session["StdId"]);
            List<SessionDetailsModel> li = BR.GetStudentDtlByID(CandID);
            return Json(li, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetStudentCDFDeails()
        {
            int CandID = Convert.ToInt32(Session["StdId"]);
            List<SessionDetailsModel> li = BR.GetStudentCDFDeailsById(CandID);
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStudentShadowDeails()
        {
            int CandID = Convert.ToInt32(Session["StdId"]);
            List<SessionDetailsModel> li = BR.GetStudentShadowDetailsById(CandID);
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStudentSessionDeails()
        {
            int CandID = Convert.ToInt32(Session["StdId"]);
            List<SessionDetailsModel> li = BR.GetStudentSessionDeailsById(CandID);
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetUserQuesAnsDtlById()
        {
            int id = Convert.ToInt32(Session["StdId"]);
            List<FeedBackDetailDtl> questions = BR.GetFeedBackQuestionsDtl(id);
            SPP_FBQuestionSetDtl QuesList = BR.GetUserQuesAnsDtlById(questions);
            return Json(QuesList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetParentFeedbackDtl()
        {
            int id = Convert.ToInt32(Session["StdId"]);
            List<ParentFeedBackDetail> questions = BR.GetParentFeedbackQuesAnsDtl(id);
            Parent_FBQuestionSetDtl QuesList = BR.GetParentFeedbackQuesAnsById(questions);
            return Json(QuesList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult FdbCommentAndRating()
        {
            int id = Convert.ToInt32(Session["StdId"]);
            List<CommentsAndRatingDtl> cmtRat = BR.GetComRatingFdbDtlById(id);
            return Json(cmtRat, JsonRequestBehavior.AllowGet);
        }
    }
}
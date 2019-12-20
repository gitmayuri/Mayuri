using OnlineEngagement.Models;
using OnlineEngagement.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineEngagement.Controllers
{
    public class StudentListController : Controller
    {
        ViewModel mymodel = new ViewModel();
        BasicRepository BR = new BasicRepository();
        int StudentId;
        //string CandidateName;
        // GET: StudentList
        public ActionResult Index()
        {
            return View();
        }


        //[HttpPost]
        //public ActionResult Index(string selectedTime)
        //{
        //    ViewBag.Message = "Selected Time: " + selectedTime;
        //    return View();
        //}

        //[HttpGet]
        //[ActionName("GetStudentMoreInfo")]
        //public ActionResult GetStudentMoreInfo()
        //{
        //    //GetStudentMoreInfo1();
        //    //GetCDFDetails();
        //    return View();
        //}

        [HttpGet]
        //[ActionName("GetStudentMoreInfo")]
        public ActionResult GetStudentMoreInfo()
        {
            mymodel.UserDetail = BR.GetStudentMoreInfo(StudentId);
            //mymodel.CDFDetails = BR.GetCDFDetails(StudentId);
            string fullName = "";
            fullName = mymodel.UserDetail.Fname + " " + mymodel.UserDetail.Lname;
            mymodel.UserDetail.Name = fullName;
            return View(mymodel);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetCDFListToAssign(string lblStudentPinCode)
        {
            //CandidateName = HttpContext.Session["CandName"].ToString();
            //  TempData["StudentName"] = CandidateName;
            // ViewBag.StudentName = CandidateName;
            //mymodel.UserDetail.StudentName = CandidateName; 
            int CandId = Convert.ToInt32(HttpContext.Session["CandId"]);
            string CandPinCode = HttpContext.Session["CandPinCode"].ToString();
            List<UserDetail> li = BR.GetCDFListToAssignStudent(CandPinCode, CandId);
            //  mymodel.UserDetail = BR.GetCDFListToAssign();
            return Json(li, JsonRequestBehavior.AllowGet);
            
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetShadowCDFListToAssign()
        {
            //CandidateName = HttpContext.Session["CandName"].ToString();
            int CandId = Convert.ToInt32(HttpContext.Session["CandId"]);
            string CandPinCode = HttpContext.Session["CandPinCode"].ToString();
            List<UserDetail> li = BR.GetShadowCDFListToAssignStudent(CandPinCode, CandId);
            //  mymodel.UserDetail = BR.GetCDFListToAssign();
            
            return Json(li, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetShadowCDFListToAssignByPincode(string pincode)
        {
            //CandidateName = HttpContext.Session["CandName"].ToString();
            int CandId = Convert.ToInt32(HttpContext.Session["CandId"]);
            List<UserDetail> li = BR.GetShadowCDFListToAssignStudentByPincode(pincode, CandId);
            //  mymodel.UserDetail = BR.GetCDFListToAssign();
            //System.Threading.Thread.Sleep(2000);
            return Json(li, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetShadowCDFListToAssignByEmailId(string emailId)
        {
            //CandidateName = HttpContext.Session["CandName"].ToString();
            int CandId = Convert.ToInt32(HttpContext.Session["CandId"]);
            List<UserDetail> li = BR.GetShadowCDFListToAssignStudentByEmailId(emailId, CandId);
            //  mymodel.UserDetail = BR.GetCDFListToAssign();
            //System.Threading.Thread.Sleep(1000);
            return Json(li, JsonRequestBehavior.AllowGet);
         }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetShadowCDFListToAssignByCityName(string City)
        {
            //CandidateName = HttpContext.Session["CandName"].ToString();
            int CandId = Convert.ToInt32(HttpContext.Session["CandId"]);
            List<UserDetail> li = BR.GetShadowCDFListToAssignStudentByCityName(City, CandId);
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetCDFListToAssignByPinCode(int PinCode)
        {
            //CandidateName = HttpContext.Session["CandName"].ToString();
            int CandId = Convert.ToInt32(HttpContext.Session["CandId"]);
            List<UserDetail> li = BR.GetCDFListAssignToStudByPincode(CandId, PinCode);
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetCDFListToAssignByEmailId(string EmailId)
        {
            //CandidateName = HttpContext.Session["CandName"].ToString();
            int CandId = Convert.ToInt32(HttpContext.Session["CandId"]);
            List<UserDetail> li = BR.GetCDFListAssignToStudByEmailId(CandId, EmailId);
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetCDFListToAssignByCityName(string City)
        {

            //CandidateName = HttpContext.Session["CandName"].ToString();
            int CandId = Convert.ToInt32(HttpContext.Session["CandId"]);
            List<UserDetail> li = BR.GetCDFListAssignToStudByCity(CandId, City);
            return Json(li, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult City_Bind()
        {

            List<SelectListItem> citylist = new List<SelectListItem>();
            UserDetail userDetail = new UserDetail();
            userDetail.Cities = BR.Get_City();
            ViewBag.city = userDetail.Cities;
            return Json(userDetail.Cities, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShadowCDF_City_Bind()
        {

            List<SelectListItem> citylist = new List<SelectListItem>();
            UserDetail userDetail = new UserDetail();
            userDetail.Cities = BR.Get_City();
            ViewBag.city = userDetail.Cities;
            return Json(userDetail.Cities, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PostSessionCDFDetails(SessionDetails obj)
        {
            //string Message = "";
            SessionDetails SD = new SessionDetails();
            StudentId = Convert.ToInt32(HttpContext.Session["CandId"]);
            SD.LoginClientId = Convert.ToInt32(HttpContext.Session["ClientId"]);
            SD.CandId = StudentId;
            SD.CDFId = obj.CDFId;
            SD.sessionVenue = obj.sessionVenue;
            // SD.sessionPinCode = obj.sessionPinCode;
            SD.AreaId = obj.AreaId;
            SD.SesDate = obj.SesDate.ToString();
            //SD.SessionTime = obj.SessionTime;
            SD.FinalTime = obj.FinalTime;
            SD.CDFName = obj.CDFName;
            SD.CDFContact = obj.CDFContact;
            bool i = BR.InsertSessionCDFData(SD);
            //if (i == true)
            //{
            //    Message = "CDF And Session Assigned Successfully...";
            //}
            //else
            //{
            //    Message = "CDF And Session Not Assigned...";
            //}
            //return Json(Message, JsonRequestBehavior.AllowGet);
            //return View();
            return Json(new { result = i, url = Url.Action("GetStudentMoreInfo", "StudentList") });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PostShadowCDFDetails(int ShadowCDFID, string ShadowCDFName, string ShadowCDFContact)
        {
            //string Message = "";
            SessionDetails SD = new SessionDetails();
            StudentId = Convert.ToInt32(HttpContext.Session["CandId"]);
            SD.CandId = StudentId;
            SD.ShadowCDF = ShadowCDFID;
            SD.ShadowName = ShadowCDFName;
            SD.ShadowContact = ShadowCDFContact;
            bool i = BR.InsertShadowCDFData(SD);
            //if (i == true)
            //{
            //    Message = "Shadow CDF Assigned Successfully...";
            //}
            //else
            //{
            //    Message = "Shadow CDF Not Assigned...";
            //}
            //return Json(Message, JsonRequestBehavior.AllowGet);
            //return View();
            return Json(new { result = i, url = Url.Action("GetStudentMoreInfo", "StudentList") });
        }




        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult BindSessionArea(string Pincode)
        {
            List<SelectListItem> li = new List<SelectListItem>();
            SessionDetails SD = new SessionDetails();
            SD.sessionArea = BR.GetDDLSessionArea(Pincode);
            ViewBag.Area = SD.sessionArea;
            var Area = SD.sessionArea;
            return Json(SD.sessionArea, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult BindState()
        {
            List<SelectListItem> li = new List<SelectListItem>();
            SessionDetails SD = new SessionDetails();
            SD.sessionState = BR.GetDDLSessionState();
            ViewBag.state = SD.sessionState;
            var Area = SD.sessionState;
            return Json(SD.sessionState, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult BindCity(int stateId)
        {
            List<SelectListItem> li = new List<SelectListItem>();
            SessionDetails SD = new SessionDetails();
            SD.sessionCity = BR.GetDDLSessionCity(stateId);
            ViewBag.city = SD.sessionCity;
            var Area = SD.sessionCity;
            return Json(SD.sessionCity, JsonRequestBehavior.AllowGet);

        }

        //[ChildActionOnly]
        //[HttpGet]
        //public ActionResult GetCDFDetails()
        //{
        //    //UserDetail UD = new UserDetail();
        //    //List<CDFDetails> li = BR.GetCDFDetails();
        //    //for (int i = 0; i < li.Count; i++)
        //    //{
        //    //    UD.CDFName = li[i].CDFName;
        //    //    UD.CDFEmail = li[i].CDFEmail;
        //    //    UD.CDFContact = li[i].CDFContact;
        //    //    UD.CDFCity = li[i].CDFCity;
        //    //}
        //     return View(mymodel);
        //    //return PartialView("_GetCDFDetailsPartial",li);
        //}
    }
}
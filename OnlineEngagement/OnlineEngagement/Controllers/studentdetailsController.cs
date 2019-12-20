using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineEngagement.Models;
using System.Configuration;
using OnlineEngagement.Repository;

namespace OnlineEngagement.Controllers
{
    public class studentdetailsController : Controller
    {
        string strcon = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        StudentDetails Stud = new StudentDetails();
        BasicRepository BR = new BasicRepository();
      
        public ActionResult Index()
        {
            
            return View();
        }
        
       
        [ActionName("StudentDetails")]
        public ActionResult StudentDetails()
        {
            List<StudentDetails> li = BR.GetStudentDetails();        

            return View(li);
        }
        //public ActionResult Details(int Id)
        //{
        //    StudentDetails student = new Models.StudentDetails();
        //    student =StudentDetails.
        //    return View("Details", student);
        //}

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetStudentDetailsById()
        {
            List<StudentDetails> li = BR.GetStudentDetails();
            return Json(li, JsonRequestBehavior.AllowGet);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineEngagement.Models;
using OnlineEngagement.Repository;

namespace OnlineEngagement.Controllers
{
    public class StudentController : Controller
    {
        UserDetail mymodel = new UserDetail();
        // GET: Student
        BasicRepository BR = new BasicRepository();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ActionName("GetStudentList")]
        public ActionResult GetStudentList()
        {
            List<UserDetail> li = BR.GetAllStudentList();
            return View(li);
        }

        [HttpGet]
        public ActionResult StudentMoreInfo(int id, string pincode, string studName,string email, string City)
        {

            HttpContext.Session["CandId"] = id;
            HttpContext.Session["CandPinCode"] = pincode;
            HttpContext.Session["CandName"] = studName;
            HttpContext.Session["EmailId"] = email;
            HttpContext.Session["City"] = City;
            return RedirectToAction("GetStudentMoreInfo", "StudentList");

        }
    }
}
using OnlineEngagement.Models;
using OnlineEngagement.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineEngagement.Controllers
{
    public class AuthCodeGenerationController : Controller
    {
        BasicRepository BR = new BasicRepository();
        // GET: AuthCodeGeneration
        public ActionResult Index()
        {
            return View();
        } 

        public ActionResult AuthCodeGeneration()
        {
            authCode AC = new authCode();    
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetProductDetails()
        {
            List<SelectListItem> li = new List<SelectListItem>();
            authCode AC = new authCode();
            AC.ProductName = BR.GetProductDetails();
            ViewBag.product = AC.ProductName;
            var Product = AC.ProductName;
            return Json(AC.ProductName, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PostAuthCode(authCode AuthC)
        {
            object userId = HttpContext.Session["ClientId"];
            //string Message = "";
            authCode AC = new authCode();
            AC.userId = userId;
            AC.ProductId = AuthC.ProductId;
            AC.CreatedFor = AuthC.CreatedFor;
            AC.NoOfCodes = AuthC.NoOfCodes;
            AC.ValidityInDays = AuthC.ValidityInDays;
            AC.Comment = AuthC.Comment;
            bool i = BR.PostAuthCode(AC);
            //if (i == true)
            //{
            //    Message = "AuthCode Generated Successfully...";
            //}
            //else
            //{
            //    Message = "AuthCode Not Generated...";
            //}
            //return Json(Message, JsonRequestBehavior.AllowGet);
            return Json(new { result = i, url = Url.Action("AuthCodeGeneration", "AuthCodeGeneration") });
            //return View();
        }

    }
}
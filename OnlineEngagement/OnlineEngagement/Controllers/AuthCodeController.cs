using OnlineEngagement.Models;
using OnlineEngagement.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineEngagement.Controllers
{
    public class AuthCodeController : Controller
    {
        BasicRepository BR = new BasicRepository();
        UserDetail UD = new UserDetail();
        ViewModelDashboard mymodel = new ViewModelDashboard();
        Authcode au = new Authcode();

        public ActionResult Index()
        {
            mymodel.UserDetail = BR.GetRegStudentList();
            return View(au);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAuthcodePrivew()
        {
            List<Authcode> li = BR.GetAuthcodePrivew();
            System.Threading.Thread.Sleep(2000);
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAuthcodePrivewById(int promotorId)
        {
            List<Authcode> li = BR.GetAuthcodePrivewById(promotorId);
            return Json(li, JsonRequestBehavior.AllowGet);

        }
    }
}
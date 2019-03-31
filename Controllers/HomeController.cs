using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using portal.mps.Data;
using portal.mps.Data.Repository;
using portal.mps.Models;
using portal.mps.Services;

namespace portal.mps.Controllers
{
    public class HomeController : Controller
    {
        private IEmailSender _mailer;
        private IUtils _utils;
        private IStudentsRepository _repo;

        public HomeController(Services.IEmailSender mailer, IUtils utils, IStudentsRepository repo)
        {
            _mailer = mailer;
            _utils = utils;
            _repo = repo;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult getUserDetails(string username)
        {
            var u = _utils.getUserFromUserNameAsync(username).Result;
            string role = _utils.GetRolesAsync(u).Result;
            string exep="";
            string imgUrl=u.UserPic==null?"":_utils.download(u.UserPic,out exep);
            if(!string.IsNullOrEmpty(exep)){imgUrl="";}
            StudentSlab s= new StudentSlab();
            Staff st = new Staff();
            if(role=="Student"){s = _utils.getStudentFromUserAsync(u).Result;}
            if(role=="Staff" || role =="Teacher"){st = _utils.getStaffFromUserAsync(u).Result;}
            return Json(new {user = u, role=role, student= s, staff=st, imgUrl = imgUrl});
        }
        [HttpPost]
        public JsonResult getUserRole(string username)
        {
            var u = _utils.getUserFromUserNameAsync(username).Result;
            string role = _utils.GetRolesAsync(u).Result;
            return Json(new{role = role});
        }
        [HttpPost]
        public JsonResult getStudentDetails(string username)
        { 
            var m = _repo.getStudentMatrixFromUserName(username);  
            return Json(m);
        }
        [Authorize(Roles="Student,Admin,SuperAdmin")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        [HttpPost]
        public IActionResult Send(string email)
        {
            //"saudkhan03@outlook.com"
            bool b = _mailer.SendEmail(email,"","test","test");
            return RedirectToAction("Index");
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

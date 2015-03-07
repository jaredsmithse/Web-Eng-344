using Facebook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebPortal.Models;
using WebPortal.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Owin;

namespace WebPortal.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {
        private AccountController ac = new AccountController();
        private System.Threading.Tasks.Task<ActionResult> test;
        [Authorize]
        public ActionResult Index()
        {
            return View(this.ac.FacebookFeed());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
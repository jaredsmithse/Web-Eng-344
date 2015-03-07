using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebPortal.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        //
        // GET: /Calendar/
        public ActionResult Index()
        {
            return View();
        }
	}
}
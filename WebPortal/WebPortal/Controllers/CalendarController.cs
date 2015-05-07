using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPortal.Models;

namespace WebPortal.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        // GET: /Calendar/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getEvents()
        {
            using (var db = new WebPortalContext())
            {
                var events = db.CalEvents.Where(e => e.user == User.Identity.Name).ToList<CalEvent>();

                Object[] eventsArray = new Object[events.Count];
                for (int i = 0; i < events.Count; i++ )
                {
                    CalEvent e = events.ElementAt(i);
                    eventsArray[i] = new { title = e.title, start = e.start, end = e.end };
                }
                var json= Json(eventsArray, JsonRequestBehavior.AllowGet);
                return json;
            }
        }

        public ActionResult addEvent(string title, DateTime? start, DateTime? end)
        {
            if (start == null || end == null)
            {
                return Content("Start and End date cannot be left empty. Click Back to return to Calendar.");
            }

            if (start > end)
            {
                return Content("Start Date cannot come after End Date. Click Back to return to Calendar.");
            }

            using (var db = new WebPortalContext())
            {
                var e = new CalEvent();
                e.title = title;
                e.start = start;
                e.end = end;
                e.user = User.Identity.Name;

                db.CalEvents.Add(e);
                db.SaveChanges(); 
            }
            return View("Index");
        }

        public ActionResult ExportEvents()
        {
            string csv = "title\tstart\tend\n";
            using (var db = new WebPortalContext())
            {
                var events = db.CalEvents.Where(e => e.user == User.Identity.Name).ToList<CalEvent>();
                foreach(var e in events)
                {
                    csv += e.title + "\t" + e.start + "\t" + e.end + "\n";
                }
            }
            string fileName = "CalendarEventData.csv";
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", fileName);
        }
	}
}
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
        //
        // GET: /Calendar/
        public ActionResult Index()
        {
            var u = User.Identity.Name;

            /*
            using (var db = new CalEventDBContext())
            {
                var events = db.CalEvents.ToList<CalEvent>();
                Console.WriteLine(events);
            }
            */

            return View();
        }

        public ActionResult getEvents()
        {
            using (var db = new WebPortalContext())
            {
                // TODO only grab events for current user
                var events = db.CalEvents.ToList<CalEvent>();

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

        public ActionResult addEvent(string title, DateTime start, DateTime end)
        {
            // TODO check start and end != null
            // TODO check end time > start time
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
	}
}
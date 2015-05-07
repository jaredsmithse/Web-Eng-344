using System;
using System.Collections.Generic;
using System.IO;
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

        [HttpPost]
        public ActionResult UploadEvents(HttpPostedFileBase file)
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                var events = new List<CalEvent>();
                try
                {
                    // Read File
                    string fileData = new StreamReader(file.InputStream).ReadToEnd();
                    // Parse
                    string[] fileDataArray = fileData.Split('\n');
                    // Start at [1] because [0] is header
                    for (int i = 1; i < fileDataArray.Length; i++)
                    {
                        var eventData = fileDataArray[i];
                        if (eventData != null && eventData.Length > 0)
                        {
                            // Form of line is Title\tStart\tEnd
                            var eventDataArray = eventData.Split('\t');
                            var e = new CalEvent();
                            e.title = eventDataArray[0];
                            e.start = Convert.ToDateTime(eventDataArray[1]);
                            e.end = Convert.ToDateTime(eventDataArray[2]);
                            e.user = User.Identity.Name;
                            events.Add(e);
                        }
                    }
                }
                catch
                {
                    return Content("Error trying to read file. Click Back and try a different File");
                }

                try
                {
                    using (var db = new WebPortalContext())
                    {
                        db.CalEvents.AddRange(events);
                        db.SaveChanges();
                    }
                }
                catch
                {
                    return Content("Error trying to save to Database. Click Back and try again");
                }
            }
            return RedirectToAction("Index");
        }
	}
}